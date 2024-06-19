using UnityEngine;

[System.Obsolete("Please use SoundEffects class instead.")]
public class SfxManager : MonoBehaviour
{
    private AudioSource soundFxSource;

    [Space(5)]
    [Header("Stage Sounds")]
    [SerializeField] private AudioClip blockFilledSfx;

    [Space(5)]
    [Header("UX Interactions")]
    [SerializeField] private AudioClip uxClickPositive;
    [SerializeField] private AudioClip uxClickNegative;

    [Space(5)]
    [Header("Game Over Sounds")]
    [SerializeField] private AudioClip[] successClips;
    [SerializeField] private AudioClip[] failureClips;
    [SerializeField] private AudioClip rewardsDialogShown;
    [SerializeField] private AudioClip skipLevelPop;

    [Space(5)]
    [Header("Rewards Screen")]
    [SerializeField] private AudioClip confettiSfx;
    [SerializeField] private AudioClip starFillSfx;
    [SerializeField] private AudioClip fullStarSfx;

    //
    // Begin Singleton
    //
    private static SfxManager _instance;

    private float _volume;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        else if (_instance != this)
            Destroy(gameObject);

        TryGetComponent(out soundFxSource);

        _volume = soundFxSource.volume;
    }

    public static SfxManager Instance => _instance;
    //
    // End Singleton
    //

    /// <summary>
    /// Play a single audio clip
    /// </summary>
    /// <param name="audioClip">The sound to play</param>
    public void PlaySfx(AudioClip audioClip)
    {
        if (soundFxSource != null)
            soundFxSource.PlayOneShot(audioClip);
    }

    /// <summary>
    /// Play a random audio clip
    /// </summary>
    /// <param name="clips">The random audio clips source</param>
    public void PlayRandomSfx(AudioClip[] clips)
    {
        int clipIndex = Random.Range(0, clips.Length);
        AudioClip clip = clips[clipIndex];

        PlaySfx(clip);
    }

    //=========================================================
    //.................UI INTERACTION SOUNDS ..................
    //=========================================================
    public void PlayUxPositiveClick() => PlaySfx(uxClickPositive);
    public void PlayUxNegativeClick() => PlaySfx(uxClickNegative);

    //=========================================================
    //................... GAME OVER SOUNDS ....................
    //=========================================================
    public void PlaySuccessSfx() => PlayRandomSfx(successClips);
    public void PlayFailureSfx() => PlayRandomSfx(failureClips);

    public void PlayRewardsDialogShown() => PlaySfx(rewardsDialogShown);

    public void PlayConfettiSfx() => PlaySfx(confettiSfx);
    public void PlayStarFill()    => PlaySfx(starFillSfx);
    public void PlayFullStar()    => PlaySfx(fullStarSfx);
    public void PlaySkipLevelPop() => PlaySfx(skipLevelPop);

    //=========================================================
    //.................LEVEL / STAGE SOUNDS ...................
    //=========================================================
    public void PlayBlockFilled() => PlaySfx(blockFilledSfx);

    //=========================================================
    //................ AUDIO SOURCE CONTROLS ..................
    //=========================================================
    public void Mute()   => soundFxSource.mute = true;
    public void UnMute() => soundFxSource.mute = false;
    public bool IsMuted() => soundFxSource.mute;

    //=========================================================
    //................... BEHAVIOUR LOGIC .....................
    //=========================================================
    /// <summary>
    /// Force stop the bgm from playing, clear its audio clip then
    /// destroy its instance, including its attached gameobject.
    /// </summary>
    public void Shutdown()
    {
        if (_instance != null)
        {
            Stop(true);
            
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }

    public void Stop(bool clearClip = false)
    {
        soundFxSource.Stop();

        if (clearClip)
           soundFxSource.clip = null;
    }
}
