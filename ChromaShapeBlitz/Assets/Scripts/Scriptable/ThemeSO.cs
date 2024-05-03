using UnityEngine;

[CreateAssetMenu(fileName = "Theme Template", menuName = "Scriptable Objects/Theme Template")]
public class ThemeSO : ScriptableObject
{
    public int Id;
    public string Name;
    public BlockColors Category;
    public Sprite ColorSprite;
    
    // This will be displayed in the shop item menu
    public Sprite PreviewImage;
    public float ScaleX = 1.0F;
    public float ScaleY = 1.0F;
    public float SpeedX = 0.1F;
    public float SpeedY = 0.1F;

    // COST     |> Coin or Gem
    // PRICE    |> How much
    public CurrencyType Cost;
    public int Price = 10;
}