using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public abstract class BaseAudioManager : MonoBehaviour
{
    private AudioSource audioSource;

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

    public void Mute()   => audioSource.mute = true;
    public void UnMute() => audioSource.mute = false;
    public void SetMute(bool mute) => audioSource.mute = mute;
    
    public void Loop(bool loop) => audioSource.loop = loop;

    public virtual void Initialize()
    {
        TryGetComponent(out audioSource);
    }

    void Awake() => Initialize();
}