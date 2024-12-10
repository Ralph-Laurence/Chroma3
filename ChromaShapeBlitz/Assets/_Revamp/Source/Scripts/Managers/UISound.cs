using UnityEngine;

public class UISound : BaseAudioManager
{
    #region SINGLETON
    public static UISound Instance {get; private set;}

    public override void Initialize()
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

    [SerializeField] private AudioClip uxClickPositive;
    [SerializeField] private AudioClip uxClickNegative;

    public void PlayUxPositiveClick() => Audio.PlayOneShot(uxClickPositive);
    public void PlayUxNegativeClick() => Audio.PlayOneShot(uxClickNegative);
    public void PlayUISfx(AudioClip clip) => Audio.PlayOneShot(clip);
}