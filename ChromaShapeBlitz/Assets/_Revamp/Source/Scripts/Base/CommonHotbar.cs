using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The HotBar is a subset of the Inventory, showing only the items that are currently equipped 
/// or quickly accessible during gameplay. It's a more focused view of the player's inventory, tailored for immediate use.
/// However, this can also be used in the Inventory page.
/// </summary>
public abstract class CommonHotbar : MonoBehaviour
{
    /// <summary>
    /// Invoked when the internal Awake() has finished
    /// </summary>
    protected abstract void OnAwake();

    [SerializeField] protected HotBarSlot[] dynamicSlots;
    [SerializeField] protected AudioClip enqueueSfx;
    [SerializeField] protected AudioClip dequeueSfx;

    private Dictionary<int, InventoryItemData> itemQueue = new();

    private bool isDirty = false;

    void Awake()
    {
        AssignSlotIndeces();
        OnAwake();
    }

    private void AssignSlotIndeces()
    {
        // Assign a unique index to each hotbar slot
        for (var i = 0; i < dynamicSlots.Length; i++)
        {
            var slot = dynamicSlots[i];
            slot.SlotIndex = i;
        }
    }

    /// <summary>
    /// Add an item to the slot
    /// </summary>
    public virtual void EnqueueItem(InventoryItemData itemData)
    {
        var itemQueue = GetItemQueue();

        if (itemQueue.ContainsKey(itemData.ID) || itemQueue.Count >= dynamicSlots.Length)
            return;

        itemQueue.Add(itemData.ID, itemData);
        
        // Adding an item makes the queue dirty
        isDirty = true;
        
        itemData.SetInvisible();

        // Find which slot is unoccupied
        for (var i = 0; i < dynamicSlots.Length; i++)
        {
            var slot = dynamicSlots[i];

            if (!slot.IsSlotFilled)
            {
                slot.FillItem(new HotbarSlotDataSource
                {
                    ItemId = itemData.ID,
                    ItemCountSprite = itemData.AmountIcon,
                    ItemThumbnail = itemData.Thumbnail,
                });

                break;
            }
        }
    }

    /// <summary>
    /// Remove an item from the slot, by its index
    /// </summary>
    public virtual void DequeueItem(int slotIndex)
    {
        var slot        = dynamicSlots[slotIndex];
        var itemQueue   = GetItemQueue();

        // If an item does not exist from the queue, skip..
        if (!itemQueue.ContainsKey(slot.GetItemID()))
            return;

        // Assume that the item exists in the queue, then cache a reference to it
        var itemData = itemQueue[slot.GetItemID()];
        itemData.SetVisible();

        // Remove it from the queue
        itemQueue.Remove(itemData.ID);

        // Removing an item makes the queue dirty
        isDirty = true;

        // Redraw the hotbar at given index
        dynamicSlots[slotIndex].Reset();
    }
    
    /// <summary>
    /// Reset the dirty state
    /// </summary>
    public void CleanUpState() => isDirty = false;

    /// <summary>
    /// Get the dirty state
    /// </summary>
    public bool IsDirty   => isDirty;
    
    /// <summary>
    /// Get the Ids of the queued items
    /// </summary>
    public List<int> QueuedItemIds => itemQueue.Keys.ToList();

    /// <summary>
    /// Give the hotbar's item queue with newer values
    /// </summary>
    public void SetItemQueueSource(Dictionary<int, InventoryItemData> itemQueue) => this.itemQueue = itemQueue;
    
    /// <summary>
    /// THe internal itemsource datasource of hotbar
    /// </summary>
    public Dictionary<int, InventoryItemData> GetItemQueue() => itemQueue;

    /// <summary>
    /// Check if an item ID exists in the internal item queue
    /// </summary>
    public bool ItemQueueContains(int itemId)
    {
        if (itemQueue == null)
            return false;

        return itemQueue.ContainsKey(itemId);
    }

    /// <summary>
    /// Update an item's internal data from the queue. This does not redraw the item's visuals.
    /// </summary>
    public void UpdateItemValue(int itemId, InventoryItemData data)
    {
        if (!itemQueue.ContainsKey(itemId))
            return;

        itemQueue[itemId] = data;
    }
}