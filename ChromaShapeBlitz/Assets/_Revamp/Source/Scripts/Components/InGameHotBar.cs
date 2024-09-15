using System;
using UnityEngine;

public class InGameHotBar : CommonHotbar
{
    private Sprite[] countIndicatorSubSprites;

    protected override void OnAwake()
    {
     
    }

    public void UpdateSlotCount(int slotIndex, int count)
    {
        if (countIndicatorSubSprites == null || countIndicatorSubSprites?.Length < 1)
            return;

        GetSlot(slotIndex).SetCountIndicator(countIndicatorSubSprites[count]);
    }

    public void SetCountIndicatorsSource(Sprite[] sprites) => countIndicatorSubSprites = sprites;
}
