using UnityEngine;

[System.Obsolete]
public class ControlButtonAnimation : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private AudioClip buttonShownSfx;

    // Start is called before the first frame update
    void Start() => TryGetComponent(out animator);

    public void OnControlButtonShown() => SfxManager.Instance.PlaySfx(buttonShownSfx);

    public void BeginAnimation() => animator.Play(Constants.ControlButtonAnimationState);
}
