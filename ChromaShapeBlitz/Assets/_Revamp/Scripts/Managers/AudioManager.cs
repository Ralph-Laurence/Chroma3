using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class BaseAudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    private float originalVolume;

    public void Pause()
    {
        if (audioSource.isPlaying)
            audioSource.Pause();
    }
    
    public void Resume()
    {
        if (!audioSource.isPlaying)
            audioSource.UnPause();
    }

    public void Stop(bool clearClip = true)
    {
        audioSource.Stop();

        if (clearClip)
           audioSource.clip = null;
    }

    public AudioSource Audio => audioSource;

    public float OriginalVolume => originalVolume;
    public float GetVolume() => audioSource.volume;
    public void SetVolume(float volume) => audioSource.volume = volume;
    public void Mute()   => audioSource.mute = true;
    public void UnMute() => audioSource.mute = false;
    public void SetMute(bool mute) => audioSource.mute = mute;
    
    public void Loop(bool loop) => audioSource.loop = loop;

    public virtual void Initialize()
    {
        TryGetComponent(out audioSource);
        originalVolume = audioSource.volume;
    }

    void Awake() => Initialize();
}