using System;
using Revamp;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    [Space(10)]
    [Header("Main Behaviour")]
    [SerializeField] private RectTransform targetDialog;
    [SerializeField] private GameOverTypes gameOverType;
    [SerializeField] private TextMeshProUGUI totalRewardLabel;
    [SerializeField] private TextMeshProUGUI totalPlayTimeLabel;
    [SerializeField] private TextMeshProUGUI totalPlayerBalanceLabel;
    [SerializeField] private AudioClip gameOverSfx;
    [SerializeField] private AudioClip dialogSlideInSfx;
    [SerializeField] private AudioClip dialogSlideOutSfx;
    [SerializeField] private GameOverScreenButton[] dialogButtons;

    [Space(10)]
    [Header("Icon Tweening")]
    [SerializeField] private GameObject icon;
    [SerializeField] private int   oscillations = 2;
    [SerializeField] private float iconOscSpeed = 6.5F;
    [SerializeField] private float iconOscFrom  = -8.0F;
    [SerializeField] private float iconOscTo    = 8.0F;

    [Space(10)]
    [Header("Success Star Rating")]
    [SerializeField] private Image[] stars;
    [SerializeField] private Sprite  filledStar;
    [SerializeField] private Sprite  defaultStar;

    [Space(4)]
    [SerializeField] private float starAnimationDuration = 0.3F;
    [SerializeField] private float delayBetweenStars = 0.16F;

    [Space(4)]
    [SerializeField] private AudioClip starFillSfx;
    [SerializeField] private AudioClip starPerfectSfx;

    private Vector2 targetDialogStartPos;
    private RectTransform iconRect;
    private Image iconImage;
    private SoundEffects sfx;
    private GameOverScreenButton nextButton;

    private bool isInitialized;
    private int starAmount;

    void OnEnable()
    {
        Initialize();
        GameOverScreenNotifier.BindEvent(ObserveGameOver);
    }

    void OnDisable() => GameOverScreenNotifier.UnbindEvent(ObserveGameOver);
    void Awake()     => Initialize();

    private void ObserveGameOver(GameOverEventArgs eventArgs)
    {
        if (eventArgs.GameOverType != gameOverType)
            return;

        ResetTweens();

        // Should we hide the "Next" button?
        // This happens only when the final ending stage was reached.
        if (nextButton != null)
        {
            if (eventArgs.DisableNextButton)
                nextButton.DisableClicks();
            else
                nextButton.EnableClicks();
        }

        var textColor = "<color=#FF9533>"; // ORANGE
        
        if (eventArgs.GameOverType == GameOverTypes.Success)
        {
            textColor = "<color=#81FF21>"; // GREEN 
            totalRewardLabel.text = eventArgs.TotalReward.ToRewardText(eventArgs.RewardType);
        }

        starAmount = eventArgs.TotalStars;
        totalPlayTimeLabel.text = $"{textColor}{eventArgs.TotalPlayTime} secs";


        var currencyStyle = "Coin";
        var totalBalance  = eventArgs.TotalPlayerCoinBalance;
        
        if (eventArgs.RewardType.Equals(RewardTypes.Gems))
        {
            currencyStyle = "Gem";
            totalBalance = eventArgs.TotalPlayerGemBalance;
        }

        totalPlayerBalanceLabel.text = $"<style=\"{currencyStyle}\">\u00d7{totalBalance}";
        
        BeginAnimation();
    }

    private void Initialize()
    {
        if (isInitialized)
            return;

        sfx = SoundEffects.Instance;
        icon.TryGetComponent(out iconRect);
        iconRect.TryGetComponent(out iconImage);

        targetDialogStartPos = new Vector2
        (
            -Screen.width / 2.0F - targetDialog.rect.width / 2.0F,
            targetDialog.anchoredPosition.y
        );

        foreach (var b in dialogButtons)
        {
            // We will use this when we want to disable the "Next" button,
            // usually when we reached the final ending stage.
            if (b.Action == GameManagerActionEvents.NextStage)
                nextButton = b;

            b.ButtonComponent.onClick.AddListener(() => {
                Hide(() => GameManagerEventNotifier.Notify(b.Action));
            });
        }

        isInitialized = true;
    }

    private void PlaySfx(AudioClip clip)
    {
        if (sfx == null)
            return;

        sfx.PlayOnce(clip);
    }

    private void ResetTweens()
    {
        LeanTween.reset();

        // Reset the icon
        iconImage.color = new Color(1.0F, 1.0F, 1.0F, 0.0F);
        iconRect.localScale = Vector3.zero;

        // Reset the dialog
        targetDialog.anchoredPosition = targetDialogStartPos;
        targetDialog.gameObject.SetActive(false);

        // Reset the stars
        foreach (var star in stars)
        {
            star.transform.localScale = Vector3.one;
            star.sprite = defaultStar;
        }
    }

    private void BeginAnimation()
    {
        PlaySfx(gameOverSfx);

        // Fade-in the icon
        LeanTween.alpha(iconRect, 1.0F, 0.16F);
        LeanTween.scale(iconRect, Vector3.one * 1.2F, 0.25F);

        // Adjust duration based on speed
        var timePerOscillation = 0.5f / iconOscSpeed;

        // Total number of half oscillations minus one to account for the last smooth transition
        var totalOscillations  = (oscillations * 2) - 1;

        // Callback after the icon animation finishes
        var onIconAnimated = new Action(() =>
        {
            // Smoothly return to 0 at the end
            LeanTween.rotateZ(icon, 0f, timePerOscillation).setEase(LeanTweenType.easeInOutSine);
            
            // Wait for 0.45 seconds then shrink and fade out the icon
            // and finally show the dialog
            LeanTween.scale(icon, Vector3.one, 0.25F).setDelay(0.45F).setOnComplete(() => {
                
                LeanTween.scale(iconRect, Vector3.zero, 0.25F);
                LeanTween.alpha(iconRect, 0.0F, 0.25F).setOnComplete(() => SlideInDialog());
            });
        });

        // Oscillate the icon back and forth
        LeanTween.value(icon, iconOscFrom, iconOscTo, timePerOscillation)
            .setEase(LeanTweenType.easeInOutSine)
            .setLoopPingPong(totalOscillations)
            .setOnUpdate((float value) =>
            {
                iconRect.localRotation = Quaternion.Euler(0, 0, value);
            })
            .setOnComplete(onIconAnimated);
    }

    /// <summary>
    ///  This function only applies to Gameover Success
    /// </summary>
    /// <param name="amount">The amount of stars to fill</param>
    private void FillStars(int amount)
    {
        amount = Mathf.Clamp(amount, 0, stars.Length);

        var starSfxIndex = 1;

        for (var i = 0; i < amount; i++)
        {
            var starIndex = i;
            var delay     = (starAnimationDuration + delayBetweenStars) * i;
            var starObj   = stars[starIndex].gameObject;

            LeanTween.value(starObj, 0f, 1f, starAnimationDuration)
                     .setDelay(delay)
                     .setOnUpdate((float val) =>
                     {
                        stars[starIndex].sprite = filledStar;
                        stars[starIndex].transform.localScale = Vector3.Lerp(Vector3.one * 2.5F, Vector3.one, val);
                     })
                     .setOnComplete(() =>
                     {
                        // The third star gets a different sound
                        var clip = starSfxIndex == 3 ? starPerfectSfx : starFillSfx;
                        PlaySfx(clip);

                        starSfxIndex++;
                        
                        // Ensure the scale is set to 1 after animation
                        stars[starIndex].transform.localScale = Vector3.one;
                     });
        }
    }

    /// <summary>
    /// Show the gameover dialog
    /// </summary>
    private void SlideInDialog()
    {
        // Set the initial position to the left of the screen
        targetDialog.anchoredPosition = targetDialogStartPos;
        targetDialog.gameObject.SetActive(true);

        PlaySfx(dialogSlideInSfx);

        // Slide-In to the middle of the screen
        LeanTween.moveX(targetDialog, 0.0F, 0.25F)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() => {

                    if (gameOverType == GameOverTypes.Success)
                        FillStars(starAmount);
                 });
    }

    private void SlideOutDialog(Action onSlid = null)
    {
        PlaySfx(dialogSlideOutSfx);

        // Slide-Out to the right of the screen
        LeanTween.moveX(targetDialog, Screen.width, 0.25F)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() => {
                    ResetTweens();

                    // Hide the main overlay
                    transform.parent.gameObject.SetActive(false);

                    onSlid?.Invoke();
                 });
    }

    public void Hide(Action onSlid = null) => SlideOutDialog(onSlid);
}
