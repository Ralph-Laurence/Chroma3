using UnityEngine;
using UnityEngine.UI;

public class MainMenuSideFab : MonoBehaviour
{
    [SerializeField] private Image exclamation;
    [SerializeField] private Sprite exclamationOn;
    [SerializeField] private Sprite exclamationOff;

    [SerializeField] private float beatScale = 1.2f; // The scale to increase to
    [SerializeField] private float beatTime  = 0.5f; // Time for one beat

    private RectTransform exclamationRect;
    private Image m_image;
    private RectTransform m_rect;

    private void Awake()
    {
        exclamation.TryGetComponent(out exclamationRect);
        TryGetComponent(out m_image);
        TryGetComponent(out m_rect);
    }

    public void MakeActive()
    {
        exclamation.sprite = exclamationOn;

        // Start the beating effect
        StartBeating();
    }

    public void MakeInactive()
    {
        exclamation.sprite = exclamationOff;

        LeanTween.cancel(exclamation.gameObject);
        exclamationRect.localScale = Vector3.one;
    }

    public void ChangeFabIcon(Sprite newIcon) => m_image.sprite = newIcon;
    
    public void SetScale(Vector2 scale)
    {
        m_rect.localScale = new Vector3(scale.x, scale.y, 1.0F);
    }

    void StartBeating()
    {
        LeanTween.scale(exclamation.gameObject, Vector3.one * beatScale, beatTime)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setLoopPingPong(); // Makes the scale go up and down in a loop
    }
}
