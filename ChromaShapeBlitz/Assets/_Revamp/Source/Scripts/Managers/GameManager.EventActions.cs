using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Revamp
{
    public partial class GameManager : MonoBehaviour
    {
        private IEnumerator ResumeBgm()
        {
            yield return new WaitForSeconds(0.4F);
            bgm.Resume();
        }
        /// <summary>
        /// Game Manager Action Events are prefixed with "GMAEV_"
        /// </summary>
        private void GMAEV_Pause()
        {
            bgm.Pause();
            PauseTimeScale();

            pauseMenu.SetActive(true);
            sfx.PlayOnce(pauseSound);

            MainCamera.Freeze();
        }

        private void GMAEV_Resume()
        {
            sfx.PlayOnce(resumeSound);
            ResumeTimeScale();

            pauseMenu.SetActive(false);
            MainCamera.UnFreeze();
            StartCoroutine(ResumeBgm());
        }

        private void GMAEV_Retry()
        {
            ResumeTimeScale();
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            stageTimer.Stop();
            stageFactory.Clear();
            stageFactory.Create(gsm.SelectedDifficulty, gsm.SelectedStageNumber);

            MainCamera.ResetView(true);
            pauseMenu.SetActive(false);
        }

        private void GMAEV_ExitToMainMenu()
        {
            gsm.ClearSession();
            stageTimer.Stop();

            ResumeTimeScale();
            Instantiate(mainMenuSceneLoader).TryGetComponent(out FancySceneLoader loader);

            loader.LoadScene(Constants.Scenes.MainMenu);
        }

        private void GMAEV_NextStage()
        {
            var stageNumber = gsm.SelectedStageNumber + 1;
            var difficulty = gsm.SelectedDifficulty;

            gsm.ClearSession();
            
            stageFactory.Clear();
            stageFactory.Create(difficulty, stageNumber);

            gsm.SelectedDifficulty = difficulty;
            gsm.SelectedStageNumber = stageNumber;

            MainCamera.ResetView();
        }
    }
}