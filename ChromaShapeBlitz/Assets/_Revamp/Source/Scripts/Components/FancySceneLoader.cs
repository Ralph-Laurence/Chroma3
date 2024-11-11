using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FancySceneLoader : MonoBehaviour
{
    private List<Func<IEnumerator>> tasks = new();

    private string targetScene;

    /// <summary>
    /// <b>Load a scene by name.</b>
    /// <para>
    /// By default, this executes all tasks first then loads the target scene.<br/>
    /// When <paramref name="immediate"/> is TRUE, it immediately loads the scene
    /// without executing any tasks.
    ///
    /// </para>
    /// </summary>
    public void LoadScene(string sceneName, bool immediate = true)
    {
        targetScene = sceneName;

        if (immediate)
        {
            LoadSceneImmediate(targetScene);
            return;
        }

        var loadScene = new Action(() => StartCoroutine(LoadSceneAsync()));

        if (tasks == null || tasks.Count <= 0)
        {
            loadScene.Invoke();
            return;
        }

        StartCoroutine(ProcessTasks(whenDone: loadScene));
    }

    public void AddTask(Func<IEnumerator> task) => tasks.Add(task);

    /// <summary>
    /// Just load the scene
    /// </summary>
    private void LoadSceneImmediate(string sceneName)
    {
        targetScene = sceneName;
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator ProcessTasks(Action whenDone = null)
    {
        for (var i = 0; i < tasks.Count; i++)
        {
            var coroutine = tasks[i];
            yield return StartCoroutine(coroutine());
        }

        whenDone?.Invoke();
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
