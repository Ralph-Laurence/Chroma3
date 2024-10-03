using System.Collections;
using Revamp;
using UnityEngine;

public class WizardFx : MonoBehaviour
{
    [SerializeField] private GameObject character;
    [SerializeField] private GameObject[] wandIgnitionEffects;
    [SerializeField] private ParticleSystem onSpawnSmoke;
    [SerializeField] private ParticleSystem onMagicApplied;
    [SerializeField] private ParticleSystem onMagicExit;
    [SerializeField] private AudioClip sfxSpawn;
    [SerializeField] private AudioClip sfxIgniteWand;
    [SerializeField] private AudioClip sfxDoMagic;
    [SerializeField] private AudioClip sfxExit;
    [SerializeField] private AudioClip[] wizardLaughs;

    [HideInInspector] public StageVariant StageVariantEffectTarget;
    [HideInInspector] public System.Action OnEffectCompleted;

    private Animator animator;
    private SoundEffects sfx;

    void Awake()
    {
        sfx = SoundEffects.Instance;
        TryGetComponent(out animator);
    }

    #region EVENT_OBSERVER
    void OnEnable()
    {
        GameManagerEventNotifier.BindObserver(HandleGameManagerActionChanged);
    }

    void OnDisable()
    {
        GameManagerEventNotifier.UnbindObserver(HandleGameManagerActionChanged);
    }

    private void HandleGameManagerActionChanged(GameManagerActionEvents actionEvent)
    {
        // Detect changes to game manager action.
        // We will only subscribe to the "Retry" state
        if (actionEvent == GameManagerActionEvents.Retry)
            ForceStopEffect();
    }
    #endregion EVENT_OBSERVER

    // void Update()
    // {
    //     if (Input.GetKeyUp(KeyCode.F1))
    //     {
    //         if (onSpawnSmoke.isPlaying)
    //             return;

    //         StartCoroutine(SpawnTheWizard());
    //     }
    //     else if (Input.GetKeyUp(KeyCode.F2))
    //     {
    //         character.SetActive(false);
    //         onSpawnSmoke.Stop();
    //         onSpawnSmoke.gameObject.SetActive(false);
    //     }
    // }

    #region STATE_CONTROL
    public void BeginEffect()
    {
        InGameHotbarInteractionStateNotifier.NotifyObserver(blockInteraction: true);

        // Poisition the mage just above the stage variant
        var posAboveStage = StageVariantEffectTarget.transform.position;
        posAboveStage.y += 0.5F;

        transform.position = posAboveStage;

        StartCoroutine(SpawnTheWizard());
    }

    // Immediate stop without animation and sound effects.
    // Used when the powerup is applied but the player initiates a retry
    private void ForceStopEffect()
    {
        animator.StopPlayback();
        StageVariantEffectTarget = null;

        for (var i = 0; i < wandIgnitionEffects.Length; i++)
        {
            wandIgnitionEffects[i].SetActive(false);
        }

        character.SetActive(false);
        onSpawnSmoke.gameObject.SetActive(false);
        onMagicApplied.gameObject.SetActive(false);
        onMagicExit.gameObject.SetActive(false);

        InGameHotbarInteractionStateNotifier.NotifyObserver(blockInteraction: false);
        gameObject.SetActive(false);
    }
    #endregion STATE_CONTROL

    private IEnumerator SpawnTheWizard()
    {
        onSpawnSmoke.gameObject.SetActive(true);
        sfx.PlayOnce(sfxSpawn);

        yield return new WaitForSeconds(0.3F);
        character.SetActive(true);

        yield return new WaitForSeconds(1.0F);
        animator.SetTrigger("DoMagic");
    }

    private IEnumerator ToggleExtraEffects(bool toggle)
    {
        for (var i = 0; i < wandIgnitionEffects.Length; i++)
        {
            wandIgnitionEffects[i].SetActive(toggle);
            yield return null;
        }
    }

    private IEnumerator ExitMagicEffect()
    {
        character.SetActive(false);
           
        yield return StartCoroutine(ToggleExtraEffects(false));
        onMagicExit.gameObject.SetActive(true);
        sfx.PlayOnce(sfxExit);

        InGameHotbarInteractionStateNotifier.NotifyObserver(blockInteraction: false);
        OnEffectCompleted?.Invoke();
    }

    
    #region ANIMATION_EVENTS
    public void AnimEvt_OnChargeMagic()
    {
        StartCoroutine(ToggleExtraEffects(true));
        sfx.PlayOnce(sfxIgniteWand);
    }

    public void AnimEvt_OnApplyMagic()
    {
        onMagicApplied.gameObject.SetActive(true);
        sfx.PlayOnce(sfxDoMagic);

        // Turn off the first ignition effect which is the "crown"
        wandIgnitionEffects[0].SetActive(false);

        // Apply the effect
        StageVariantEffectTarget.ExecuteWizardEffect( () => animator.SetTrigger("FinishMagic"));
    }

    public void AnimEvt_OnMagicApplied()
    {
        var laugh = wizardLaughs[Random.Range(0, wizardLaughs.Length)];
        sfx.PlayOnce(laugh);
    }

    public void AnimEvt_OnExit() => StartCoroutine(ExitMagicEffect());
    #endregion ANIMATION_EVENTS
}
