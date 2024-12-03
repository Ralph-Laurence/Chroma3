using UnityEngine;

[CreateAssetMenu(fileName = "ButtonThemeAsset", menuName = "Scriptable Objects/ButtonThemeAsset")]
public class ButtonThemeAsset : ScriptableObject
{
    [Header("Base Button")]
    public Vector2 BaseScale = Vector2.one;
    public Sprite BaseSprite;
    public Color NormalColor = Color.white;
    public Color HoverColor  = new(0.96F, 0.96F, 0.96F);
    public Color ClickColor  = new(0.78F, 0.78F, 0.78F);

    [Header("Button Icon")]
    public Sprite Icon;
    public Vector2 IconScale = Vector2.one;
}
