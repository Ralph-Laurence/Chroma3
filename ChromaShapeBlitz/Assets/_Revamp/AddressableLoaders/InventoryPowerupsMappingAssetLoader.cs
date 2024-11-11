using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
using UnityEngine.SceneManagement;

public class InventoryPowerupsMappingAssetLoader : MonoBehaviour
{
    [SerializeField] private AssetLoaderReleaseBehaviours releaseBehaviour;

    private readonly string PowerupsLutAddress = Constants.PackedAssetAddresses.PowerupsLUT;
    private AsyncOperationHandle<PowerupsAssetGroup> handle;

    private void Start()
    {
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    private void SceneManager_sceneUnloaded(Scene scene)
    {
        if (scene == null || !scene.name.Equals(Constants.Scenes.MainMenu))
            return;

        if (releaseBehaviour == AssetLoaderReleaseBehaviours.OnSceneUnload)
            Release();
    }

    public IEnumerator Load(Action<List<PowerupsAsset>> result)
    {
        // .............................................
        // # LOAD POWERUPS FROM THE SCRIPTABLE OBJECTS #
        // .............................................
        handle = Addressables.LoadAssetAsync<PowerupsAssetGroup>(PowerupsLutAddress);
        yield return handle;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogWarning("Failed to load powerups");
            yield break;
        }

        var powerupAssetsList = handle.Result.Powerups;
        result?.Invoke(powerupAssetsList);
    }

    public void Release()
    {
        if (handle.IsValid())
        {
            Addressables.Release(handle);
            Debug.Log($"Releasing handles for {SceneManager.GetActiveScene().name}");
        }
    }
}
