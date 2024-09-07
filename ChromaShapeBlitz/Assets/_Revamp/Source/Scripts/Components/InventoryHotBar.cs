using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHotBar : CommonHotbar
{
    private SoundEffects sfx;
    
    protected override void OnAwake()
    {
        sfx = SoundEffects.Instance;
    }

    public override void EnqueueItem(InventoryItemData itemData)
    {
        base.EnqueueItem(itemData);

        if (enqueueSfx != null)
            sfx.PlayOnce(enqueueSfx);
    }

    public override void DequeueItem(int slotIndex)
    {
        base.DequeueItem(slotIndex);

        if (enqueueSfx != null)
            sfx.PlayOnce(dequeueSfx);
    }

    /// <summary>
    /// Compare two hotbar items such as internal queue and updated queue
    /// </summary>
    // public bool HotbarChanged(List<int> original, List<int> current)
    // {
    //     // First: Length Check => Check if the lengths of the original and current lists are different.
    //     // If they are, it returns true.
    //     if (original.Count != current.Count)
    //         return true;

    //     // Second: Element Comparison => Iterate through both lists and compares each element.
    //     // If any element is different, it returns true.
    //     for (int i = 0; i < original.Count; i++)
    //     {
    //         if (original[i] != current[i])
    //             return true;
    //     }
        
    //     // No Changes: If all elements are the same and in the same order, it returns false.
    //     return false;
    // }

    /// <summary>
    /// Fills the hotbar with new items. This will force each slot to reset their current state.
    /// </summary>
    public IEnumerator RePopulateHotbar()
    {
        yield return StartCoroutine(IEPopulateHotbar(GetItemQueue()));
    }

    private IEnumerator IEPopulateHotbar(Dictionary<int, InventoryItemData> dataSource)
    {
        if (dataSource == null || dataSource.Count <= 0)
            yield break;

        var items = new List<InventoryItemData>();

        // Collect every item data from the pair
        foreach (var item in dataSource)
        {
            items.Add(item.Value);
            yield return null;
        }

        // Force reset all slots then fill them with new items
        for (var i = 0; i < dynamicSlots.Length; i++)
        {
            var slot = dynamicSlots[i];
            slot.Reset();

            if (i < dataSource.Count)
            {
                var itemData = items[i];

                slot.FillItem(new HotbarSlotDataSource
                {
                    ItemId = itemData.ID,
                    ItemCountSprite = itemData.AmountIcon,
                    ItemThumbnail = itemData.Thumbnail,
                });
            }

            yield return null;
        }

        yield return null;
    }
}