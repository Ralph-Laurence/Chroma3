using System;
using UnityEngine;

[Obsolete]
public class SplashAnimation : MonoBehaviour
{
    [SerializeField] private AudioClip letterVisibleSfx;
    [SerializeField] private AudioClip bigTextVisibleSfx;

    private AudioSource sfx;
    private Animator animator;

    public Action OnAnimationEndCallback;

    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out sfx);
        TryGetComponent(out animator);
    }

    public void OnLetterShown()
    {
        if (sfx != null) 
            sfx.PlayOneShot(letterVisibleSfx);
    }

    public void OnBigTextShown()
    {
        if (sfx != null)
            sfx.PlayOneShot(bigTextVisibleSfx);
    }

    public void BeginAnimation() => animator.Play(Constants.SplashAnimationState);
    public void OnAnimationEnd() => OnAnimationEndCallback?.Invoke();
}
