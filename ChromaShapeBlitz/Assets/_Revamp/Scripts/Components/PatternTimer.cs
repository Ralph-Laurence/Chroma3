using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PatternTimer : MonoBehaviour
{
    [SerializeField] private Image patternPreviewer;

    [Space(5)] 
    [Header("Timer Logic")]
    [SerializeField] private Image timerFill;
    [SerializeField] private TextMeshProUGUI timerText;

    private int duration;
    private bool isStopped;
    private bool isPaused;

    // Start is called before the first frame update
    public void Begin() => StartCoroutine(StartCountdown());
    public void Stop() => isStopped = true;
    public void Pause() => isPaused = true;
    public void Resume() => isPaused = false;

    public void Prepare(int duration, Sprite patternObjective)
    {
        this.duration = duration;
        patternPreviewer.sprite = patternObjective;
    }

    public void SetPattern(Sprite pattern) => patternPreviewer.sprite = pattern;

    /// <summary>
    /// Elapsed seconds refer to the amount of time that passes from the start of an activity to its end. 
    /// The formula to find the elapsed time is as follows: Elapsed time = End time – Start time.
    /// </summary>
    public int ElapsedSeconds { get; private set; }

    private IEnumerator StartCountdown()
    {
        float remainingTime = duration;
        float lastUpdateTime = Time.realtimeSinceStartup; // Use unscaled time for accuracy

        while (remainingTime > 0 && !isStopped)
        {
            // Handle paused state
            if (isPaused)
            {
                lastUpdateTime = Time.realtimeSinceStartup; // Reset the update time to prevent jumps
                yield return null;
                continue;
            }

            // Update the slider's value
            timerFill.fillAmount = remainingTime / duration;

            // Upon reaching less than 0.5, immediately stop
            if (remainingTime < 0.01F)
            {
                remainingTime = 0;
                timerText.text = "0";
                break;
            }

            // Update the display
            int remainingSecs = Mathf.CeilToInt(remainingTime);
            timerText.text = remainingSecs.ToString();

            ElapsedSeconds = duration - remainingSecs;

            // Wait for the next frame
            yield return null;

            // Calculate the time since the last update
            float currentTime = Time.realtimeSinceStartup;
            float deltaTime = currentTime - lastUpdateTime;
            lastUpdateTime = currentTime;

            // Decrease the remaining time
            remainingTime -= deltaTime;
        }

        // Ensure final update to zero state
        timerFill.fillAmount = 0;
        timerText.text = "0";
    }

    /*
    private IEnumerator StartCountdown()
    {
        float remainingTime = duration;     // This is for computation

        while (remainingTime > 0 && !isStopped)
        {
            if (isPaused)
                continue;

            // Update the slider's value
            timerFill.fillAmount = remainingTime / duration;

            // Upon reaching less than 1, immediately stop
            if (remainingTime < 0.5F)
            {
                remainingTime    = 0;
                timerText.text = "0";
                break;
            }

            // Update the display
            var remainingSecs = Mathf.RoundToInt(remainingTime);
            timerText.text = remainingSecs.ToString();

            ElapsedSeconds = duration - remainingSecs;

            // if (sfx != null)
            // {
            //     if (remainingSecs == criticalSeconds && !isCriticalSoundPlayed)
            //     {
            //         isCriticalSoundPlayed = true;
            //         sfx.PlayOneShot(criticalTick);
            //     }

            //     if ((remainingSecs <= lowSeconds && remainingSecs > 0) && lastBeepSecs != remainingSecs)
            //     {
            //         sfx.PlayOneShot(lowSecondsBeep);
            //         lastBeepSecs = remainingSecs;
            //     }
            // }

            // Wait for the next frame
            yield return null;

            // Decrease the remaining time
            remainingTime -= Time.deltaTime;
        }

        if (isStopped)
        {
            // OnTimerStopped?.Invoke();
        }
        else
        {
            OnTimerEndedNotifier.Publish();
            
            // Ensure the slider is empty at the end of the countdown,
            timerFill.fillAmount = 0;

            // OnTimerBeforeFlash?.Invoke();

            // animator.Play("Times Up Flash");
        }
    }*/
}
