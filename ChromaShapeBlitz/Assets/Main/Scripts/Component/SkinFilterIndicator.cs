using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkinFilterIndicator : MonoBehaviour
{
    [SerializeField] private Image border;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI label;

    public void DrawIndicator(ColorSwatches color)    
    {
        label.text       = color.ToColorName();
        border.color     = color.ToUnityColor();
        background.color = color.ToUnityColor();
    }
}