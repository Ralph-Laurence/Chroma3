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
            
            // Assume that this component will be attached to a gameobject
            // that is a child of SoundBank gameobject
            if (transform.parent == null || !transform.parent.name.Equals(SoundBank.Instance.GetName()))
                DontDestroyOnLoad(gameObject);
        }

        else if (Instance != this)
            Destroy(gameObject);
    }
    #endregion SINGLETON

    [SerializeField] private AudioClip themeBgm;

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

    /// <summary>
    /// The game's main background theme
    /// </summary>
    public void PlayMainBgm()
    {
        SetClip(themeBgm);
        Play();
    }
}