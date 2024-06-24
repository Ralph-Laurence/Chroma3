using UnityEngine;

[CreateAssetMenu(fileName = "Block Skin Asset", menuName = "Scriptable Objects/Block Skin Asset")]
public class BlockSkinsAsset : ScriptableObject
{
    public int Id;
    public string Name;
    
    public ColorSwatches ColorCategory;
    public Sprite PreviewImage;
    public CurrencyType Cost;
    public int Price;

    [Space(5)]
    public Material Material;
    public string MaterialFilename;
}