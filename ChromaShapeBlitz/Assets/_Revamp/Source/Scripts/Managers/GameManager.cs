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
        public static int TotalHardStages   => 20;

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

            // Evaluate the game results
            var userData = ComputeRubrics(starsAttained);
            
            // Should we show the trophy?
            var shouldGiveTrophy = CheckGiveTrophy(gsm.SelectedDifficulty, userData);

            if (shouldGiveTrophy)
            {
                GiveTrophy(gsm.SelectedDifficulty, userData);
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
                    TotalPlayerGemBalance  = outUserData.TotalGems
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

        private UserData ComputeRubrics(int stars)
        {
            var userData = gsm.UserSessionData;
            var stageIndex = gsm.SelectedStageNumber - 1;

            StageProgress stageProgress;
            switch (gsm.SelectedDifficulty)
            {
                case LevelDifficulties.Easy:
                    stageProgress = userData.StageProgressEasy[stageIndex];

                    if (gsm.SelectedStageNumber >= userData.HighestEasyStage
                       && userData.HighestEasyStage < TotalEasyStages)
                        userData.HighestEasyStage++;

                    if (stars > stageProgress.StarsAttained)
                    {
                        stageProgress.StarsAttained = stars;
                        userData.StageProgressEasy[stageIndex] = stageProgress;
                    }
                    break;

                case LevelDifficulties.Normal:
                    stageProgress = userData.StageProgressNormal[stageIndex];

                    if (gsm.SelectedStageNumber >= userData.HighestNormalStage
                       && userData.HighestNormalStage < TotalNormalStages)
                        userData.HighestNormalStage++;

                    if (stars > stageProgress.StarsAttained)
                    {
                        stageProgress.StarsAttained = stars;
                        userData.StageProgressNormal[stageIndex] = stageProgress;
                    }
                    break;

                case LevelDifficulties.Hard:
                    stageProgress = userData.StageProgressHard[stageIndex];

                    if (gsm.SelectedStageNumber >= userData.HighestHardStage
                       && userData.HighestHardStage < TotalHardStages)
                        userData.HighestHardStage++;

                    if (stars > stageProgress.StarsAttained)
                    {
                        stageProgress.StarsAttained = stars;
                        userData.StageProgressHard[stageIndex] = stageProgress;
                    }
                    break;
            }

            var totalReward = stageFactory.CreatedStage.TotalReward;
            var rewardType  = stageFactory.CreatedStage.RewardType;
            // var args = new PlayerCurrencyEventArgs { Amount = totalReward };
            
            switch (rewardType)
            {
                case RewardTypes.Coins: 
                    userData.TotalCoins += totalReward;
                    // args.Currency = CurrencyType.Coin;
                    break;

                case RewardTypes.Gems:
                    userData.TotalGems  += totalReward;
                    // args.Currency = CurrencyType.Gem;
                    break;
            }
            
            return userData;
            // PlayerCurrencyNotifier.NotifyObserver(args);
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

        private void GiveTrophy(LevelDifficulties level, UserData userData)
        {
            GameObject trophy = null;
            var maxLevels     = 0;
            var rewardCoins   = 0;
            var rewardGems    = 0;

            switch(level)
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

            Instantiate(trophy, mainCanvas).TryGetComponent(out TrophyRewardAnimation anim);
            sfx.PlayOnce(trophyScreenSfx);

            var continueButtonAction = new Action(() => {
                
                MoveNextStage();
                Destroy(anim.gameObject);
            });
            
            anim.SetParams(level, maxLevels, rewardCoins, rewardGems, continueButtonAction);

            userData.TotalCoins += rewardCoins;
            userData.TotalGems += rewardGems;
        }

        #endregion GAME_OVER
    }
}