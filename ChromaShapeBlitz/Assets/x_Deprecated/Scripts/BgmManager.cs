using System;
using UnityEngine;

[Obsolete("Please use BackgroundMusic class instead.")]
public class BgmManager : MonoBehaviour
{
    private AudioSource bgmSource;

    [Space(5)]
    [Header("UI / Menu Musics")]
    [SerializeField] private AudioClip mainMenu;
    [SerializeField] private AudioClip pauseMenu;

    [Space(5)]
    [Header("Level Musics")]
    [SerializeField] private AudioClip[] stageThemes;
    // Begin Singleton
    //
    private static BgmManager _instance;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        else if (_instance != this)
            Destroy(gameObject);


        TryGetComponent(out bgmSource);
    }

    public static BgmManager Instance => _instance;
    //
    // End Singleton
    //

    public void PlayBgm()
    {
        if (bgmSource != null)
            bgmSource.Play();
    }

    public void StopBgm(bool clear = false)
    {
        if (bgmSource != null && bgmSource.isPlaying)
            bgmSource.Stop();

        if (clear)
            bgmSource.clip = null;
    }

    public void PauseBgm()
    {
        if (bgmSource != null && bgmSource.isPlaying)
            bgmSource.Pause();
    }
    
    public void ResumeBgm()
    {
        if (bgmSource != null && !bgmSource.isPlaying)
            bgmSource.UnPause();
    }

    public void SetClip(AudioClip audioClip)
    {
        if (bgmSource != null && bgmSource.clip != audioClip)
        {
            // Force stop the bgm and remove its current clip
            StopBgm(true);

            bgmSource.clip = audioClip;
        }
    }

    public void PlayMainMenu()
    {
        SetClip(mainMenu);
        PlayBgm();
    }

    public void PlayStageTheme(int themeIndex)
    {
        SetClip(stageThemes[themeIndex]);
        PlayBgm();
    }

    /// <summary>
    /// Force stop the bgm from playing, clear its audio clip then
    /// destroy its instance, including its attached gameobject.
    /// </summary>
    public void Shutdown()
    {
        if (_instance != null)
        {
            StopBgm(true );

            Destroy(_instance.gameObject);
            _instance = null;
        }
    }

    //=========================================================
    //................ AUDIO SOURCE CONTROLS ..................
    //=========================================================
    public void Mute()   => bgmSource.mute = true;
    public void UnMute() => bgmSource.mute = false;
    public bool IsMuted() => bgmSource.mute;
}
