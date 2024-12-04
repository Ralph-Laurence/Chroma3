using UnityEngine;

[CreateAssetMenu(fileName = "ToggleThemeAsset", menuName = "Scriptable Objects/ToggleThemeAsset")]
public class ToggleThemeAsset : ScriptableObject
{
    [Header("Base Toggle")]
    public Sprite BaseSprite;
    public Color NormalColor = Color.white;
    public Color HoverColor  = new(0.96F, 0.96F, 0.96F);
    public Color ClickColor  = new(0.78F, 0.78F, 0.78F);
}
