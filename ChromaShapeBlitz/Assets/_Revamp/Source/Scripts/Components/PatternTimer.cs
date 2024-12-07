using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class PatternTimer : MonoBehaviour
{
    [SerializeField] private RectTransform effector;

    [SerializeField] private Image patternPreviewer;
    [SerializeField] private GameObject timesUpOverlay;
    [SerializeField] private RectTransform timesUpCaption;
    [SerializeField] private AudioClip countdownSfx;
    [SerializeField] private AudioClip timesUpSfx;
    [SerializeField] private Image darkPreview;
    [SerializeField] private RectTransform darkMaskRect;
    private Vector2 darkMaskInitialSize;

    [Space(5)] 
    [Header("Timer Logic")]
    [SerializeField] private Image timerFill;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject timerBadge;
    [SerializeField] private Color normalTimerFillColor     = new(1.00F, 0.73F, 0.00F, 1.0F);
    [SerializeField] private Color normalTimerBadgeColor    = new(0.41F, 1.0F, 0.0F, 1.0F);
    [SerializeField] private Color normalTimerTextColor     = Color.white;
    [SerializeField] private Color criticalTimerBadgeColor  = new(1.0F, 0.74F, 0.13F, 1.0F);
    [SerializeField] private Color criticalTimerTextColor   = new(1.0F, 0.14F, 0.23F, 1.0F);
    
    private RectTransform timerTextRect;
    private RectTransform timerBadgeRect;

    private SoundEffects sfx;
    private BackgroundMusic bgm;
    
    public Action OnTimesUp;
    private int duration;
    private float remainingTime;
    private float elapsedTime;
    private bool isStopped;

    private bool pulseTextBegan;
    private readonly int CriticalSecOffset = 5;
    private readonly float TimerTextMaxPulseScale = 1.5F;
    private readonly float TimerTextPulseDuration = 0.5F;
    
    private float lastTickPlayedSecond;

    private LTDescr tweenBlackenPattern;
    private int tweenID_blackenPattern;

    void Awake()
    {
        sfx = SoundEffects.Instance;
        bgm = BackgroundMusic.Instance;

        timerText.TryGetComponent(out timerTextRect);
        timerBadge.TryGetComponent(out timerBadgeRect);

        darkMaskInitialSize = darkMaskRect.sizeDelta;
        
        InitializePowerupEffector();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.K))
        {
            //isEMPBrownout = !isEMPBrownout;
            HotbarPowerupEffectNotifier.NotifyObserver(null, new PowerupEffectData
            {
                Category    = PowerupCategories.TimerPause,
                EffectValue = Constants.PowerupEffectValues.POWERUP_EFFECT_EMP
            });
            
            Debug.Log($"Brownout -> {isEMPBrownout}");
        }

        if (isEMPBrownout || isStopped || isTimeFreeze)
            return;

        remainingTime -= Time.deltaTime;

        // Clamp the remaining time to be between 0 and duration
        remainingTime = Mathf.Clamp(remainingTime, 0, duration);

        // Update the fill amount of the image smoothly
        timerFill.fillAmount = remainingTime / duration;

        elapsedTime += Time.deltaTime;

        // Check if a full 1 second has passed
        if (elapsedTime >= 1f)
        {
            // Decrease the whole seconds remaining time by 1
            elapsedTime -= 1f; // Subtract one second from elapsed time
            
            // Update timer label
            var remainingSecs = Mathf.CeilToInt(remainingTime);
            timerText.text = remainingSecs.ToString();

            // Tone the bgm down to hear the countdown sound
            if (remainingSecs == CriticalSecOffset + 1)
            {
                bgm.ToneDown();
                pulseTextBegan = true;
            }

            // Play the critical tick sound
            if (remainingTime <= CriticalSecOffset)
            {
                // Keep playing the critical tick sound until the remaining 1second
                if (remainingTime > 0 && lastTickPlayedSecond != remainingTime)
                {
                    PlayClip(countdownSfx);
                    lastTickPlayedSecond = remainingTime;
                }

                // Pulse the timer text even if it reached zero
                if (pulseTextBegan && remainingTime >= 0)
                    StartCoroutine(PulsateTimerText());
            }

            // Check if the timer has reached zero
            if (remainingTime <= 0.0F)
            {
                Stop();
                InvokeTimesUp();

                ResetTimerText();
            }
        }
    }
    /// <summary>
    /// Elapsed seconds refer to the amount of time that passes from the start of an activity to its end. 
    /// The formula to find the elapsed time is as follows: Elapsed time = End time – Start time.
    /// </summary>
    public int ElapsedSeconds => Mathf.FloorToInt(duration - remainingTime);

    // Start is called before the first frame update
    public void Begin(bool blackenPattern = false)
    {
        ResetTimerText();

        isTimeFreeze = false;
        isStopped    = false;

        ResetBgmVolume();
        
        // Blackening the pattern is used in Hard Mode
        if (blackenPattern)
            BlackenPattern();

        Debug.Log($"Brownout Status -> {isEMPBrownout}");
    }

    public void Stop() 
    {
        isStopped = true;
            
        ResetBgmVolume();
    }

    public void Prepare(int duration, Sprite patternObjective)
    {
        this.duration           = duration;
        remainingTime           = duration;
        timerFill.fillAmount    = 1.0F;
        elapsedTime             = 0.0F;

        lastTickPlayedSecond    = Mathf.CeilToInt(remainingTime);

        patternPreviewer.sprite = patternObjective;
        var remainingSecs       = Mathf.CeilToInt(remainingTime);
        timerText.text          = remainingSecs.ToString();

        // The pattern darkener must have the same image as the previewer
        darkPreview.sprite      = patternObjective;
        ResetDarkMask();

        // Always hide the times up overlay.
        // Sometimes the timesup and success screen show at once
        // during close-call success
        timesUpOverlay.SetActive(false);
    }
    
    private IEnumerator PulsateTimerText()
    {
        var animationCompleted = false;

        // Store the original scale
        var originalScale = Vector3.one;

        var pulseDuration = TimerTextPulseDuration / 2.0F;

        timerText.color = criticalTimerTextColor;

        StartCoroutine(PulsateTimerBadge(pulseDuration));

        // Pulse effect: scale up and then back to original
        LeanTween.scale(timerTextRect, originalScale * TimerTextMaxPulseScale, pulseDuration)
                 .setEase(LeanTweenType.easeOutQuad).setOnComplete(() =>
                 {
                    LeanTween.scale(timerTextRect, originalScale, pulseDuration)
                             .setEase(LeanTweenType.easeInQuad)
                             .setOnComplete(() => {
                                timerText.color = normalTimerTextColor;
                                animationCompleted = true;
                             });
                 });

        // Wait until the animation completes
        yield return new WaitUntil(() => animationCompleted);
    }

    private void ResetTimerText()
    {
        timerBadge.TryGetComponent(out Image image);

        if (image == null)
            return;

        image.color = normalTimerBadgeColor;
        timerText.color = normalTimerTextColor;
        timerTextRect.localScale = Vector3.one;
        pulseTextBegan = false;
    }

    private IEnumerator PulsateTimerBadge(float duration)
    {
        var animationCompleted = false;

        // Pulsate text
        var normalBadgeColor = new Action(() => {
            LeanTween.color(timerBadgeRect, normalTimerBadgeColor, duration)
                 .setEase(LeanTweenType.easeOutQuad)
                 .setOnComplete(() => animationCompleted = true);
        });

        // Pulsate badge
        LeanTween.color(timerBadgeRect, criticalTimerBadgeColor, duration)
                 .setEase(LeanTweenType.easeOutQuad)
                 .setOnComplete(() => {
                    normalBadgeColor.Invoke();
                 });

        yield return new WaitUntil(() => animationCompleted);
    }

    private void BlackenPattern(float fadeOutDelay = 3.0F, float duration = 0.35F)
    {
        var blinkCount  = 2;
        var from        = Constants.ColorSwatches.TRANSPARENT;
        var to          = Color.black;

        var finalColor  = new Action(() => {
            LeanTween.value
            (
                darkPreview.gameObject, 
                (color) => darkPreview.color = color, 
                from, 
                to, 
                duration
            );
        });

        tweenBlackenPattern?.reset();
        tweenBlackenPattern = LeanTween.value(darkPreview.gameObject, CallbackDarkPreview, from, to, duration)
                .setDelay(fadeOutDelay)
                .setEase(LeanTweenType.easeInQuad)
                .setLoopPingPong(blinkCount)
                .setOnComplete(() => {
                    darkPreview.color = to;
                });
                 
        tweenID_blackenPattern = tweenBlackenPattern.id;
    }

    private void CallbackDarkPreview(Color color) => darkPreview.color = color; 

    private void ResetDarkMask(bool resetRevealState = true)
    {
        darkPreview.color       = Constants.ColorSwatches.TRANSPARENT;
        darkMaskRect.sizeDelta  = darkMaskInitialSize;
        darkMaskRect.anchoredPosition = Vector2.zero;
        patternLaser.SetActive(false);
        
        if (resetRevealState)
        {
            isPatternReveal = false;
            isPermanentReveal = false;
        }
    }
    public int GetRemainingSecs() => Mathf.CeilToInt(remainingTime);
    private void InvokeTimesUp()
    {
        isStopped = true;
        PlayClip(timesUpSfx);
        bgm.Stop();
        ResetBgmVolume();
        
        timesUpOverlay.SetActive(true);
        LeanTween.scale(timesUpCaption, Vector3.one * 1.35F, 0.25F)
            .setEaseInBounce()
            .setDelay(0.5F)
            .setOnComplete(() =>
            {
                timesUpOverlay.SetActive(false);
                OnTimesUp?.Invoke();
            });
    }

    private void PlayClip(AudioClip clip)
    {
        if (sfx != null)
            sfx.PlayOnce(clip);
    }

    private void ResetBgmVolume()
    {
        if (bgm != null)
            bgm.ResetVolume();
    }
}
