using UnityEngine;

public class TutorialDialogArrow : MonoBehaviour
{
    public DialogArrowDirections Direction;
    public Vector2 positionOffset;

    private RectTransform m_rect;

    private float selfWidth;
    private float selfHeight;

    void Awake()
    {
        TryGetComponent(out m_rect);

        selfWidth  = m_rect.sizeDelta.x;
        selfHeight = m_rect.sizeDelta.y;
    }

    public bool IsActive => gameObject.activeInHierarchy;
    public GameObject GameObjectReference => this.gameObject;

    public void SetPositionToLeftOf(RectTransform element)
    {
        var elementHalfWidth = -(element.sizeDelta.x / 2.0F);
        var selfHalfWidth    = selfWidth / 2.0F;

        var posX = elementHalfWidth - selfHalfWidth + positionOffset.x;

        m_rect.anchoredPosition = new Vector2(posX, 0.0F);
    }

    public void SetPositionToRightOf(RectTransform element)
    {
        var elementHalfWidth = element.sizeDelta.x / 2.0F;
        var selfHalfWidth    = selfWidth / 2.0F;

        var posX = elementHalfWidth + selfHalfWidth - positionOffset.x;

        m_rect.anchoredPosition = new Vector2(posX, 0.0F);
    }

    public void SetPositionAbove(RectTransform element)
    {
        var elementHalfHeight = element.sizeDelta.y / 2.0F;
        var selfHalfHeight    = selfHeight / 2.0F;

        var posY = elementHalfHeight + selfHalfHeight - positionOffset.y;

        m_rect.anchoredPosition = new Vector2(0.0F, posY);
    }
}