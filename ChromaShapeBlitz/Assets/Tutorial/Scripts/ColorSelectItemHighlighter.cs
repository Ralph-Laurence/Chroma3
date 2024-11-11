using UnityEngine;
using UnityEngine.UI;

public class ColorSelectItemHighlighter : MonoBehaviour
{
    private Toggle m_Toggle;
    private LTDescr tween;

    private Color colorToUse;

    void OnEnable()
    {
        if (m_Toggle == null && TryGetComponent(out m_Toggle))
        {
            var colorBlock = m_Toggle.colors;
            colorBlock.highlightedColor = Constants.ColorSwatches.ORANGE;

            m_Toggle.colors = colorBlock;
        }
    }

    public void HighlightWithColor(Color colorToUse)
    {
        this.colorToUse = colorToUse;

        GameObject itemBackground = null;

        // Loop to find the Dropdown List
        for (var i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            if (child.name.Equals("Item Background"))
            {
                itemBackground = child.gameObject;
                break;
            }
        }

        if (itemBackground != null && itemBackground.TryGetComponent(out Image img))
        {
            var color = img.color;
            color.a = 1.0F;

            img.color = color;
        }

        tween?.reset();
        tween = LeanTween.value(gameObject, OnValueCallback, 0.0F, 1.0F, 0.3F)
                         .setLoopPingPong();
    }

    private void OnValueCallback(float alpha)
    {
        var colorBlock  = m_Toggle.colors;
        var colorNormal = colorBlock.normalColor;
        
        colorNormal.r = colorToUse.r;
        colorNormal.g = colorToUse.g;
        colorNormal.b = colorToUse.b;
        colorNormal.a = alpha;

        colorBlock.normalColor = colorNormal;

        m_Toggle.colors = colorBlock;
    }
}
