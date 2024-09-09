using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PowerupsAsset")]
public class PowerupsAsset : ScriptableObject
{
    public int                  Id;
    public string               Name;
    public Sprite               PreviewImage;
    public CurrencyType         Cost;
    public int                  Price;
    public PowerupCategories    PowerupCategory;
    public PowerupType          ItemType;
    public PowerupActivation    ActivationMode;
    public PowerupItemCardColor CardColor;
    public int                  MaxCount = 1;

    [Space(10)]
    [Header("For powerups driven by values (i.e. time)")]
    public int                  EffectValue;

    [Space(10)]
    [TextArea]
    public string Description;
}