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

    private void Awake()
    {
        exclamation.TryGetComponent(out exclamationRect);
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

    void StartBeating()
    {
        LeanTween.scale(exclamation.gameObject, Vector3.one * beatScale, beatTime)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setLoopPingPong(); // Makes the scale go up and down in a loop
    }
}
