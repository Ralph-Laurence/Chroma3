using System;
using UnityEngine;
using UnityEngine.Events;

public class GameOverScreenBase : MonoBehaviour
{
    [Space(10)]
    [Header("\u25A7\u25A7\u25A7\u25A7\u25A7\u25A7 Gameover Behaviour \u25A8\u25A8\u25A8\u25A8\u25A8\u25A8")]
    [Space(10)]

    [SerializeField] private bool playTransitionOnAwake = true;
    [SerializeField] private AudioClip gameOverSfx;
    [SerializeField] private AudioClip slideInSfx;
    [SerializeField] private AudioClip slideOutSfx;

    [Space(5)]
    [Header("Event Actions")]
    public UnityEvent onDialogShown;
    public UnityEvent onDialogHidden;

    [Space(10)]
    [Header("\u25A7\u25A7\u25A7\u25A7\u25A7\u25A7 Gameover Icon \u25A8\u25A8\u25A8\u25A8\u25A8\u25A8")]
    [Space(10)]

    [SerializeField] private RectTransform icon;
    [SerializeField] private int   oscillations = 2;
    [SerializeField] private float iconOscSpeed = 6.5F;
    [SerializeField] private float iconOscFrom  = -8.0F;
    [SerializeField] private float iconOscTo    = 8.0F;
    
    [Space(10)]
    [Header("\u25A7\u25A7\u25A7\u25A7\u25A7\u25A7 Gameover Dialog \u25A8\u25A8\u25A8\u25A8\u25A8\u25A8")]
    [Space(10)]
    [SerializeField] private RectTransform dialog;
    private Vector2 dialogStartPos;
    private GameOverTypes gameOverType;
    private SoundEffects sfx;

    void Start()
    {
        dialogStartPos = new Vector2
        (
            -Screen.width / 2.0F - dialog.rect.width / 2.0F,
            dialog.anchoredPosition.y
        );
    }

    public void SetGameOverType(GameOverTypes gameOverType) => this.gameOverType = gameOverType;

    /// <summary>
    /// Tweening for Gameover Icon
    /// </summary>
    /// <param name="onAnimationFinished"></param>
    public void OscillateIcon(Action onAnimationFinished = null)
    {
        if (sfx != null)
            sfx.PlayOnce(gameOverSfx);

        // Fade-in the icon
        LeanTween.alpha(icon, 1.0F, 0.16F);
        LeanTween.scale(icon, Vector3.one * 1.2F, 0.25F);

        // Adjust duration based on speed
        var timePerOscillation = 0.5f / iconOscSpeed;

        // Total number of half oscillations minus one to account for the last smooth transition
        var totalOscillations  = (oscillations * 2) - 1;

        // Callback after the icon animation finishes
        var onIconAnimated = new Action(() =>
        {
            // Smoothly return to 0 at the end
            LeanTween.rotateZ(icon.gameObject, 0f, timePerOscillation).setEase(LeanTweenType.easeInOutSine);
            
            // Wait for 0.45 seconds then shrink and fade out the icon
            // and finally show the dialog
            LeanTween.scale(icon, Vector3.one, 0.25F).setDelay(0.45F).setOnComplete(() => {
                
                LeanTween.scale(icon.gameObject, Vector3.zero, 0.25F);
                LeanTween.alpha(icon.gameObject, 0.0F, 0.25F)
                         .setOnComplete(onAnimationFinished);
            });
        });

        // Oscillate the icon back and forth
        LeanTween.value(icon.gameObject, iconOscFrom, iconOscTo, timePerOscillation)
            .setEase(LeanTweenType.easeInOutSine)
            .setLoopPingPong(totalOscillations)
            .setOnUpdate((float value) =>
            {
                icon.localRotation = Quaternion.Euler(0, 0, value);
            })
            .setOnComplete(onIconAnimated);
    }

    /// <summary>
    /// Show the gameover dialog
    /// </summary>
    private void SlideInDialog()
    {
        if (sfx != null)
            sfx.PlayOnce(slideInSfx);

        // Slide-In to the middle of the screen
        LeanTween.moveX(dialog, 0.0F, 0.25F)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() => {

                    // if (gameOverType == GameOverTypes.Success)
                    //     FillStars(starAmount);
                    onDialogShown?.Invoke();
                 });
    }

    private void SlideOutDialog(Action onSlid = null)
    {
        if (sfx != null)
            sfx.PlayOnce(slideOutSfx);

        // Slide-Out to the right of the screen
        LeanTween.moveX(dialog, Screen.width, 0.25F)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() => {

                    // Hide the main overlay
                    transform.parent.gameObject.SetActive(false);

                    onSlid?.Invoke();
                    onDialogHidden?.Invoke();
                 });
    }

    public void Hide() => SlideOutDialog(null);

    void OnEnable()
    {
        if (sfx == null)
            sfx = SoundEffects.Instance;

        if (playTransitionOnAwake)
            BeginAnimation();
    }

    // void OnDisable()
    // {
        
    // }

    public virtual void BeginAnimation()
    {
        OscillateIcon(onAnimationFinished: () => {
            
            // Set the initial position to the left of the screen
            dialog.anchoredPosition = dialogStartPos;
            dialog.gameObject.SetActive(true);

            SlideInDialog();
        });
    }
}
