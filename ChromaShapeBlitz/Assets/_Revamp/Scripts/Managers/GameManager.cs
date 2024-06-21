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
        public static int TotalEasyStages   => 50;
        public static int TotalNormalStages => 30;
        public static int TotalHardStages   => 25;

        public static int GetTotalStages(LevelDifficulties difficulty)
        {
            switch (difficulty)
            {
                case LevelDifficulties.Easy:
                    return TotalEasyStages;

                case LevelDifficulties.Normal:
                    return TotalNormalStages;

                case LevelDifficulties.Hard:
                    return TotalHardStages;

                default:
                    return 0;
            }
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

            gameOverScreenOverlay.SetActive(true);
            GameOverScreenNotifier.NotifyObserver(new GameOverEventArgs
            {
                GameOverType  = GameOverTypes.Success,
                TotalStars    = stars,
                TotalPlayTime = stageTimer.ElapsedSeconds
            });
        }

        private void OnGameFailed()
        {
            gameOverScreenOverlay.SetActive(true);
            GameOverScreenNotifier.NotifyObserver(new GameOverEventArgs { 
                GameOverType  = GameOverTypes.Fail,
                TotalPlayTime = stageTimer.ElapsedSeconds
            });
        }

        private void BreakExecution()
        {
            currentState = GameManagerStates.Stopped;
            stageTimer.Stop();
            bgm.Stop();
        }

        #endregion GAME_OVER
    }
}