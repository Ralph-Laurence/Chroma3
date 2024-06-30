using System;
using System.Collections;
using UnityEngine;

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

        void OnEnable() => AttachEventObservers();
        
        void OnDisable() => DetachEventObservers();
        
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
            int stars;
            var playTime = stageTimer.ElapsedSeconds;

            // To get a full-three star, finish the game in the given minimum time
            if (playTime <= gsm.SelectedStageMinTime)
                stars = 3;

            // To get 2 - stars, finish within the given minimum time
            else if (playTime > gsm.SelectedStageMinTime && playTime <= gsm.SelectedStageMaxTime)
                stars = 2;

            else
                stars = 1;

            StartCoroutine(SaveProgress(stars, stageFactory.CreatedStage, (userData) => {

                gameOverScreenOverlay.SetActive(true);
                GameOverScreenNotifier.NotifyObserver(new GameOverEventArgs
                {
                    GameOverType    = GameOverTypes.Success,
                    TotalStars      = stars,
                    TotalPlayTime   = stageTimer.ElapsedSeconds,
                    TotalReward     = stageFactory.CreatedStage.TotalReward,
                    RewardType      = stageFactory.CreatedStage.RewardType,

                    TotalPlayerCoinBalance = userData.TotalCoins,
                    TotalPlayerGemBalance  = userData.TotalGems
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

        private IEnumerator SaveProgress(int stars, StageVariant currentStage, Action<UserData> callback)
        {
            var userData = gsm.UserSessionData;
            StageProgress stageProgress = default;
            var stageIndex = gsm.SelectedStageNumber-1;

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
            var args = new PlayerCurrencyEventArgs { Amount = totalReward };
            
            switch (rewardType)
            {
                case RewardTypes.Coins: 
                    userData.TotalCoins += totalReward;
                    args.Currency = CurrencyType.Coin;
                    break;

                case RewardTypes.Gems:
                    userData.TotalGems  += totalReward;
                    args.Currency = CurrencyType.Gem;
                    break;
            }
            
            PlayerCurrencyNotifier.NotifyObserver(args);
            gsm.UserSessionData = userData;

            yield return UserDataHelper.Instance.SaveUserData(userData, (u) => callback?.Invoke(u));
        }

        #endregion GAME_OVER
    }
}