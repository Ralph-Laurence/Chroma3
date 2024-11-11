using UnityEngine;

public class InventoryHotBar : CommonHotbar
{
    [SerializeField] protected AudioClip enqueueSfx;
    [SerializeField] protected AudioClip dequeueSfx;
    
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

        InventoryHotbarNotifier.NotifyObserver(InventoryHotbarEventNames.ItemEnqueued);
    }

    public override void DequeueItem(int slotIndex)
    {
        base.DequeueItem(slotIndex);

        if (enqueueSfx != null)
            sfx.PlayOnce(dequeueSfx);

        InventoryHotbarNotifier.NotifyObserver(InventoryHotbarEventNames.ItemDequeued);
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
}