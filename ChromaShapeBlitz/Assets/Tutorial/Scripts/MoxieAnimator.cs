using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum MoxieEyeAnimations
{
    Steady,             // Idle eyes
    BlinkIndefinite,    // Blinking indefinitely
    HappyIndefinite,    // Happy Eyes Indefinite
    HappyOneShot,       // Happy Eyes, Play Once, Revert to steady
    SadIndefinite,      // Sad Eyes, Play Once, Revert to steady
    SadOneShot          // Sad Eyes Indefinite
}

public enum MoxieEyeDirections
{
    Center,
    Left,
    Right,
    Up,
    Down
}

public enum MoxieBodyAnimations
{
    Idle,
    PointingRight,
    PointingLeft,
    Cute
}

public class MoxieAnimator : MonoBehaviour
{
    [Header("Moxie Eye Sprites ")]
    [SerializeField] private Sprite moxieEyeSteady;
    [SerializeField] private Sprite moxieEyeSad;
    [SerializeField] private Sprite moxieEyeHappy;

    [Space(10)] [Header("Moxie Eyes")]
    [SerializeField] private Image moxieEyeLeft;
    [SerializeField] private Image moxieEyeRight;

    [Space(10)] [Header("Animation States")]
    [SerializeField] private MoxieEyeAnimations currentEyeAnimState;
    [SerializeField] private MoxieBodyAnimations currentEyeDirection;

    private RectTransform leftEye;
    private RectTransform rightEye;

    private readonly float eyeSpacing = 20.0F;

    void Awake()
    {
        moxieEyeLeft.TryGetComponent(out leftEye);
        moxieEyeRight.TryGetComponent(out rightEye);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1))
            PlayEyeState(MoxieEyeAnimations.BlinkIndefinite);
    }

    void PlayEyeState(MoxieEyeAnimations eyeAnimationState)
    {
        currentEyeAnimState = eyeAnimationState;

        // Cancel currently playing animations
        LeanTween.cancel(gameObject);
        ResetEyes();

        // Play the animation by state
        switch (currentEyeAnimState)
        {
            case MoxieEyeAnimations.SadIndefinite:
                SetEyesAppearance(moxieEyeSad);
                break;

            case MoxieEyeAnimations.HappyIndefinite:
                SetEyesAppearance(moxieEyeHappy);
                break;

            case MoxieEyeAnimations.BlinkIndefinite:
                BlinkEyes();
                break;

            case MoxieEyeAnimations.Steady:
            default:
                ResetEyes();
                break;
        }
    }

    private void ResetEyes(bool resetAppearance = true)
    {
        rightEye.anchoredPosition = Vector2.right * eyeSpacing;
        leftEye.anchoredPosition  = Vector2.left * eyeSpacing;

        rightEye.localScale = Vector2.one;
        leftEye.localScale  = Vector2.one;

        if (!resetAppearance)
            return;

        moxieEyeRight.sprite = moxieEyeSteady;
        moxieEyeLeft.sprite  = moxieEyeSteady;
    }

    private void SetEyesAppearance(Sprite eyeAppearance)
    {
        moxieEyeRight.sprite = eyeAppearance;
        moxieEyeLeft.sprite  = eyeAppearance;
    }

    private void BlinkEyes(bool oneShot = false)
    {
        var blinkSpeed = 0.12F;
        var blinkRate  = 2.0F;

        var eyes = new GameObject[] {leftEye.gameObject, rightEye.gameObject};

        if (!oneShot)
        {
            SetEyesAppearance(moxieEyeSteady);

            for (var i = 0; i < eyes.Length; i++)
            {
                var eye = eyes[i];

                LeanTween.delayedCall(blinkRate, () =>
                {

                    LeanTween.scaleY(eye, 0.0F, blinkSpeed)
                    .setLoopPingPong()
                    .setEase(LeanTweenType.easeInOutBack)
                    .setRepeat(2)
                    .setOnComplete(() =>
                         {
                             // Open the eyes ..
                             LeanTween.scaleY(eye, 1.0F, blinkSpeed);
                         });

                }).setRepeat(-1);
            }
        }
    }
}
