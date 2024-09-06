using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class InventoryController : NavContentPageMenuController
{
    private readonly int LOAD_ACTION_STATUS_COMPLETE = 2;

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
    private Sprite[]                            countIndicatorSubSprites;
    private Dictionary<int, InventoryItemData>  m_ownedPowerups;
    private List<Func<IEnumerator>>             m_loadTasks;

    private GameSessionManager gsm;
    private bool m_IsInitialized;


    void Awake()
    {
        gsm         = GameSessionManager.Instance;
        m_loadTasks = new();
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

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1))
            gsm.SetInventoryPageNeedsReload(true);
    }

    /// <summary>
    /// Everytime the inventory page shows up ...
    /// </summary>
    public override void OnBecameFullyVisible(int pageId)
    {
        if (pageId != PageID)
            return;

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
        if (!hotbar.IsDirty)
            return;

        powerupsIO.UpdateEquippedPowerups(hotbar.GetItemQueue());
        hotbar.CleanUpState();
    }

    #region ASSET_LOADING
    /// <summary>
    /// Load the necessary assets then render the UI
    /// </summary>
    /// <param name="fullReload">Applicable only if performing a full data loading. This includes reading the subsprites.</param>
    private IEnumerator IEBeginLoadData()
    {
        // If we already initialized the inventory page before,
        // but it is not marked for reload, we do nothing.
        if (!gsm.IsInventoryPageNeedsReload && m_IsInitialized)
            yield break;

        ProgressLoaderNotifier.NotifyIndefiniteBar(true);
        
        powerupsIO.Setup(gsm);

        // Required only during first run
        if (!m_IsInitialized)
            m_loadTasks.Add(IELoadSubSprites);

        m_loadTasks.Add(IELoadOwnedPowerups);
        m_loadTasks.Add(FIllInventoryListView);

        if (gsm.IsInventoryPageNeedsReload || !m_IsInitialized)
            m_loadTasks.Add(IERepopulateHotbar);

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
    /// Load Subsprites from Addressables
    /// </summary>
    private IEnumerator IELoadSubSprites()
    {
        yield return StartCoroutine
        (
            powerupsIO.LoadCountIndicatorSubSpritesAsync((sprites) =>
            {
                countIndicatorSubSprites = sprites;
            })
        );
    }

    /// <summary>
    /// Load Powerups from Scriptable Objects
    /// </summary>
    private IEnumerator IELoadOwnedPowerups()
    {
        yield return StartCoroutine
        (
            powerupsIO.LoadOwnedPowerupsAssetsAsync((owned, equipped) =>
            {
                m_ownedPowerups  = owned;
                hotbar.UpdateItemQueue(equipped);
            })
        );        
    }

    /// <summary>
    /// All async loading operations have completed
    /// </summary>
    private IEnumerator FIllInventoryListView()
    {
        // Fill the listview of powerups
        foreach (var kvp in m_ownedPowerups)
        {
            var listItemObj = Instantiate(inventoryItemObj, scrollRectContent);
            listItemObj.TryGetComponent(out InventoryListItem listItem);

            var itemData = kvp.Value;
            var data     = new InventoryItemData
            {
                ID                  = itemData.ID,
                Name                = itemData.Name,
                ItemType            = itemData.ItemType,
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
            yield return null;
        }
    }

    /// <summary>
    /// Refill the hotbar and force reset each slot then assign a new item.
    /// </summary>
    private IEnumerator IERepopulateHotbar()
    {
        yield return StartCoroutine(hotbar.RePopulateHotbar());
    }
    //
    //
    #endregion ASSET_LOADING
}
