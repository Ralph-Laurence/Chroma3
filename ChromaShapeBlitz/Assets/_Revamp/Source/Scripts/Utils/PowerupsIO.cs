using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

public class PowerupsIO : MonoBehaviour
{
    private GameSessionManager gsm;

    void Awake()
    {
        gsm = GameSessionManager.Instance;
    }

    public IEnumerator LoadOwnedPowerupsAssetsAsync(Action
    <
        Dictionary<int, InventoryItemData>, // OUTPUT: OWNED POWERUPS
        Dictionary<int, InventoryItemData>  // OUTPUT: EQUIPPED POWERUPS
    > loadComplete)
    {
        // .............................................
        // # LOAD POWERUPS FROM THE SCRIPTABLE OBJECTS #
        // .............................................

        var powerupsLutAddress = Constants.PackedAssetAddresses.PowerupsLUT;
        var handle = Addressables.LoadAssetAsync<PowerupsAssetGroup>(powerupsLutAddress);

        yield return handle;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogWarning("Failed to load powerups");
            yield break;
        }

        var powerupAssetsList = handle.Result.Powerups;

        // .................................................
        // # LOAD OWNED POWERUPS FROM SERIALIZED INVENTORY #
        // .................................................

        var ownedPowerupsLookup         = new Dictionary<int, InventoryItemData>();
        var activePowerupsLookup        = new Dictionary<int, InventoryItemData>();
        var playerInventory             = gsm.UserSessionData.Inventory;
        var ownedPowerupsInventory      = playerInventory.OwnedPowerups;
        var equippedPowerupsInventory   = playerInventory.EquippedPowerupIds;

        if ( ownedPowerupsInventory == null || 
             ownedPowerupsInventory?.Count < 1)
            yield break;

        // No owned powerups yet ... skip.
        if (ownedPowerupsInventory == null || ownedPowerupsInventory?.Count == 0)
        {
            loadComplete?.Invoke(ownedPowerupsLookup, activePowerupsLookup);
            Addressables.Release(handle);
        }

        // Collect initial powerups data such as the current amount

        for (var i = 0; i < ownedPowerupsInventory.Count; i++)
        {
            var powerup = ownedPowerupsInventory[i];

            ownedPowerupsLookup.Add(powerup.PowerupID, new InventoryItemData
            {
                ID = powerup.PowerupID,
                CurrentAmount = powerup.CurrentAmount
            });

            yield return null;
        }

        // Retrieve powerup data such as Name and Image

        for (var i = 0; i < powerupAssetsList.Count; i++)
        {
            // PowerupAsset item
            var powerup = powerupAssetsList[i];

            // Skip powerups that we do not own
            if (!ownedPowerupsLookup.ContainsKey(powerup.Id))
                continue;

            // Update the owned powerups data, i.e. assign thumbnail
            var ownedPowerupData = ownedPowerupsLookup[powerup.Id];

            ownedPowerupData.Name           = powerup.Name;
            ownedPowerupData.Thumbnail      = powerup.PreviewImage;
            ownedPowerupData.ItemType       = powerup.ItemType.ToInventoryItemType();
            ownedPowerupData.ItemCategory   = powerup.PowerupCategory;
            ownedPowerupData.MaxAmount      = powerup.MaxCount;
            ownedPowerupData.IsVisible      = true;

            ownedPowerupsLookup[powerup.Id] = ownedPowerupData;

            // Collect equipped powerups
            if (
                equippedPowerupsInventory != null &&
                equippedPowerupsInventory.Contains(powerup.Id)
               )
            {
                activePowerupsLookup.Add(powerup.Id, ownedPowerupData);
            }

            yield return null;
        }

        loadComplete?.Invoke(ownedPowerupsLookup, activePowerupsLookup);

        if (handle.IsValid())
            Addressables.Release(handle);
    }

    /// <summary>
    /// Load subsprites in the addressables
    /// </summary>
    //public IEnumerator LoadCountIndicatorSubSpritesAsync(Action<Sprite[]> loadComplete)
    //{
    //    var spriteSheetAddress = Constants.PackedAssetAddresses.CountIndicatorSpriteSheets;
    //    var handle = Addressables.LoadAssetAsync<Sprite[]>(spriteSheetAddress);

    //    yield return handle;

    //    if (handle.Status != AsyncOperationStatus.Succeeded)
    //    {
    //        Debug.LogWarning("Failed to load subsprites");
    //        yield break;
    //    }

    //    // Ensure we only access handle.Result while handle is valid
    //    var sprites = handle.Result;
    //    loadComplete?.Invoke(sprites);

    //    // Now it's safe to release the handle
    //    if (handle.IsValid())
    //        Addressables.Release(handle);

    //    if (sprites == null)
    //    {
    //        Debug.Log("Dem azzettes aint found!");
    //    }
    //}

    /// <summary>
    /// Helper method of the UpdateEquippedPowerups coroutine.
    /// </summary>
    /// <param name="equippedPowerups">The updated powerups</param>
    public void UpdateEquippedPowerups(Dictionary<int, InventoryItemData> equippedPowerups, Action onComplete = null)
    {
        ProgressLoaderNotifier.NotifyFourSegment(true);

        StartCoroutine(IEUpdateEquippedPowerups(equippedPowerups, () => {
            ProgressLoaderNotifier.NotifyFourSegment(false);
            onComplete?.Invoke();
        }));
    }

    /// <summary>
    /// Update the binary serialized data of equipped powerups
    /// </summary>
    private IEnumerator IEUpdateEquippedPowerups(Dictionary<int, InventoryItemData> equippedPowerups, Action onComplete = null)
    {
        var equippedPowerupIds = new List<int>();
        var userData           = gsm.UserSessionData;

        if (equippedPowerups?.Count > 0)
        {
            foreach (var kvp in equippedPowerups)
            {
                equippedPowerupIds.Add(kvp.Value.ID);
                yield return null;
            }
        }

        userData.Inventory.EquippedPowerupIds = equippedPowerupIds;
        gsm.UserSessionData = userData;

        // Write the changes to file
        yield return StartCoroutine(UserDataHelper.Instance.SaveUserData(userData, (d) =>
        {
            onComplete?.Invoke();   
        }));
    }
}