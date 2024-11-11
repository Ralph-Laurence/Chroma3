using System.Collections;
using UnityEngine;

public class InGameHotBar : CommonHotbar
{
    public Sprite[] countIndicatorSubSprites;

    protected override void OnAwake() {}

    void OnEnable() => InGameHotbarInteractionStateNotifier.BindObserver(HandleObserver);
    void OnDisable() => InGameHotbarInteractionStateNotifier.UnbindObserver(HandleObserver);

    private void HandleObserver(bool blockInteraction)
    {
        StartCoroutine(ToggleSlots(blockInteraction));
    }

    private IEnumerator ToggleSlots(bool blockInteraction)
    {
        for (var i = 0; i < dynamicSlots.Length; i++)
        {
            if (blockInteraction)
                dynamicSlots[i].DisableInteraction();
            else
                dynamicSlots[i].SetInteractable();
                
            yield return null;
        }
    }

    public void UpdateSlotCount(int slotIndex, int count)
    {
        if (countIndicatorSubSprites == null || countIndicatorSubSprites?.Length < 1)
            return;

        GetSlot(slotIndex).SetCountIndicator(countIndicatorSubSprites[count]);
    }

    //public void SetCountIndicatorsSource(Sprite[] sprites) => countIndicatorSubSprites = sprites;
}