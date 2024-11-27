using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Revamp
{
    public partial class GameManager : MonoBehaviour
    {
        private readonly int FLAG_VISIT_SHOP = 1;

        private IEnumerator ResumeBgm()
        {
            yield return new WaitForSeconds(0.4F);
            bgm.Resume();
        }
        /// <summary>
        /// Game Manager Action Events are prefixed with "GMAEV_"
        /// </summary>
        private void GMEV_Pause()
        {
            bgm.Pause();
            PauseTimeScale();

            pauseMenu.SetActive(true);
            sfx.PlayOnce(pauseSound);

            MainCamera.Freeze();
        }

        private void GMEV_Resume()
        {
            sfx.PlayOnce(resumeSound);
            ResumeTimeScale();

            pauseMenu.SetActive(false);
            MainCamera.UnFreeze();
            StartCoroutine(ResumeBgm());
        }

        private void GMEV_Retry()
        {
            ResumeTimeScale();
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            stageTimer.Stop();
            stageFactory.Clear();
            stageFactory.Create(gsm.SelectedDifficulty, gsm.SelectedStageNumber);

            MainCamera.ResetView(true);
            pauseMenu.SetActive(false);
        }

        private void GMEV_ExitToMainMenu() => ExitToMainMenu();

        private void GMEV_VisitShop() => ExitToMainMenu(FLAG_VISIT_SHOP);

        private void GMAEV_NextStage() => MoveNextStage();

        private void ExitToMainMenu(int flag = 0)
        {
            gsm.ClearSession();
            stageTimer.Stop();

            ResumeTimeScale();
            Instantiate(mainMenuSceneLoader).TryGetComponent(out FancySceneLoader loader);

            if (flag == FLAG_VISIT_SHOP)
                gsm.IsVisitShopOnGameOver = true;

            loader.LoadScene(Constants.Scenes.MainMenu);
        }

        public void MoveNextStage()
        {
            var stageNumber = gsm.SelectedStageNumber + 1;
            var difficulty  = gsm.SelectedDifficulty;

            gsm.ClearSession();
            
            // Decide which stage to spawn next.
            switch (difficulty)
            {
                // After completing the Easy stages, move on to Normal
                case LevelDifficulties.Easy:

                    // If the current selected stage was the ending stage...
                    if (stageNumber > TotalEasyStages)
                    {
                        difficulty  = LevelDifficulties.Normal;
                        stageNumber = 1;
                    }
                    
                    break;

                case LevelDifficulties.Normal:
                    
                    // After completing the Normal stages, move on to Hard
                    if (stageNumber > TotalNormalStages)
                    {
                        difficulty  = LevelDifficulties.Hard;
                        stageNumber = 1;
                    }
                    break;
                
                case LevelDifficulties.Hard:
                    // stageNumber = Mathf.Clamp(stageNumber, 1, TotalHardStages);
                    break;
            }
            // Debug.Log($"Load Next stage : {stageNumber}; Lvl : {difficulty}");
            stageFactory.Clear();
            stageFactory.Create(difficulty, stageNumber);

            gsm.SelectedDifficulty  = difficulty;
            gsm.SelectedStageNumber = stageNumber;

            MainCamera.ResetView();
        }

        private void ProceedNextStage()
        {

        }
    }
}