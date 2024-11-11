using System;
using System.Collections.Generic;

[Serializable]
public struct SlotMachineItemData
{
    public int PrizeIdentifier;
    public string ItemName;
    public SlotMachineBetTypes ObtainableBy;
    public int Rarity;
    public int ItemID;
    public int PrizeAmountOnBetCoin;
    public int PrizeAmountOnBetGem;
    public SlotMachineItemPrizeTypes PrizeType;
}

[Serializable]
public struct SlotMachineItems
{
    public List<SlotMachineItemData> Items;
}
