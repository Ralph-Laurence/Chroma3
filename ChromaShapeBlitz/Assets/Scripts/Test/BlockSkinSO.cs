using UnityEngine;

[CreateAssetMenu(fileName = "Block Skin Template", menuName = "Scriptable Objects/Block Skin Template")]
public class BlockSkinSO : ScriptableObject
{
    public int Id;
    public string Name;
    public BlockColors Category;
    public Texture2D Skin;
    
    // This will be displayed in the shop item menu
    public Sprite PreviewImage;

    // COST     |> Coin or Gem
    // PRICE    |> How much
    public CurrencyType Cost;
    public int Price = 10;
}