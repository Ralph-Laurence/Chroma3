using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/BackgroundsAsset")]
public class BackgroundsAsset : ScriptableObject
{
    public int Id;
    public string Name;
    public string Description;
    public Sprite PreviewImage;
    public CurrencyType Cost;
    public int Price;
}