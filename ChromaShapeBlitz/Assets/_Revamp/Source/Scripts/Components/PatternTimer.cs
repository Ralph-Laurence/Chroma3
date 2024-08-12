using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PatternTimer : MonoBehaviour
{
    [SerializeField] private RectTransform effector;

    [SerializeField] private Image patternPreviewer;
    [SerializeField] private GameObject timesUpOverlay;
    [SerializeField] private RectTransform timesUpCaption;
    [SerializeField] private AudioClip countdownSfx;
    [SerializeField] private AudioClip timesUpSfx;

    [Space(5)] 
    [Header("Timer Logic")]
    [SerializeField] private Image timerFill;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject timerBadge;
    [SerializeField] private Color normalTimerBadgeColor = new(0.41F, 1.0F, 0.0F, 1.0F);
    [SerializeField] private Color normalTimerTextColor = Color.white;
    [SerializeField] private Color criticalTimerBadgeColor = new(1.0F, 0.74F, 0.13F, 1.0F);
    [SerializeField] private Color criticalTimerTextColor = new(1.0F, 0.14F, 0.23F, 1.0F);

    private RectTransform timerTextRect;
    private RectTransform timerBadgeRect;

    private SoundEffects sfx;
    private BackgroundMusic bgm;
    
    public Action OnTimesUp;
    private int duration;
    private float remainingTime;
    private float elapsedTime;
    private bool isStopped;
    //private bool isPaused;

    private bool pulseTextBegan;
    private readonly int CriticalSecOffset = 5;
    private readonly float TimerTextMaxPulseScale = 1.5F;
    private readonly float TimerTextPulseDuration = 0.5F;

    private float lastTickPlayedSecond;

    void Awake()
    {
        sfx = SoundEffects.Instance;
        bgm = BackgroundMusic.Instance;

        timerText.TryGetComponent(out timerTextRect);
        timerBadge.TryGetComponent(out timerBadgeRect);
    }

    void Update()
    {
        if (isStopped)
            return;

        remainingTime -= Time.deltaTime;

        // Clamp the remaining time to be between 0 and duration
        remainingTime = Mathf.Clamp(remainingTime, 0, duration);

        // Update the fill amount of the image smoothly
        timerFill.fillAmount = remainingTime / duration;
//PositionEffector();
        elapsedTime += Time.deltaTime;

         // Check if a full second has passed
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

                pulseTextBegan = false;
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
        isStopped = false;
        ResetBgmVolume();
        
        // Reset the pattern color to default
        LightenPattern();
        
        // Blackening the pattern is used in Hard Mode
        if (blackenPattern)
            BlackenPattern();
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
    }

    public void SetPattern(Sprite pattern) => patternPreviewer.sprite = pattern;

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
        // Blink the pattern for 3 times before blackening it
        var blinkCount = 3;

        // Chain the blink animations using LeanTween
        var callback = new Action<Color>((color) => patternPreviewer.color = color);
        var finalColor = new Action(() => {
            LeanTween.value
            (
                patternPreviewer.gameObject, 
                (color) => patternPreviewer.color = color, 
                Color.white, 
                Color.black, 
                duration
            ).setOnComplete(() => {
                // Set the final color to black after blinking
                patternPreviewer.color = Color.black;
            });
        });

        LeanTween.value(patternPreviewer.gameObject, callback, Color.white, Color.black, duration)
                 .setDelay(fadeOutDelay)
                 .setEase(LeanTweenType.easeInQuad)
                 .setLoopPingPong(blinkCount)
                 .setOnComplete(() => finalColor.Invoke());
    }

    public void LightenPattern() => patternPreviewer.color = Color.white;

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

    void PositionEffector()
    {
        var radius = 60f;

        float fillAmount = timerFill.fillAmount;

        // Calculate the angle based on fill amount
        float angle = 360f * fillAmount;

        // Convert angle to radians
        float angleRad = angle * Mathf.Deg2Rad;

        // Calculate position on the circle
        float x = Mathf.Cos(angleRad) * radius;
        float y = Mathf.Sin(angleRad) * radius;

        // Adjust for the rounded corners
        if (fillAmount < 0.125f || (fillAmount > 0.375f && fillAmount < 0.625f) || (fillAmount > 0.875f && fillAmount < 1f))
        {
            // In the corner areas
            x *= Mathf.Sqrt(2) / 2;
            y *= Mathf.Sqrt(2) / 2;
        }

        // Update the follower position
        effector.localPosition = new Vector3(x, y, 0f);
        
        // float fillAmount = timerFill.fillAmount;
        // float angle = fillAmount * 360f;
        // float halfWidth = timerFill.rectTransform.rect.width / 2f;
        // float halfHeight = timerFill.rectTransform.rect.height / 2f;
        
        // Vector2 position = Vector2.zero;

        // if (angle <= 90f)
        // {
        //     // Top edge
        //     position.x = Mathf.Lerp(0, halfWidth, angle / 90f);
        //     position.y = halfHeight;
        // }
        // else if (angle <= 180f)
        // {
        //     // Right edge
        //     angle -= 90f;
        //     position.x = halfWidth;
        //     position.y = Mathf.Lerp(halfHeight, -halfHeight, angle / 90f);
        // }
        // else if (angle <= 270f)
        // {
        //     // Bottom edge
        //     angle -= 180f;
        //     position.x = Mathf.Lerp(halfWidth, -halfWidth, angle / 90f);
        //     position.y = -halfHeight;
        // }
        // else
        // {
        //     // Left edge
        //     angle -= 270f;
        //     position.x = -halfWidth;
        //     position.y = Mathf.Lerp(-halfHeight, halfHeight, angle / 90f);
        // }

        // position.x *= -1.0F;
        // // Set the position of the effector
        // effector.anchoredPosition = position;
    }
}
