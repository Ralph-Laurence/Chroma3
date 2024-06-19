using UnityEngine;

namespace Revamp
{
    public enum StageCompletionType
    {
        Success,
        Failed
    }

    public partial class GameManager : MonoBehaviour
    {
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
            gsm = GameSessionManager.Instance;
            bgm = BackgroundMusic.Instance;
            sfx = SoundEffects.Instance;

            Debug.Log(gsm.SelectedStageNumber);
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

        private void OnGameSuccess()
        {
            Debug.LogWarning("Success!");
        }

        private void OnGameFailed()
        {
            Debug.LogWarning("Failed!");
        }
        
        #endregion GAME_MECHANICS
    }
}