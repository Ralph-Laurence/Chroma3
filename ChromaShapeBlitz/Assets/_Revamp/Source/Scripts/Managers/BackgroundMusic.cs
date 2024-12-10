using System.Collections;
using UnityEngine;

public class BackgroundMusic : BaseAudioManager
{
    #region SINGLETON
    public static BackgroundMusic Instance {get; private set;}

    void Awake()
    {
        base.Initialize();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // Assume that this component will be attached to a gameobject
            // that is a child of SoundBank gameobject
            //if (transform.parent == null || !transform.parent.name.Equals(SoundBank.Instance.GetName()))
            //    DontDestroyOnLoad(gameObject);
        }

        else if (Instance != this)
            Destroy(gameObject);
    }
    #endregion SINGLETON

    [SerializeField] private AudioClip themeBgm;


    #region AUDIO_BACKEND
    public void Play() => Audio.Play();
    
    public void SetClip(AudioClip audioClip)
    {
        if (Audio.clip != audioClip)
        {
            // Force stop the bgm and remove its current clip
            Stop(true);
            Audio.clip = audioClip;
        }
    }
    #endregion AUDIO_BACKEND

    private bool toneDownBegan;
    private readonly float toneDownDuration = 1.5F;
    private readonly float toneDownTarget = 0.125F;

    public void ToneDown()
    {
        if (toneDownBegan)
            return;

        StartCoroutine(FadeOutVolume());
    }

    public void ResetVolume()
    {
        toneDownBegan = false;
        SetVolume(OriginalVolume);
    }

    private IEnumerator FadeOutVolume()
    {
        toneDownBegan = true;

        var currentVolume = GetVolume();
        var startTime = Time.time;

        while (Time.time < startTime + toneDownDuration)
        {
            float elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / toneDownDuration);
            var volume = Mathf.Lerp(currentVolume, toneDownTarget, t);
            SetVolume(volume);

            yield return null;
        }

        // Ensure final volume is set correctly
        SetVolume(toneDownTarget);
    }
    /// <summary>
    /// The game's main background theme
    /// </summary>
    public void PlayMainBgm()
    {
        SetClip(themeBgm);
        Play();
    }
    
    /// <summary>
    /// Tell if the BGM is playing
    /// </summary>
    public bool IsPlaying => Audio.isPlaying;
}