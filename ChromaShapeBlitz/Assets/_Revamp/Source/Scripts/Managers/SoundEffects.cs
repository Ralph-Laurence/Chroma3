using UnityEngine;

public class SoundEffects : BaseAudioManager
{
    #region SINGLETON
    public static SoundEffects Instance {get; private set;}

    public override void Initialize()
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

    #region AUDIO_BACKEND
    /// <summary>
    /// Play an audio clip once
    /// </summary>
    public void PlayOnce(AudioClip audioClip) => Audio.PlayOneShot(audioClip);

    /// <summary>
    /// Play a random audio clip once
    /// </summary>
    /// <param name="clips">The random audio clips source</param>
    public void RandomOneShot(AudioClip[] clips)
    {
        int clipIndex = Random.Range(0, clips.Length);
        AudioClip clip = clips[clipIndex];

        PlayOnce(clip);
    }
    #endregion

    #region GameMechanics
    
    [Space(10)]
    [SerializeField] private AudioClip blockFilling;

    public void PlayBlockFill() => PlayOnce(blockFilling);
    #endregion
}
