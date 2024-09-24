using System.Collections;
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

    private Animator animator;
    private SoundEffects sfx;

    void Awake()
    {
        sfx = SoundEffects.Instance;
        TryGetComponent(out animator);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1))
        {
            if (onSpawnSmoke.isPlaying)
                return;

            StartCoroutine(SpawnTheWizard());
        }
        else if (Input.GetKeyUp(KeyCode.F2))
        {
            character.SetActive(false);
            onSpawnSmoke.Stop();
            onSpawnSmoke.gameObject.SetActive(false);
        }
    }

    private IEnumerator SpawnTheWizard()
    {
        onSpawnSmoke.gameObject.SetActive(true);
        sfx.PlayOnce(sfxSpawn);

        yield return new WaitForSeconds(0.3F);
        character.SetActive(true);

        yield return new WaitForSeconds(1.0F);
        animator.SetTrigger("DoMagic");

        yield return new WaitForSeconds(3.0F);
        animator.SetTrigger("FinishMagic");
    }

    public void AnimEvt_OnChargeMagic()
    {
        StartCoroutine(ToggleIgnitionEffects(true));
        sfx.PlayOnce(sfxIgniteWand);
    }

    public void AnimEvt_OnApplyMagic()
    {
        onMagicApplied.gameObject.SetActive(true);
        sfx.PlayOnce(sfxDoMagic);

        // Turn off the first ignition effect which is the "crown"
        wandIgnitionEffects[0].SetActive(false);
    }

    public void AnimEvt_OnMagicApplied()
    {
        var laugh = wizardLaughs[Random.Range(0, wizardLaughs.Length)];
        sfx.PlayOnce(laugh);
    }

    public void AnimEvt_OnExit()
    {
        StartCoroutine(ExitMagicEffect());
    }

    private IEnumerator ToggleIgnitionEffects(bool toggle)
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
           
        yield return StartCoroutine(ToggleIgnitionEffects(false));
        onMagicExit.gameObject.SetActive(true);
        sfx.PlayOnce(sfxExit);
    }
}
