using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class InventoryController : NavContentPageMenuController, IShopController
{

    #region INVENTORY_UI
    //
    //
    [SerializeField] private InventoryHotBar    hotbar;
    [SerializeField] private RectTransform      scrollRectContent;
    [SerializeField] private GameObject         inventoryItemObj;
    //
    //
    #endregion INVENTORY_UI

    [SerializeField] private PowerupsIO         powerupsIO;
    [SerializeField] private Sprite[]           countIndicatorSubSprites;
    private List<Func<IEnumerator>>             m_loadTasks;

    private Dictionary<int, InventoryItemData> ownedPowerupsLookup;
    private Dictionary<int, InventoryItemData> activePowerupsLookup;

    private GameSessionManager gsm;
    private bool m_IsInitialized;

    [SerializeField] private InventoryPowerupsMappingAssetLoader powerupsMappingAssetLoader;

    void Awake()
    {
        gsm                  = GameSessionManager.Instance;
        m_loadTasks          = new();
        InventoryItemCards   = new();
        ownedPowerupsLookup  = new();
        activePowerupsLookup = new();
    }

    void OnEnable()
    {
        InventoryListItemSelectedNotifier.BindObserver(ObserveInventoryListItem);
        HotbarSlotSelectedNotifier.BindObserver(ObserveHotbarSlotSelected);
    }

    void OnDisable()
    {
        InventoryListItemSelectedNotifier.UnbindObserver(ObserveInventoryListItem);
        HotbarSlotSelectedNotifier.UnbindObserver(ObserveHotbarSlotSelected);

        HandleInventoryClosing();
    }

    /// <summary>
    /// Everytime the inventory page shows up ...
    /// </summary>
    public override void OnBecameFullyVisible(int pageId)
    {
        Debug.Log("Inventory became visible again.");
        if (pageId != PageID)
            return;
        Debug.Log("Begin load data.");
        StartCoroutine(IEBeginLoadData());
    }

    /// <summary>
    /// When an inventory list item has been selected, enqueue it
    /// </summary>
    /// <param name="sender"></param>
    private void ObserveInventoryListItem(InventoryListItem sender)
    {
        var data = sender.GetItemData();

        if (data.Equals(default(InventoryItemData)))
        {
            return;
        }

        // We can only equip one item of the same type
        foreach (var kvp in hotbar.GetItemQueue())
        {
            var item = kvp.Value;

            if (item.ItemCategory == data.ItemCategory)
            {
                Debug.Log("Can't enqueue an item of the same type");
                return;
            }
        }

        hotbar.EnqueueItem(data);
    }

    private void ObserveHotbarSlotSelected(int slotIndex) => hotbar.DequeueItem(slotIndex);

    /// <summary>
    /// Whenever the inventory UI closes, we perform a hotbar queue check.
    /// Whenever a hotbar is dirtied, we must verify the order of its items.
    /// If they are the same, we do not need to update the internal data.
    /// Otherwise, update the serialized data.
    /// </summary>
    private void HandleInventoryClosing()
    {
        if (!hotbar.IsDirty || powerupsIO == null)
            return;

        powerupsIO.UpdateEquippedPowerups(hotbar.GetItemQueue(), onComplete: () =>
        {
            gsm.SetInventoryPageNeedsReload(true);
            hotbar.CleanUpState();
        });
        //WriteSerializedHotbarItems();
    }

    //public void WriteSerializedHotbarItems()
    //{
    //    powerupsIO.UpdateEquippedPowerups(hotbar.GetItemQueue(), onComplete: () =>
    //    {
    //        gsm.SetInventoryPageNeedsReload(true);
    //        hotbar.CleanUpState();
    //    });
    //}

    #region ASSET_LOADING
    /// <summary>
    /// Load the necessary assets then render the UI
    /// </summary>
    /// <param name="fullReload">Applicable only if performing a full data loading. This includes reading the subsprites.</param>
    private IEnumerator IEBeginLoadData()
    {
        if (m_IsInitialized)
        {
            // If we already initialized the inventory page before,
            // but it is not marked for reload, we do nothing.
            if (!gsm.IsInventoryPageNeedsReload)
                yield break;

            // However, if this page is already initialized but it needs
            // to be redrawn, we reset its contents for redraw
            while (scrollRectContent.childCount > 0)
            {
                DestroyImmediate(scrollRectContent.GetChild(0).gameObject);
                yield return null;
            }
        }

        ProgressLoaderNotifier.NotifyIndefiniteBar(true);

        m_loadTasks.Add(IELoadOwnedPowerups);
        m_loadTasks.Add(FIllInventoryListView);

        if (gsm.IsInventoryPageNeedsReload || !m_IsInitialized)
            m_loadTasks.Add(hotbar.RePopulateHotbar);

        for (var i = 0; i < m_loadTasks.Count; i++)
        {
            var coroutine = m_loadTasks[i];

            // yield return ensures that each coroutine fully completes before moving on to the next one. 
            // This is different from running them concurrently (in parallel).
            yield return StartCoroutine(coroutine());
        }

        m_loadTasks.Clear();

        if (!m_IsInitialized)
            m_IsInitialized = true;
        
        ProgressLoaderNotifier.NotifyIndefiniteBar(false);

        if (gsm.IsInventoryPageNeedsReload)
            gsm.SetInventoryPageNeedsReload(false);
    }

    /// <summary>
    /// Load Powerups from Scriptable Objects
    /// </summary>
    private IEnumerator IELoadOwnedPowerups()
    {
        //===============================================//
        //== LOAD POWERUPS FROM THE SCRIPTABLE OBJECTS ==//
        //===============================================//

        var powerupsMap = new List<PowerupsAsset>();

        yield return powerupsMappingAssetLoader.Load( (result) => powerupsMap = result);

        //===================================================//
        //== LOAD OWNED POWERUPS FROM SERIALIZED INVENTORY ==//
        //===================================================//
        
        ownedPowerupsLookup.Clear();
        activePowerupsLookup.Clear();

        var playerInventory             = gsm.UserSessionData.Inventory;
        var ownedPowerupsInventory      = playerInventory.OwnedPowerups;
        var equippedPowerupsInventory   = playerInventory.EquippedPowerupIds;

        // No owned powerups yet ... skip.
        if (ownedPowerupsInventory == null ||
            ownedPowerupsInventory?.Count < 1)
        {
            powerupsMappingAssetLoader.Release();
            yield break;
        }
        //
        // Collect initial powerups data such as the current amount
        //
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
        //
        // Retrieve powerup data such as Name and Image
        //
        for (var i = 0; i < powerupsMap.Count; i++)
        {
            // PowerupAsset item
            var powerup = powerupsMap[i];

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

        Debug.LogWarning($"Equipped Items size on disk: {activePowerupsLookup.Count}");
        hotbar.SetItemQueueSource(activePowerupsLookup);
        //yield return StartCoroutine
        //(
        //    powerupsIO.LoadOwnedPowerupsAssetsAsync((owned, equipped) =>
        //    {
        //        Debug.LogWarning($"Equipped Items size on disk: {equipped.Count}");
        //        m_ownedPowerups  = owned;
        //        hotbar.SetItemQueueSource(equipped);
        //    })
        //);        
    }

    /// <summary>
    /// All async loading operations have completed
    /// </summary>
    private IEnumerator FIllInventoryListView()
    {
        if (ownedPowerupsLookup == null || ownedPowerupsLookup?.Count < 1)
            yield break;

        // Fill the listview of powerups
        foreach (var kvp in ownedPowerupsLookup)
        {
            var listItemObj = Instantiate(inventoryItemObj, scrollRectContent);
            listItemObj.TryGetComponent(out InventoryListItem listItem);

            var itemData = kvp.Value;
            var data     = new InventoryItemData
            {
                ID                  = itemData.ID,
                Name                = itemData.Name,
                ItemType            = itemData.ItemType,
                ItemCategory        = itemData.ItemCategory,
                Thumbnail           = itemData.Thumbnail,
                CurrentAmount       = itemData.CurrentAmount,
                AttachedGameObject  = listItemObj,

                AmountIcon = itemData.CurrentAmount.MapAmountToSprite
                (
                    countIndicatorSubSprites,
                    itemData.ItemType
                )
            };

            var itemId = data.ID;

            // Apply the fixed/formatted item data into the original source,
            // such as the updated amount icon. In this case, our original
            // datasource is in the hotbar's item queue
            if (hotbar.ItemQueueContains(itemId))
            {
                data.IsVisible = false;
                data.SetInvisible();

                hotbar.UpdateItemValue(itemId, data);
            }

            listItem.SetData(data);

            // This is used mainly for tutorial.
            // Otherwise it has no other uses in this context
            InventoryItemCards.Add(listItem);

            yield return null;
        }
    }

    #endregion ASSET_LOADING
    //
    //--------------------------------------------------
    //       USED MOSTLY DURING TUTORIAL SESSIONS
    //--------------------------------------------------
    //
    public void OnBecameFullyVisible()
    {
        ShopMenuPageCommonNotifier.NotifyObserver(ShopMenuPageEventNames.InventoryPageFullyShown);
    }

    public List<InventoryListItem> InventoryItemCards { get; private set; }
}