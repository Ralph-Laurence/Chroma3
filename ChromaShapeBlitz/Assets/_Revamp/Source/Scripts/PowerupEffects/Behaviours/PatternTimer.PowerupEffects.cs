using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class PatternTimer : MonoBehaviour
{
    [Space(10)]
    [Header("Time Freeze Effect")]
    [SerializeField] private GameObject freezeEffectOverlayObj;
    [SerializeField] private GameObject frozenTimeLabelEffect;
    [SerializeField] private AudioClip  sfxThreeSecs;
    [SerializeField] private AudioClip  sfxFiveSecs;
    [SerializeField] private AudioClip  sfxUnfreeze;
    [SerializeField] private float      freezeEffectSpeed    = 4.0F;
    [SerializeField] private Color      frozenTimeBadgeColor = new(0.58F, 0.83F, 0.97F, 1.0F);
    [SerializeField] private Color      frozenTimerFillColor = new(0.04F, 0.32F, 0.99F, 1.0F);

    [SerializeField] private GameObject empTimerBadge;
    [SerializeField] private Color      empTimerFillColor    = new(0.47F, 0.25F, 1.00F, 1.0F);

    private readonly int EmpMissileMasUsages = 5;

    [Space(10)]
    [Header("Pattern Reveal Effect")]
    [SerializeField] private GameObject patternLaser;
    [SerializeField] private AudioClip  sfxRevealPattern;

    private readonly Color TRANSPARENT = Constants.ColorSwatches.TRANSPARENT;

    private Image freezeOverlay;
    private Image frozenTimeLabel;
    private Image frozenTimeBadge;
    private bool isTimeFreeze;
    private float freezeTimeRemaining;
    private bool isPatternReveal;
    private bool isPermanentReveal;
    private bool isEMPBrownout;

    private LTDescr freezeTimeTween;

    // The awake method
    private void InitializePowerupEffector()
    {
        freezeEffectOverlayObj.TryGetComponent(out freezeOverlay);
        freezeOverlay.enabled = false;

        timerBadge.TryGetComponent(out frozenTimeBadge);
        frozenTimeLabelEffect.TryGetComponent(out frozenTimeLabel);
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

                PowerupEffectAppliedNotifier.NotifyObserver(sender, effectData);

                // With cutscene effect
                if (effectData.EffectValue == Constants.PowerupEffectValues.POWERUP_EFFECT_EMP)
                {
                    SetBrownOut(true);

                    // While the cutscene plays, we update the user save data
                    var userData = GameSessionManager.Instance.UserSessionData;

                    userData.RemainingEMPUsage = EmpMissileMasUsages;
                    // Write the changes to file
                    StartCoroutine(UserDataHelper.Instance.SaveUserData(userData, null));

                    SceneManager.LoadSceneAsync(Constants.Scenes.CutSceneEMPAttack, LoadSceneMode.Additive);

                    return;
                }

                // Without cutscenes (in-game effects)
                var freezeDuration = effectData.EffectValue;

                StartCoroutine(IEFreezeTimer(freezeDuration));
                sender.BeginLockSlot(freezeDuration + 1);

                break;

            case PowerupCategories.TimerIncrease:
                
                var increase = effectData.EffectValue;

                AddTime(increase);
                
                PowerupEffectAppliedNotifier.NotifyObserver(sender, effectData);

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

                if (isPatternReveal || isPermanentReveal) return;
                    isPatternReveal = true;

                var seconds = effectData.EffectValue;

                RevealPatternForSeconds(seconds);
                PowerupEffectAppliedNotifier.NotifyObserver(sender, effectData);

                if (sender != null)
                    sender.BeginLockSlot(seconds);

                break;

            case PowerupCategories.SpecialVision:

                if (effectData.EffectValue == Constants.PowerupEffectValues.POWERUP_EFFECT_XRAY)
                {
                    if (isPatternReveal)
                        return;
                        
                    isPatternReveal = true;
                    RevealPatternImmediate();
                    
                    PowerupEffectAppliedNotifier.NotifyObserver(sender, effectData);
                }
                // if (effectData.EffectValue == Constants.PowerupEffectValues.POWERUP_EFFECT_VISOR)
                // # Handled in StageVariant.PowerupEffector
                
                break;
        }
    }
    #endregion EVENT_OBSERVERS
    
    #region POWERUP_EFFECTS_FREEZE_TIMER

    /// <summary>
    /// Just sets the boolean flag to "freeze"; No other behaviours are applied
    /// </summary>
    public void SetFlagFreezeTimer(bool freeze) => isTimeFreeze = freeze;
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

    /// <summary>
    /// Set the current status to brownout. The countdown wont execute when true.
    /// </summary>
    public void SetBrownOut(bool brownout)
    {
        isEMPBrownout = brownout;

        if (!isEMPBrownout)
        {
            empTimerBadge.SetActive(false);
            timerFill.color = normalTimerFillColor;
            return;
        }

        empTimerBadge.SetActive(true);
        timerFill.color = empTimerFillColor;
    }

    private void ShowFreezeEffect(int freezeSeconds)
    {
        var sound = sfxThreeSecs;

        if (freezeSeconds == 5)
            sound = sfxFiveSecs;

        freezeOverlay.enabled = true;
        sfx.PlayOnce(sound);

        var effectSpeed = freezeSeconds / freezeEffectSpeed;
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
                    var playUnfreeze = false;
                    
                    LeanTween.value(freezeEffectOverlayObj, SetFreezeOverlayColor, freezeOverlay.color, TRANSPARENT, effectSpeed)
                             .setDelay(delay)
                             .setOnUpdate((float v) => {
                                if (!playUnfreeze)
                                {
                                    playUnfreeze = true;
                                    sfx.PlayOnce(sfxUnfreeze);
                                }
                             })
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

    /// <summary>
    /// Reveal the previewer without animations
    /// </summary>
    private void RevealPatternImmediate()
    {
        if (isPermanentReveal)
            return;

        isPermanentReveal = true;

        // Interrup the tween
        LeanTween.cancel(tweenID_blackenPattern);

        // Hide the dark mask, but keep its revealed state
        ResetDarkMask(false);
    }

    /// <summary>
    /// Reveal the previewer by hiding the mask, for a given duration, 
    /// of course with animations
    /// </summary>
    private void RevealPatternForSeconds(int seconds, bool permanent = false)
    {
        patternLaser.SetActive(true);

        var initialHeight   = darkMaskRect.sizeDelta.y;
        var initialY        = darkMaskRect.anchoredPosition.y;
        var scaleDown       = 0.0F;
        var rate            = 0.75F;
        
        sfx.PlayOnce(sfxRevealPattern);

        var tween = LeanTween.value(darkMaskRect.gameObject, initialHeight, scaleDown, rate)
            .setOnUpdate((float value) =>
            {
                var sizeDelta   = darkMaskRect.sizeDelta;
                sizeDelta.y     = value;

                darkMaskRect.sizeDelta = sizeDelta;

                // Adjust the anchored position to maintain the bottom position
                darkMaskRect.anchoredPosition = new Vector2(
                    darkMaskRect.anchoredPosition.x,
                    initialY - (initialHeight - value) * 0.5F // Using 0.5 to move half the distance
                );

            })
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() => {
                
                // Only if powerup was applied
                if (!permanent)
                    StartCoroutine(IERedarkenPattern(seconds));
            });
    }

    /// <summary>
    /// Bring the pattern preview back to dark
    /// </summary>
    private IEnumerator IERedarkenPattern(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        Debug.Log($"isPermanentReveal at IERedarken -> {isPermanentReveal}");

        if (isPermanentReveal)
            yield break;

        var from = Constants.ColorSwatches.TRANSPARENT;
        var to   = darkPreview.color;

        ResetDarkMask(false);

        LeanTween.value(darkPreview.gameObject, CallbackDarkPreview, from, to, 0.25F)
                 .setOnComplete(() => isPatternReveal = false);
    }

    #endregion POWERUP_EFFECTS_REVEAL_PATTERN
}