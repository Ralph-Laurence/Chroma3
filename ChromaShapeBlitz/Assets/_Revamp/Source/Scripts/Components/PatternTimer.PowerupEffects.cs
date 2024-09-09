using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public partial class PatternTimer : MonoBehaviour
{
    [Space(10)]
    [Header("Time Freeze Effect")]
    [SerializeField] private GameObject freezeEffectOverlayObj;
    [SerializeField] private GameObject frozenTimeLabelEffect;
    [SerializeField] private AudioClip  sfxThreeSecs;
    [SerializeField] private AudioClip  sfxFiveSecs;
    [SerializeField] private float      freezeEffectSpeed = 4.0F;
    [SerializeField] private Color      frozenTimeBadgeColor = new(0.58F, 0.83F, 0.97F, 1.0F);
    [SerializeField] private Color      frozenTimerFillColor = new(0.04F, 0.32F, 0.99F, 1.0F);

    [Space(10)]
    [Header("Pattern Reveal Effect")]
    [SerializeField] private GameObject darkenerMask;
    [SerializeField] private GameObject patternLaser;
    [SerializeField] private Image      darkPattern;
    [SerializeField] private AudioClip  sfxRevealPattern;

    private readonly Color TRANSPARENT = Constants.ColorSwatches.TRANSPARENT;
    private RectTransform darkenerMaskRect;
    private Image freezeOverlay;
    private Image frozenTimeLabel;
    private Image frozenTimeBadge;
    private bool isTimeFreeze;
    private float freezeTimeRemaining;
    private float patternRevealTimeRemaining;
    private bool isPatternReveal;

    private LTDescr freezeTimeTween;

    // The awake method
    private void InitializePowerupEffector()
    {
        freezeEffectOverlayObj.TryGetComponent(out freezeOverlay);
        freezeOverlay.enabled = false;

        timerBadge.TryGetComponent(out frozenTimeBadge);
        frozenTimeLabelEffect.TryGetComponent(out frozenTimeLabel);

        darkenerMask.TryGetComponent(out darkenerMaskRect);
    }

    #region EVENT_OBSERVERS
    void OnEnable()
    {
        HotbarPowerupEffectNotifier.BindObserver(ObservePowerupReceived);
    }

    void OnDisable()
    {
        HotbarPowerupEffectNotifier.UnbindObserver(ObservePowerupReceived);
    }

    private void ObservePowerupReceived(HotBarSlot sender, PowerupEffectData effectData)
    {
        // We cant apply powerups when timer is 0
        if (remainingTime < 1.0F)
            return;

        // These are the only powerup effects recognized by the pattern timer
        switch (effectData.Category)
        {
            case PowerupCategories.TimerPause:

                // Initiate the time freeze
                if (isTimeFreeze) return;
                
                var freezeDuration = effectData.EffectValue;

                StartCoroutine( IEFreezeTimer(freezeDuration) );
                sender.BeginLockSlot(freezeDuration + 1);

                break;

            case PowerupCategories.TimerIncrease:
                
                var increase = effectData.EffectValue;

                AddTime(increase);

                // Immediately apply the text without waiting for the Update() to handle it
                timerText.text = Mathf.CeilToInt(remainingTime + increase).ToString();

                // If the remaining time after applying the powerup is greater than the critical,
                // we must reset the bgm volume as it reduces upon reaching critical seconds
                if (remainingTime > CriticalSecOffset)
                    bgm.ResetVolume();

                // Add extra secs lock to avoid abuse of power
                sender.BeginLockSlot(increase);

                break;

            case PowerupCategories.PatternReveal:

                if (isPatternReveal) return;

                var seconds = effectData.EffectValue;
                RevealPatternForSeconds(seconds);

                // Disable the slot
                sender.BeginLockSlot(seconds);

                break;
        }
    }
    #endregion EVENT_OBSERVERS
    
    #region POWERUP_EFFECTS_FREEZE_TIMER

    private IEnumerator IEFreezeTimer(int seconds)
    {
        isTimeFreeze = true;
        freezeTimeRemaining = seconds;

        // Animate the effect [this isnt the actual effect; just aesthetics]
        ShowFreezeEffect(seconds);

        // This is the actual pause effect.
        // Countdown the freeze effect, respecting the game's time scale
        while (freezeTimeRemaining > 0.0F)
        {
            // We only apply the time freeze countdown when the game isnt paused
            if (Time.timeScale > 0.0F)
            {
                freezeTimeRemaining -= Time.deltaTime;
            }
            yield return null;
        }
        
        ApplyFreezeTimeLabel(false);
        isTimeFreeze = false; // Resume the main timer once the freeze ends
    }

    private void ShowFreezeEffect(int freezeSeconds)
    {
        var sound = sfxThreeSecs;

        if (freezeSeconds == 5)
            sound = sfxFiveSecs;

        freezeOverlay.enabled = true;
        sfx.PlayOnce(sound);

        var effectSpeed = freezeSeconds / this.freezeEffectSpeed;
        var targetColor = Constants.ColorSwatches.WHITE;
        
        SetFreezeOverlayColor(TRANSPARENT);
        
        // Apply the time freeze on time label
        ApplyFreezeTimeLabel(true);

        // Apply the time freeze overlay effect
        freezeTimeTween?.reset();
        freezeTimeTween = LeanTween.value(freezeEffectOverlayObj, SetFreezeOverlayColor, freezeOverlay.color, targetColor, effectSpeed)
                 .setOnComplete(() => {

                    SetFreezeOverlayColor(targetColor);
                    var delay = freezeSeconds - effectSpeed;

                    LeanTween.value(freezeEffectOverlayObj, SetFreezeOverlayColor, freezeOverlay.color, TRANSPARENT, effectSpeed)
                             .setDelay(delay)
                             .setOnComplete(() => {
                                SetFreezeOverlayColor(TRANSPARENT);
                                freezeOverlay.enabled = false;
                             });
                 });
    }

    private void SetFreezeOverlayColor(Color color) => freezeOverlay.color = color;
    private void SetFrozenLabelColor(Color color) => frozenTimeLabel.color = color;

    private void ApplyFreezeTimeLabel(bool freeze)
    {
        if (!freeze)
        {
            frozenTimeLabel.enabled = false;
            frozenTimeBadge.color   = normalTimerBadgeColor;
            timerFill.color         = normalTimerFillColor;

            return;
        }
        
        frozenTimeLabel.enabled = true;
        SetFrozenLabelColor(TRANSPARENT);

        LeanTween.value(
                frozenTimeLabelEffect, 
                SetFrozenLabelColor, 
                frozenTimeLabel.color, 
                Constants.ColorSwatches.WHITE, 
                0.25F
            )
            .setOnComplete(() => {
                timerFill.color = frozenTimerFillColor;
                frozenTimeBadge.color = frozenTimeBadgeColor;
            });
    }

    #endregion POWERUP_EFFECTS_FREEZE_TIMER

    #region POWERUP_EFFECTS_ADD_SECONDS

    // Add extra seconds to the countdown
    public void AddTime(int seconds)
    {
        remainingTime += seconds;

        // Ensure remaining time does not exceed the original duration if needed
        remainingTime = Mathf.Clamp(remainingTime, 0, duration);
    }
    #endregion POWERUP_EFFECTS_ADD_SECONDS

    #region POWERUP_EFFECTS_REVEAL_PATTERN

    /// 
    /// <summary>
    /// After the main pattern gets black ...
    /// </summary>
    public void HandlePatternDarkened(Sprite pattern)
    {
        // Hide the pattern previewer by blocking it with a mask
        ConcealPattern();

        // Set the darkener pattern similar to the previewer
        darkPattern.sprite = pattern;
    }

    /// <summary>
    /// Hide the pattern previewer by blocking it with a Mask
    /// </summary>
    private void ConcealPattern()
    {
        // Reset the darkener mask transforms
        darkenerMaskRect.sizeDelta = new Vector3(128.0F, 90.0F, 1.0F);
        darkenerMaskRect.anchoredPosition = Vector2.zero;

        // Show the darkener mask, but hide the laser for now.
        // We will only show the laser when we want to reveal the pattern
        darkenerMask.SetActive(true);
        patternLaser.SetActive(false);
    }

    /// <summary>
    /// Reveal the previewer without animations
    /// </summary>
    private void RevealPatternImmediate()
    {
        darkenerMask.SetActive(false);
        patternPreviewer.color = Color.white;
    }

    /// <summary>
    /// Reveal the previewer for a given duration, of course with animations
    /// </summary>
    private void RevealPatternForSeconds(int seconds)
    {
        darkenerMask.SetActive(true);
        patternLaser.SetActive(true);

        var initialHeight   = darkenerMaskRect.sizeDelta.y;
        var initialY        = darkenerMaskRect.anchoredPosition.y;
        var scaleDown       = 0.0F;
        var rate            = 0.75F;
        
        sfx.PlayOnce(sfxRevealPattern);

        LeanTween.value(darkenerMask, initialHeight, scaleDown, rate)
                 .setOnUpdate((float value) =>
                 {
                     var sizeDelta = darkenerMaskRect.sizeDelta;
                     sizeDelta.y   = value;
         
                     darkenerMaskRect.sizeDelta = sizeDelta;
         
                     // Adjust the anchored position to maintain the bottom position
                     darkenerMaskRect.anchoredPosition = new Vector2
                     (
                         darkenerMaskRect.anchoredPosition.x, 
                         initialY - (initialHeight - value) / 2.0F
                     );
         
                 })
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() => {
                    darkenerMask.SetActive(false);

                    // Darken the pattern again
                    StartCoroutine(IEDarkenPattern(seconds));
                 });
    }

    private IEnumerator IEDarkenPattern(int seconds)
    {
        isPatternReveal = true;
        patternRevealTimeRemaining = seconds;

        // Countdown the reveal effect, respecting the game's time scale
        while (patternRevealTimeRemaining > 0.0F)
        {
            // We only apply the cooldown when the game isnt paused
            if (Time.timeScale > 0.0F)
            {
                patternRevealTimeRemaining -= Time.deltaTime;
            }
            yield return null;
        }
        
        ConcealPattern();
        isPatternReveal = false; // Resume the main timer once the freeze ends
    }

    #endregion POWERUP_EFFECTS_REVEAL_PATTERN
}