using UnityEngine;
using UnityEngine.UI;

public class ThemableToggle : MonoBehaviour
{
    private Image m_image;
    private Toggle m_toggle;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        TryGetComponent(out m_toggle);
        TryGetComponent(out m_image);
    }

    public void SetColor(Color normal, Color hover, Color click)
    {
        // Create a new ColorBlock
        ColorBlock cb = m_toggle.colors;
        
        // Set the normal color
        cb.normalColor = normal;

        // Set the highlighted color (hover color)
        cb.highlightedColor = hover;

        // Set the pressed color
        cb.pressedColor = click;

        // Apply the modified ColorBlock back to the toggle
        m_toggle.colors = cb;
    }

    public void SetBackground(Sprite background) => m_image.sprite = background;
}
