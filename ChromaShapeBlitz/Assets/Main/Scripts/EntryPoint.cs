using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntryPoint : MonoBehaviour
{
    private void Awake() => StartCoroutine(Begin());

    //========================================
    // LOAD and CACHE the BINARY DATA then 
    // launch the main menu after
    //========================================
    private IEnumerator Begin()
    {
        yield return StartCoroutine(LevelStructuresHelper.Instance.LoadLevelStructures());
        //yield return StartCoroutine(CustomizationsHelper.Instance.LoadCustomizations());
        yield return StartCoroutine(CustomBlockSkinsHelper.Instance.LoadCustomSkins());
        yield return StartCoroutine(PlayerProgressHelper.Instance.LoadPlayerData());

        yield return StartCoroutine(LoadMainMenuAsync());

        yield return null;
    }

    private IEnumerator LoadMainMenuAsync()
    {
        var load = SceneManager.LoadSceneAsync(Constants.Scenes.MainMenu); //("rv_cust"); 
        load.allowSceneActivation = false;

        // Wait until the scene is fully loaded
        while (load.progress < 0.9f)
        {
            yield return null;
        }

        // The scene has been fully loaded. Activate it.
        load.allowSceneActivation = true;

        while (!load.isDone)
        {
            yield return null;
        }
    } 
}