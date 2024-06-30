using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FancySceneLoader : MonoBehaviour
{
    //[SerializeField] private Slider progressBar;
    private string targetScene;

    public void LoadScene(string sceneName)
    {
        targetScene = sceneName;
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        // Start loading the scene
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(targetScene);

        // Prevent the scene from activating until it is fully loaded
        asyncOperation.allowSceneActivation = false;

        // While the asynchronous scene loads, update the progress bar
        while (!asyncOperation.isDone)
        {
            // Update the progress bar
            //progressBar.value = asyncOperation.progress;

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                // Loading is done, activate the scene
                yield return new WaitForSeconds(2.0F);
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
            
    }
}
