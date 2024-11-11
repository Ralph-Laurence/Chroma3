using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class InGamePowerupsMappingAssetLoader : MonoBehaviour
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
        if (scene == null || !scene.name.Equals(Constants.Scenes.GamePlay))
            return;

        if (releaseBehaviour == AssetLoaderReleaseBehaviours.OnSceneUnload)
            Release();
    }

    public IEnumerator Load(Action<Dictionary<int, PowerupEffectData>> result)
    {
        handle = Addressables.LoadAssetAsync<PowerupsAssetGroup>(PowerupsLutAddress);
        yield return handle;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogWarning("Failed to load powerups");
            yield break;
        }

        var powerupAssetsList = handle.Result.Powerups;
        var powerupEffectMap = new Dictionary<int, PowerupEffectData>();

        for (var i = 0; i < powerupAssetsList.Count; i++)
        {
            var data = powerupAssetsList[i];

            powerupEffectMap.Add(data.Id, new PowerupEffectData
            {
                PowerupId = data.Id,
                Category = data.PowerupCategory,
                EffectValue = data.EffectValue
            });

            yield return null;
        }

        result?.Invoke(powerupEffectMap);
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
