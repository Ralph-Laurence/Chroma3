using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Revamp
{
    public partial class GameManager : MonoBehaviour
    {
        private GameManagerStates currentState;
        private GameSessionManager gsm;
        private BackgroundMusic bgm;
        private SoundEffects sfx;
        
        //
        //================================================
        //             MONOBEHAVIOUR METHODS            //
        //================================================
        //
        #region MONOBEHAVIOUR

        void OnEnable()
        {
            SceneManager.sceneLoaded   += OnCutSceneLoaded;
            SceneManager.sceneUnloaded += OnCutSceneUnLoaded;

            AttachEventObservers();
        }
        
        void OnDisable()
        {
            SceneManager.sceneLoaded   -= OnCutSceneLoaded;
            SceneManager.sceneUnloaded -= OnCutSceneUnLoaded;

            DetachEventObservers();
        }
        
        void Awake()
        {
            currentState = GameManagerStates.Active;

            gsm = GameSessionManager.Instance;
            bgm = BackgroundMusic.Instance;
            sfx = SoundEffects.Instance;

            stageTimer.OnTimesUp = () => {
                BreakExecution();
                OnGameFailed();
            };
        }

        void Start()
        {
            stageFactory.Create
            (
                gsm.SelectedDifficulty, 
                gsm.SelectedStageNumber
            );
        }
        
        #endregion MONOBEHAVIOUR

        //
        //================================================
        //              GAME MECHANICS LOGIC            //
        //================================================
        //
        #region GAME_MECHANICS

        private readonly int maxStarsPerStage = 3;
        public GameManagerStates GetState() => currentState;
        public static int TotalEasyStages   => 30;
        public static int TotalNormalStages => 25;
        public static int TotalHardStages   => 14; //20;

        public static int GetTotalStages(LevelDifficulties difficulty)
        {
            return difficulty switch
            {
                LevelDifficulties.Easy => TotalEasyStages,
                LevelDifficulties.Normal => TotalNormalStages,
                LevelDifficulties.Hard => TotalHardStages,
                _ => 0,
            };
        }

        private void PauseTimeScale() => Time.timeScale = 0.0F;
        private void ResumeTimeScale() => Time.timeScale = 1.0F;
        #endregion GAME_MECHANICS

        #region GAME_OVER
        private void OnGameSuccess()
        {
            int starsAttained = 1;
            var playTime = stageTimer.ElapsedSeconds;

            // To get a full-three star, finish the game in the given minimum time
            if (playTime <= gsm.SelectedStageMinTime)
                starsAttained = 3;

            // To get 2 - stars, finish within the given minimum time
            else if (playTime > gsm.SelectedStageMinTime && playTime <= gsm.SelectedStageMaxTime)
                starsAttained = 2;
            
            // The trophy color is determined by the difficulty level
            var trophyLevel  = gsm.SelectedDifficulty;
            var isFinalStage = gsm.SelectedDifficulty == LevelDifficulties.Hard &&
                               gsm.SelectedStageNumber >= TotalHardStages;

            // Evaluate the game results, Should we award the trophy?
            var userData = EvaluateGameResult(starsAttained, out bool shouldGiveTrophy);

            if (shouldGiveTrophy)
            {
                GiveTrophy(trophyLevel, userData);
                StartCoroutine(SaveProgress(userData));
                return;
            }

            // Or show the gameover screen instead?
            StartCoroutine(SaveProgress(userData, (outUserData) => {

                gameOverScreenOverlay.SetActive(true);
                GameOverScreenNotifier.NotifyObserver(new GameOverEventArgs
                {
                    GameOverType    = GameOverTypes.Success,
                    TotalStars      = starsAttained,
                    TotalPlayTime   = stageTimer.ElapsedSeconds,
                    TotalReward     = stageFactory.CreatedStage.TotalReward,
                    RewardType      = stageFactory.CreatedStage.RewardType,

                    TotalPlayerCoinBalance = outUserData.TotalCoins,
                    TotalPlayerGemBalance  = outUserData.TotalGems,

                    DisableNextButton      = isFinalStage
                });
            }));
        }

        private void OnGameFailed()
        {
            gameOverScreenOverlay.SetActive(true);
            GameOverScreenNotifier.NotifyObserver(new GameOverEventArgs { 
                GameOverType  = GameOverTypes.Fail,
                TotalPlayTime = stageTimer.ElapsedSeconds,

                TotalPlayerCoinBalance = gsm.UserSessionData.TotalCoins,
                TotalPlayerGemBalance  = gsm.UserSessionData.TotalGems
            });
        }

        private void BreakExecution()
        {
            currentState = GameManagerStates.Stopped;
            stageTimer.Stop();
            bgm.Stop();
        }

        private IEnumerator SaveProgress(UserData userDataIn, Action<UserData> callback = null)
        {
            yield return UserDataHelper.Instance.SaveUserData(userDataIn, (userDataOut) => {
                
                gsm.UserSessionData = userDataOut;
                callback?.Invoke(userDataOut);
            });
        }

        private UserData EvaluateGameResult(int stars, out bool shouldGiveTrophy)
        {
            shouldGiveTrophy = default;
            var userData     = gsm.UserSessionData;
            var stageIndex   = gsm.SelectedStageNumber - 1;

            StageProgress stageProgress;

            switch (gsm.SelectedDifficulty)
            {
                case LevelDifficulties.Easy:
                    stageProgress = userData.StageProgressEasy[stageIndex];

                    // We only update the stars of the finished stage only if it is
                    // higher than the player's previously attained star
                    if (stars > stageProgress.StarsAttained)
                    {
                        stageProgress.StarsAttained = stars;
                        userData.StageProgressEasy[stageIndex] = stageProgress;
                    }

                    // If the selected stage number is exactly or is higher than Next Stage (Highest Stage),
                    // but is within the total stages, we increment the Next Stage (Highest Stage).
                    // This should effectively unlock the next stage.
                    if (gsm.SelectedStageNumber >= userData.HighestEasyStage
                       && userData.HighestEasyStage < TotalEasyStages)
                    {
                        // Initially, the highest stage is "0".
                        // We set it to "1" after completing the 1st stage,
                        // so that it increments to unlock the next (2)
                        if (userData.HighestEasyStage == 0)
                            userData.HighestEasyStage = 1;
                            
                        userData.HighestEasyStage++;
                    }
                    
                    // If the current selected stage was the ending stage, we unlock the "Normal Level"
                    else if (gsm.SelectedStageNumber == TotalEasyStages)
                    {
                        userData.NormalStageUnlocked = true;
                        gsm.SelectedDifficulty  = LevelDifficulties.Normal;
                        gsm.SelectedStageNumber = 0; // reset
                    }

                    shouldGiveTrophy = CheckGiveTrophy(LevelDifficulties.Easy, userData);
                    
                    break;

                case LevelDifficulties.Normal:
                    stageProgress = userData.StageProgressNormal[stageIndex];

                    if (stars > stageProgress.StarsAttained)
                    {
                        stageProgress.StarsAttained = stars;
                        userData.StageProgressNormal[stageIndex] = stageProgress;
                    }

                    if (gsm.SelectedStageNumber >= userData.HighestNormalStage
                       && userData.HighestNormalStage < TotalNormalStages)
                    {
                        if (userData.HighestNormalStage == 0)
                            userData.HighestNormalStage = 1;

                        userData.HighestNormalStage++;
                        //Debug.Log("Go NEXT stage");
                    }

                    else if (gsm.SelectedStageNumber == TotalNormalStages)
                    {
                        userData.HardStageUnlocked = true;
                        gsm.SelectedDifficulty  = LevelDifficulties.Hard;
                        gsm.SelectedStageNumber = 0; // reset
                        //Debug.Log($"Go NEXT LEVEL -> {gsm.SelectedDifficulty}-{gsm.SelectedStageNumber}");
                    }

                    shouldGiveTrophy = CheckGiveTrophy(LevelDifficulties.Normal, userData);

                    break;

                case LevelDifficulties.Hard:
                    stageProgress = userData.StageProgressHard[stageIndex];

                    if (stars > stageProgress.StarsAttained)
                    {
                        stageProgress.StarsAttained = stars;
                        userData.StageProgressHard[stageIndex] = stageProgress;
                    }

                    if (gsm.SelectedStageNumber >= userData.HighestHardStage
                       && userData.HighestHardStage < TotalHardStages)
                    {
                        if (userData.HighestHardStage == 0)
                            userData.HighestHardStage = 1;

                        userData.HighestHardStage++;
                    }
                    
                    // If the current selected stage was the ending stage, we unlock the "Normal Level"
                    // else if (gsm.SelectedStageNumber == TotalEasyStages)
                    // {
                    //     userData.NormalStageUnlocked = true;
                    //     gsm.SelectedDifficulty  = LevelDifficulties.Normal;
                    //     gsm.SelectedStageNumber = 0; // reset
                    // }

                    shouldGiveTrophy = CheckGiveTrophy(LevelDifficulties.Hard, userData);
                    break;
            }

            var totalReward = stageFactory.CreatedStage.TotalReward;
            var rewardType  = stageFactory.CreatedStage.RewardType;
            
            switch (rewardType)
            {
                case RewardTypes.Coins: 
                    userData.TotalCoins += totalReward;
                    break;

                case RewardTypes.Gems:
                    userData.TotalGems  += totalReward;
                    break;
            }
            
            return userData;
        }

        /// <summary>
        /// Only show the trophy for when all stages of a level were completed with full stars
        /// </summary>
        private bool CheckGiveTrophy(LevelDifficulties difficulty, UserData userData)
        {
            var starsAttained = 0;
            var starsRequired = 0;
            //var userData = gsm.UserSessionData;

            // Collect all the stars earned per stage
            switch (difficulty)
            {
                case LevelDifficulties.Easy:
                    
                    userData.StageProgressEasy.ForEach(progress => starsAttained += progress.StarsAttained);
                    starsRequired = maxStarsPerStage * TotalEasyStages;

                    Debug.Log($"starsAttained: {starsAttained} >= starsRequired: {starsRequired} = {starsAttained >= starsRequired}");
                    Debug.Log($"EasyStagesCompleted -> {userData.EasyStagesCompleted}");
                    Debug.Log($"{starsAttained >= starsRequired && !userData.EasyStagesCompleted}");

                    return starsAttained >= starsRequired && !userData.EasyStagesCompleted;

                case LevelDifficulties.Normal:

                    userData.StageProgressNormal.ForEach(progress => starsAttained += progress.StarsAttained);
                    starsRequired = maxStarsPerStage * TotalNormalStages;
                    return starsAttained >= starsRequired && !userData.NormalStagesCompleted;

                case LevelDifficulties.Hard:

                    userData.StageProgressHard.ForEach(progress => starsAttained += progress.StarsAttained);
                    starsRequired = maxStarsPerStage * TotalHardStages;
                    return starsAttained >= starsRequired && !userData.HardStagesCompleted;
            }
            
            return false; //starsAttained >= starsRequired;
        }

        private void GiveTrophy(LevelDifficulties trophyLevel, UserData userData)
        {
            GameObject trophy = null;
            var maxLevels     = 0;
            var rewardCoins   = 0;
            var rewardGems    = 0;

            switch(trophyLevel)
            {
                case LevelDifficulties.Easy: 
                    trophy      = bronzeTrophyScreen;
                    maxLevels   = TotalEasyStages;
                    rewardCoins = coinsOnBronzeTrophy;
                    rewardGems  = gemsOnBronzeTrophy;

                    userData.EasyStagesCompleted = true;
                    break;

                case LevelDifficulties.Normal: 
                    trophy      = silverTrophyScreen;
                    maxLevels   = TotalNormalStages;
                    rewardCoins = coinsOnSilverTrophy;
                    rewardGems  = gemsOnSilverTrophy;

                    userData.NormalStagesCompleted = true;
                    break;

                case LevelDifficulties.Hard: 
                    trophy      = goldTrophyScreen;
                    maxLevels   = TotalHardStages;
                    rewardCoins = coinsOnGoldTrophy;
                    rewardGems  = gemsOnGoldTrophy;

                    userData.HardStagesCompleted = true;
                    break;
            };

            // Show the trophy reward animation screen
            Instantiate(trophy, mainCanvas).TryGetComponent(out TrophyRewardAnimation anim);
            sfx.PlayOnce(trophyScreenSfx);
            
            var continueButtonAction = new Action(() => {

                // If the completed level is the "Final Level", exit to main menu
                if (gsm.SelectedDifficulty == LevelDifficulties.Hard)
                {
                    Instantiate(fancySceneLoader).TryGetComponent(out FancySceneLoader loader);

                    if (loader != null)
                        loader.LoadScene(Constants.Scenes.MainMenu);
                }
                else
                {
                    // Move to the next stage, then remove the animation
                    MoveNextStage();
                    Destroy(anim.gameObject);
                }
            });
            
            anim.SetParams(trophyLevel, maxLevels, rewardCoins, rewardGems, continueButtonAction);

            userData.TotalCoins += rewardCoins;
            userData.TotalGems  += rewardGems;
        }

        #endregion GAME_OVER
    }
}