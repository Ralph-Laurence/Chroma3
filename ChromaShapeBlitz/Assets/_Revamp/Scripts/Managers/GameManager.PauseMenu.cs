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

        public void Ev_Pause()
        {
            pauseMenuStageTitle.text = $"{gsm.SelectedDifficulty} - {gsm.SelectedStageNumber:D2}";

            bgm.Pause();
            stageTimer.Pause();
            PauseTimeScale();

            pauseMenu.SetActive(true);
            sfx.PlayOnce(pauseSound);
        }
        public void Ev_Resume()
        {
            sfx.PlayOnce(resumeSound);
            ResumeTimeScale();

            pauseMenu.SetActive(false);
            stageTimer.Resume();

            StartCoroutine(ResumeBgm());
        }

        public void Ev_Retry()
        {
            ResumeTimeScale();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Ev_MainMenu()
        {
            ResumeTimeScale();
            Instantiate(mainMenuSceneLoader).TryGetComponent(out FancySceneLoader loader);

            loader.LoadScene(Constants.Scenes.MainMenu);
        }
    }
}