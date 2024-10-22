using UnityEngine;
using UnityEngine.UI;

public class TutorialPointerHand : MonoBehaviour
{
    [SerializeField] private Image uiImage; // The UI Image component
    [SerializeField] private Sprite rightHandOn;
    [SerializeField] private Sprite rightHandOff;
    [SerializeField] private Sprite leftHandOn;
    [SerializeField] private Sprite leftHandOff;

    private readonly float blinkRate = 0.125F;
    private Sprite spriteOff; // First sprite
    private Sprite spriteOn; // Second sprite
    private bool isSpriteOn = true;
    //private LTDescr tweenDescr;
    private RectTransform m_rect;

    void Awake()
    {
        TryGetComponent(out m_rect);
    }

    // void OnEnable()
    // {
    //     // Start the sprite switching loop when the GameObject is enabled
    //     SwitchSprite();
    // }

    // void OnDisable()
    // {
    //     // Cancel the LeanTween delayed call when the GameObject is disabled
    //     if (tweenDescr != null)
    //     {
    //         LeanTween.cancel(tweenDescr.uniqueId);
    //     }
    // }

    void SwitchSprite()
    {
        // Toggle the active sprite
        isSpriteOn      = !isSpriteOn;
        uiImage.sprite  = isSpriteOn ? spriteOn : spriteOff;

        // Use LeanTween to call this method again after a delay
        //tweenDescr = LeanTween.delayedCall(gameObject, blinkRate, SwitchSprite); // Adjust the delay as needed
        LeanTween.delayedCall(gameObject, blinkRate, SwitchSprite); // Adjust the delay as needed
    }

    public void SetTarget(RectTransform target)
    {
        // Reset the animation
        Hide();

        // Determine the rotation of the pointer
        var pointerAngle = 30.0F;

        // These are the default pointer hand appearances
        spriteOn  = rightHandOn;
        spriteOff = rightHandOff;

        if (target.anchoredPosition.x < 0.0F)    
            m_rect.localEulerAngles = Vector3.back * pointerAngle;
        
        else if (target.anchoredPosition.x > 0.0F)
            m_rect.localEulerAngles = Vector3.forward * pointerAngle;
        
        else
            m_rect.localEulerAngles = Vector3.zero;

        m_rect.anchoredPosition = target.anchoredPosition;
        transform.SetAsLastSibling();
    }

    public void Show()
    {
        uiImage.enabled = true;
        SwitchSprite();
    }

    public void Hide()
    {
        // Cancel the LeanTween delayed call when the GameObject is disabled
        LeanTween.cancel(gameObject);
        uiImage.enabled = false;
    }
}