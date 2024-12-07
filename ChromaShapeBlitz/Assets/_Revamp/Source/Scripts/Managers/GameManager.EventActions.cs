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

        // Mostly used by "Next" button
        public void MoveNextStage()
        {
            // Get the next stage properties then Create the next stage
            // PreComputeNextStage(gsm, out int stageNumber, out LevelDifficulties difficulty);
            var nextStage  = gsm.SelectedStageNumber + 1;
            var difficulty = gsm.SelectedDifficulty;

            gsm.ClearSession();

            InstantiateStage(nextStage, difficulty);
        }

        private void OnTrophyMoveNextStage()
        {
            var currentStageNumber = gsm.SelectedStageNumber;
            var difficulty         = gsm.SelectedDifficulty;

            gsm.ClearSession();

            switch (difficulty)
            {
                case LevelDifficulties.Easy:

                    // If the current selected stage was the ending stage...
                    if (currentStageNumber >= TotalEasyStages)
                    {
                        difficulty  = LevelDifficulties.Normal;
                        currentStageNumber = 1;
                    }
                    
                    break;

                case LevelDifficulties.Normal:
                    
                    // After completing the Normal stages, move on to Hard
                    if (currentStageNumber > TotalNormalStages)
                    {
                        difficulty  = LevelDifficulties.Hard;
                        currentStageNumber = 1;
                    }
                    break;
                
                case LevelDifficulties.Hard:
                    // stageNumber = Mathf.Clamp(stageNumber, 1, TotalHardStages);
                    break;
            }
        }
        /// <summary>
        /// Spawn a specific stage by its given stagenumber and difficulty
        /// </summary>
        private void InstantiateStage(int stageNumber, LevelDifficulties difficulty)
        {
            stageFactory.Clear();
            stageFactory.Create(difficulty, stageNumber);

            gsm.SelectedDifficulty  = difficulty;
            gsm.SelectedStageNumber = stageNumber;

            MainCamera.ResetView();
        }
        
        /// <summary>
        /// Decide which stage to spawn next. We will increment the current
        /// stage's number and its difficulty, so that we can determine which
        /// stage to create next
        /// </summary>
        private void PreComputeNextStagex(GameSessionManager gsm, out int currentStageNumber, out LevelDifficulties difficulty)
        { 
            currentStageNumber = gsm.SelectedStageNumber + 1;
            difficulty         = gsm.SelectedDifficulty;

            gsm.ClearSession();
            
            switch (difficulty)
            {
                // After completing the Easy stages, move on to Normal
                case LevelDifficulties.Easy:

                    // If the current selected stage was the ending stage...
                    if (currentStageNumber > TotalEasyStages)
                    {
                        difficulty  = LevelDifficulties.Normal;
                        currentStageNumber = 1;
                    }
                    
                    break;

                case LevelDifficulties.Normal:
                    
                    // After completing the Normal stages, move on to Hard
                    if (currentStageNumber > TotalNormalStages)
                    {
                        difficulty  = LevelDifficulties.Hard;
                        currentStageNumber = 1;
                    }
                    break;
                
                case LevelDifficulties.Hard:
                    // stageNumber = Mathf.Clamp(stageNumber, 1, TotalHardStages);
                    break;
            }
        }
    }
}