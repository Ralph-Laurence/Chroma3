using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Obsolete]
public class TimerTheme
{
    public Color BlueTint       = new Color(0.0F    ,0.18F  ,1.0F   ,0.502F);
    public Color GreenTint      = new Color(0.0F    ,0.61F  ,0.42F  ,0.509F);
    public Color OrangeTint     = new Color(0.89F   ,0.41F  ,0.0F   ,0.502F);
    public Color PurpleTint     = new Color(0.45F   ,0.16F  ,0.88F  ,0.502F);
    public Color MagentaTint    = new Color(0.83F   ,0.16F  ,0.27F  ,0.502F);
    public Color YellowTint     = new Color(0.62F   ,0.50F  ,0.09F  ,0.502F);
}

[Obsolete]
public class SliderTimer : MonoBehaviour
{
    [Space(5)]
    [Header("Timer UI")]
    [SerializeField] private Image slider; // Assign your slider in the inspector
    [SerializeField] private TextMeshProUGUI secondsText;

    [Space(5)]
    [Header("Timer Sounds")]
    [SerializeField] private AudioClip criticalTick;
    [SerializeField] private AudioClip lowSecondsBeep;
    [SerializeField] private AudioClip timesUp;
    private AudioSource sfx;

    /// <summary>
    /// These seconds will trigger the timer sounds upon reaching
    /// </summary>
    [Space(5)]
    [Header("Sounds Trigger")]
    [SerializeField] private int criticalSeconds = 5;
    [SerializeField] private int lowSeconds = 3;
    [SerializeField] private int duration = 10; // The countdown time in seconds

    // [Space(5)]
    // public TimerTheme 

    [Space(5)]
    [Header("Behaviours")]
    [SerializeField] private bool beginOnLoad = false;
    private bool isCriticalSoundPlayed = false;
    private Animator animator;
    private bool isStopped;

    public int ElapsedSeconds {get; private set;}

    /// <summary>
    /// Event when the timer began counting.
    /// </summary>
    public Action OnTimerStarted;

    /// <summary>
    /// Event when the timer was intentionally stopped.
    /// The TimerFinished won't execute this time.
    /// </summary>
    public Action OnTimerStopped;

    /// <summary>
    /// Event when the timer has finished the countdown to 0.
    /// This is called when it is not intentionally stopped.
    /// </summary>
    public Action OnTimerFinished;

    /// <summary>
    /// Event when the timer is finished, and just before flashing
    /// </summary>
    public Action OnTimerBeforeFlash;

    public void SetSeconds(int seconds) => duration = seconds;
    public void Begin() => StartCoroutine(StartCountdown());
    public void Stop() 
    {
        isStopped = true;

        if (sfx != null)
        {
            sfx.Stop();
        }
    }

    private void Awake()
    {
        TryGetComponent(out sfx);
        TryGetComponent(out animator);
    }

    private void Start()
    {
        if (beginOnLoad)
            Begin();
    }

    /// <summary>
    /// Methods prefixed with "AV_*" are animation events
    /// </summary>
    public void AV_PlayTimesUpSfx()
    {
        sfx.Stop();
        sfx.PlayOneShot(timesUp);
    }

    public void AV_AnimationComplete() => OnTimerFinished?.Invoke();

    private IEnumerator StartCountdown()
    {
        ElapsedSeconds = 0;
        OnTimerStarted?.Invoke();

        float remainingTime = duration;     // This is for computation
        var lastBeepSecs    = 0;            // Get the second when the beep was last played

        while (remainingTime > 0 && !isStopped)
        {
            // Update the slider's value
            slider.fillAmount = remainingTime / duration;

            // Upon reaching less than 1, immediately stop
            if (remainingTime < 0.5F)
            {
                remainingTime    = 0;
                secondsText.text = "0";
                break;
            }

            // Update the display
            var remainingSecs = Mathf.RoundToInt(remainingTime);
            secondsText.text = remainingSecs.ToString();

            ElapsedSeconds = duration - remainingSecs;

            if (sfx != null)
            {
                if (remainingSecs == criticalSeconds && !isCriticalSoundPlayed)
                {
                    isCriticalSoundPlayed = true;
                    sfx.PlayOneShot(criticalTick);
                }

                if ((remainingSecs <= lowSeconds && remainingSecs > 0) && lastBeepSecs != remainingSecs)
                {
                    sfx.PlayOneShot(lowSecondsBeep);
                    lastBeepSecs = remainingSecs;
                }
            }

            // Wait for the next frame
            yield return null;

            // Decrease the remaining time
            remainingTime -= Time.deltaTime;
        }

        if (isStopped)
            OnTimerStopped?.Invoke();
        else
        {
            OnTimerEndedNotifier.Publish();
            
            // Ensure the slider is empty at the end of the countdown,
            slider.fillAmount = 0;

            OnTimerBeforeFlash?.Invoke();

            animator.Play("Times Up Flash");
        }
    }
}