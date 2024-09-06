using UnityEngine;

public enum InventoryItemType
{
    Unset,
    ReadOnly,
    Consumable
}

/// <summary>
/// This is the serializable version of InventoryItemData.
/// <list type="number">
///     <item>
///         <term>ID</term>
///         <description>This is enough to identify the cache'd item.</description>
///     </item>
///     <item>
///         <term>CurrentAmount</term>
///         <description>This is necessary to track the item's current amount.</description>
///     </item>
/// </list>
/// </summary>
[SerializeField]
public struct CacheableInventoryItemData
{
    public int ItemID;
    public int CurrentAmount;
}

public struct InventoryItemData
{
    public int          ID;
    public string       Name;
    public int          CurrentAmount;
    public int          MaxAmount;
    public bool         IsVisible;

    public Sprite       Thumbnail;
    public Sprite       AmountIcon;
    public GameObject   AttachedGameObject;
    
    public InventoryItemType ItemType;
    
    /// <summary>
    /// Show the attached gameobject and its "IsVisible" property
    /// </summary>
    public void SetVisible()
    {
        IsVisible = true;

        if (AttachedGameObject != null )
            AttachedGameObject.SetActive( true );
    }

    /// <summary>
    /// Hide the attached gameobject and its "IsVisible" property
    /// </summary>
    public void SetInvisible()
    {
        IsVisible = false;

        if (AttachedGameObject != null)
            AttachedGameObject.SetActive(false);
    }

    /// <summary>
    /// Reset / Clear All internal data of this item.
    /// <list type="bullet">
    ///     <item>
    ///         <description>This will also drop a reference to the attached gameobject.</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <param name="hide">Should we hide the attached gameobject after resetting the item's internal data?</param>
    public void Reset(bool hide = false)
    {
        ID                  = default;
        Name                = default;
        CurrentAmount       = default;
        MaxAmount           = default;
        IsVisible           = default;
        Thumbnail           = default;
        ItemType            = InventoryItemType.Unset;
        AmountIcon          = default;

        if (AttachedGameObject == null)
            return;

        AttachedGameObject.SetActive(!hide);
        AttachedGameObject = null;
    }

}