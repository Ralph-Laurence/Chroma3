using System;
using UnityEngine;

[Serializable]
public class BaseItemInfo
{
    [Space(5)]
    [Header("Shop Item Details")]
    public int Id;
    public string Name;
    
    public ColorSwatches ColorCategory;
    public Sprite PreviewImage;
    public CurrencyType Cost;
    public int Price;

    // This must be hidden in the Inspector because we can't manually set this.
    // This goes with the List of IDs.
    [HideInInspector]
    public bool IsOwned;
}