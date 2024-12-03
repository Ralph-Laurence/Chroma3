using UnityEngine;
using UnityEngine.UI;

public class ThemableButton : UXButton
{
    [SerializeField] private Image icon;
    private Button m_button;
    private RectTransform iconRect;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        TryGetComponent(out m_button);
        icon.TryGetComponent(out iconRect);
    }

    public void SetColor(Color normal, Color hover, Color click)
    {
        // Create a new ColorBlock
        ColorBlock cb = m_button.colors;
        
        // Set the normal color
        cb.normalColor = normal;

        // Set the highlighted color (hover color)
        cb.highlightedColor = hover;

        // Set the pressed color
        cb.pressedColor = click;

        // Apply the modified ColorBlock back to the button
        m_button.colors = cb;
    }

    public void SetIcon(Sprite newIcon) => icon.sprite = newIcon;
    public void ScaleIcon(Vector2 scale) => iconRect.localScale = scale;
}
