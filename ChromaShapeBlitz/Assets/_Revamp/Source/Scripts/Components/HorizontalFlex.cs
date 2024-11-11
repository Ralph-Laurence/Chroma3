using UnityEngine;

public enum HorizontalFlexDirection
{
    Start,
    End
}

public class HorizontalFlex : MonoBehaviour
{
    public HorizontalFlexDirection flexDirection;
    private RectTransform parentRect;
    public float spacing = 10f;
    public float rightMargin = 20f; // Adjust as needed
    public float leftMargin = 20f;  // Adjust as needed

    private void Awake()
    {
        TryGetComponent(out parentRect);
    }

    void Update()
    {
        if (flexDirection == HorizontalFlexDirection.Start)
            AlignChildrenLeft();
        else
            AlignChildrenRight();
    }

    void AlignChildrenLeft()
    {
        float parentWidth = parentRect.rect.width;
        float currentX = 0f;

        foreach (RectTransform child in parentRect)
        {
            float childWidth = child.sizeDelta.x;
            child.anchorMin = new Vector2(0, 0.5f);
            child.anchorMax = new Vector2(0, 0.5f);
            child.pivot = new Vector2(0, 0.5f);
            child.anchoredPosition = new Vector2(currentX, 0);

            currentX += childWidth + spacing;
        }

        // Adjust the width of the parent to fit all children if needed
        if (currentX > parentWidth)
        {
            parentRect.sizeDelta = new Vector2(currentX, parentRect.sizeDelta.y);
        }
    }

    void AlignChildrenRight()
    {
        float parentWidth = parentRect.rect.width;
        float currentX = 0f;

        var idxFirstChild = 0;

        foreach (RectTransform child in parentRect)
        {
            float childWidth = child.sizeDelta.x;
            child.anchorMin = new Vector2(1.0f, 0.5f);
            child.anchorMax = new Vector2(1.0f, 0.5f);
            child.pivot = new Vector2(1.0f, 0.5f);

            if (idxFirstChild == 0)
            {
                child.anchoredPosition = new Vector2(currentX - rightMargin, 0);
                currentX -= (childWidth + 10 + rightMargin + spacing);
            }
            else
            {
                child.anchoredPosition = new Vector2(currentX, 0);
                currentX -= (childWidth + 10 + spacing);
            }

            idxFirstChild++;
        }
    }
}
