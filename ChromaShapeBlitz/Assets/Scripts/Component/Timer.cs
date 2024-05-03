using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Image              outerFill;
    [SerializeField] private Image              innerFill;
    [SerializeField] private Image              timesUpFlash;
    [SerializeField] private TextMeshProUGUI    uiText;
    [SerializeField] private AudioClip          countdownTick;
    [SerializeField] private AudioClip          timesUp;
    [SerializeField] private float              timesUpFlashDuration = 1.0F;
    [SerializeField]
    [Range(1.0F, 255.0F)]
    private float timesUpFlashIntensity = 85.0F;

    [SerializeField] private Color outerFillColor   = new Color(0.58F, 0.58F, 0.58F, 0.47F);  // Gray
    [SerializeField] private Color startingColor    = new Color(0.31F, 0.25F, 0.78F, 0.47F);  // Purple
    [SerializeField] private Color halfTimeColor    = new Color(1.00F, 0.73F, 0.00F, 0.47F);  // Yellow
    [SerializeField] private Color criticalColor    = new Color(1.00F, 0.43F, 0.13F, 0.47F);  // Orange
    [SerializeField] private Color timesUpColor     = new Color(1.00F, 0.15F, 0.25F, 0.47F);  // Red

    [SerializeField] private int criticalValue = 3;

    private AudioSource audioSfx;

    public int Duration;
    private int remainingDuration;
    private int lastRemainingDuration;
    private int elapsedSeconds;

    public bool Pause;
    public bool Stopped;
    public bool OnlySeconds = true;

    public Action OnTimerEnd;
    public Action OnTimerStopped;
    public Action OnTimerZero;

    private WaitForSeconds wait;

    private void Start()
    {
        wait = new WaitForSeconds(1);

        TryGetComponent(out audioSfx);

        ResetColors();
    }

    private void ResetColors()
    {
        outerFill.color = outerFillColor;
        innerFill.color = startingColor;
        uiText.color    = startingColor;
    }

    public void Begin()
    {
        lastRemainingDuration = 0;
        elapsedSeconds = 0;
        remainingDuration = Duration;
        StartCoroutine(UpdateTimer());
    }

    public void Stop()
    {
        Stopped               = true;
        uiText.text           = "0";
        innerFill.fillAmount  = 1;
        lastRemainingDuration = remainingDuration;
        elapsedSeconds        = Duration - remainingDuration;
        remainingDuration     = 0;

        ResetColors();

        OnTimerStopped?.Invoke();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void SetDuration(int duration) => Duration = duration;
    public int GetLastSeconds() => lastRemainingDuration;
    public int GetElapsedSeconds() => elapsedSeconds;

    private IEnumerator UpdateTimer()
    {
        var fillAmount = innerFill.fillAmount;

        while (remainingDuration >= 0 && !Stopped)
        {
            if (!Pause)
            {
                // Formatted Time String Text
                if (!OnlySeconds)
                    uiText.text = $"{remainingDuration / 60:00}:{remainingDuration % 60:00}";

                // Unformatted Time String
                else
                    uiText.text = remainingDuration.ToString();

                // Critical countdown Tick
                if (remainingDuration > 0 && remainingDuration <= criticalValue)
                {
                    innerFill.color = criticalColor;
                    uiText.color    = criticalColor;

                    audioSfx.PlayOneShot(countdownTick);
                }

                // Halfway countdown Tick
                else if (remainingDuration > criticalValue && remainingDuration <= Duration / 2)
                {
                    innerFill.color = halfTimeColor;
                    uiText.color    = halfTimeColor;
                }

                // Time's up
                else if (remainingDuration == 0)
                {
                    if (OnTimerZero != null)
                        OnTimerZero.Invoke();

                    outerFill.color = timesUpColor;
                    innerFill.color = timesUpColor;
                    uiText.color    = timesUpColor;

                    audioSfx.volume = 1.0F;         // max out the volume to hear the bass drop
                    audioSfx.PlayOneShot(timesUp);

                    yield return StartCoroutine(FlashTimesUp());
                }

                // Update progress bar
                // innerFill.fillAmount = Mathf.InverseLerp(0, Duration, remainingDuration);
                var targetFillAmount = Mathf.InverseLerp(0, Duration, remainingDuration);
                fillAmount = Mathf.Lerp(fillAmount, targetFillAmount, Time.deltaTime);

                innerFill.fillAmount = fillAmount;
                
                // Subtract 1 second
                remainingDuration--;

                // Calculate elapsed time
                elapsedSeconds = Duration - remainingDuration;
                Debug.Log(elapsedSeconds);

                // Every second
                yield return wait;
            }
            yield return null;
        }

        if (!Stopped)
        {
            lastRemainingDuration = remainingDuration;
            OnTimerEnd?.Invoke();
        }
    }

    private IEnumerator FlashTimesUp()
    {
        var time        = 0.0F;
        var startAlpha  = 0.0F;
        var endAlpha    = timesUpFlashIntensity / 255.0F;

        timesUpFlash.gameObject.SetActive(true);

        while (time < timesUpFlashDuration)
        {
            var alpha = Mathf.Lerp(startAlpha, endAlpha, time / timesUpFlashDuration);

            timesUpFlash.color = new Color(
                timesUpFlash.color.r,
                timesUpFlash.color.g,
                timesUpFlash.color.b,
                alpha
            );

            time += Time.deltaTime;
            yield return null;
        }

        // Lerp in reverse.
        time = 0;

        while (time < timesUpFlashDuration)
        {
            var alpha = Mathf.Lerp(endAlpha, startAlpha, time / timesUpFlashDuration);

            timesUpFlash.color = new Color(
                timesUpFlash.color.r,
                timesUpFlash.color.g,
                timesUpFlash.color.b,
                alpha
            );

            time += Time.deltaTime;
            yield return null;
        }

        timesUpFlash.gameObject.SetActive(false);
    }
}
