using UnityEngine;

public class TutorialPointerHand3D : MonoBehaviour
{
    // Prevent UI interactions during hint animation
    [SerializeField] private Sprite pointerOff;
    [SerializeField] private Sprite pointerOn;
    [SerializeField] private SpriteRenderer pointer;
    [SerializeField] private float pointerScale = 0.1F;

    private RectTransform pointerRect;
    private readonly float blinkRate = 0.125F;
    [SerializeField] private Camera orthoCam;
    private bool isSpriteOn;

    void Awake()
    {
        pointer.TryGetComponent(out pointerRect);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdatePointerScale();
    }

    // Update is called once per frame
    void Update() 
    {
        //UpdatePointerScale(); 
        //UpdatePointerRotation();
    }

    void OnEnable()
    {
        SwitchSprite();
    }

    void OnDisable()
    {
        // Cancel the LeanTween delayed call when the GameObject is disabled
        LeanTween.cancel(gameObject);
    }

    /// <summary>
    /// Resize the pointer so that it looks the same across resolutions
    /// </summary>
    private void UpdatePointerScale()
    {
        var height = pointer.bounds.size.y;
        var scale = pointerScale / height;

        pointer.transform.localScale = new (scale, scale, 1.0F);
    }

    private void UpdatePointerRotation()
    {
        var newRot = new Vector3
        (
            orthoCam.transform.localEulerAngles.x,
            pointerRect.localEulerAngles.y,
            0.0F
        );

        pointerRect.localEulerAngles = newRot;
    }

    public void MatchStageRotation(Vector3 eulerAngles) => pointer.transform.eulerAngles = eulerAngles;

    void SwitchSprite()
    {
        // Toggle the active sprite
        isSpriteOn      = !isSpriteOn;
        pointer.sprite  = isSpriteOn ? pointerOn : pointerOff;

        // Use LeanTween to call this method again after a delay
        //tweenDescr = LeanTween.delayedCall(gameObject, blinkRate, SwitchSprite); // Adjust the delay as needed
        LeanTween.delayedCall(gameObject, blinkRate, SwitchSprite); // Adjust the delay as needed
    }

}
