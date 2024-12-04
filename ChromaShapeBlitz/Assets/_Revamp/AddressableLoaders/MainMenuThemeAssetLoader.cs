using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
public class MainMenuThemeAssetLoader : MonoBehaviour
{
    private AsyncOperationHandle<MainMenuThemeAsset> handle;
    
    private void Start()
    {
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    private void SceneManager_sceneUnloaded(Scene scene)
    {
        if (scene != null && scene.name.Equals(Constants.Scenes.MainMenu) && handle.IsValid())
        {
            Addressables.Release(handle);
            Debug.Log($"Releasing handles for {SceneManager.GetActiveScene().name}");
        }
    }

    public IEnumerator IELoadThemeAsset(string address, Action<MainMenuThemeAsset> onResult)
    {
        // if (string.IsNullOrEmpty(address))
        //     yield break;

        handle = Addressables.LoadAssetAsync<MainMenuThemeAsset>(address);
        yield return handle;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogWarning("Failed to load themes");
            yield break;
        }

        var loadedTheme = handle.Result;
        onResult?.Invoke(loadedTheme);
    }
}
