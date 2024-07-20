using System;
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

    private SoundEffects sfx;
    private BackgroundMusic bgm;
    
    public Action OnTimesUp;
    private int duration;
    private float remainingTime;
    private float elapsedTime;
    private bool isStopped;
    //private bool isPaused;

    private float lastTickPlayedSecond;

    void Awake()
    {
        sfx = SoundEffects.Instance;
        bgm = BackgroundMusic.Instance;
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

            // Tone the bgm down to hear the countdown
            if (remainingSecs == 6)
                bgm.ToneDown();

            // Play the critical tick sound
            if (remainingTime <= 5.0F && remainingTime > 0 && lastTickPlayedSecond != remainingTime)
            {
                PlayClip(countdownSfx);
                lastTickPlayedSecond = remainingTime;
            }

            // Check if the timer has reached zero
            if (remainingTime <= 0.0F)
            {
                Stop();
                InvokeTimesUp();
            }
        }
    }
    /// <summary>
    /// Elapsed seconds refer to the amount of time that passes from the start of an activity to its end. 
    /// The formula to find the elapsed time is as follows: Elapsed time = End time – Start time.
    /// </summary>
    public int ElapsedSeconds => Mathf.FloorToInt(duration - remainingTime);

    // Start is called before the first frame update
    public void Begin()
    {
        isStopped = false;
        ResetBgmVolume();
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

    private void InvokeTimesUp()
    {
        isStopped = true;
        PlayClip(timesUpSfx);

        timesUpOverlay.SetActive(true);
        LeanTween.scale(timesUpCaption, Vector3.one * 1.35F, 0.25F)
            .setEaseInBounce()
            .setDelay(0.5F)
            .setOnComplete(() =>
            {
                ResetBgmVolume();
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
