using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Buy Skin Dialog Styles", menuName = "Scriptable Objects/Buy Skin Dialog Styles")]
public class BuySkinDialogStylesSO : ScriptableObject
{
    public Sprite DialogBackground;
    public Color OverlayColor = Color.white;
    public Color TitleColor = Color.white;
    public Color PromptTextColor = Color.white;
    public Color ControlButtonsBackground = Color.white;
}
