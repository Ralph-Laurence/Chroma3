using System;
using System.Collections;
using Revamp;
using UnityEngine;

public class GrandMasterFx : MonoBehaviour
{
    [SerializeField] private GameObject character;
    [SerializeField] private GameObject mageStaff;
    [SerializeField] private GameObject lightningFx;
    [SerializeField] private GameObject sparksAuraFx;
    [SerializeField] private GameObject sparksAuraEndFx;
    [SerializeField] private GameObject smokeOnDisappear;
    [SerializeField] private GameObject flashOnExit;
    [SerializeField] private GameObject magicHalo;
    [SerializeField] private GameObject[] magicGlowFx;
    [SerializeField] private GameObject effectMinions;
    [SerializeField] private GameObject[] minions;

    [SerializeField] private Light sunLight;
    [SerializeField] private float sunLightIntensity = 1.0F;
    [SerializeField] private Color sunLightColor = new(1.0F, 0.96F, 0.84F, 1.0F);
    [SerializeField] private Color lightningLightColor = new(1.0F, 0.46F, 0.95F, 1.0F);
    [SerializeField] private float lightningIntensity = 2.0F;
    [SerializeField] private ParticleSystem onSpawnParticleFx;

    [SerializeField] private AudioClip sfxSpawn;
    [SerializeField] private AudioClip sfxSparks;
    [SerializeField] private AudioClip sfxOnDisappear;
    [SerializeField] private AudioClip sfxDoMagic;
    [SerializeField] private AudioClip sfxLaugh;
    [SerializeField] private AudioClip sfxExiting;
    [SerializeField] private AudioClip sfxExitPoof;

    [HideInInspector] public StageVariant StageVariantEffectTarget;

    private Animator animator;
    private SoundEffects sfx;

    private bool magicFloating;
    private bool magicFloatCancelled;

    void Awake()
    {
        sfx = SoundEffects.Instance;
        TryGetComponent(out animator);
    }

    void Update()
    {
        if (!magicFloating && !magicFloatCancelled)
        {
            LeanTween.cancel(character);
            magicFloatCancelled = true;
        }

        effectMinions.transform.Rotate(90.0F * Time.deltaTime * Vector3.up);
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

    #region STATE_CONTROL
    public void BeginEffect()
    {
        InGameHotbarInteractionStateNotifier.NotifyObserver(blockInteraction: true);

        // Position the grandmaster just above the stage variant
        var posAboveStage = StageVariantEffectTarget.transform.position;
        posAboveStage.y += 0.5F;

        transform.position = posAboveStage;
        StartCoroutine(SpawnTheGrandMaster());
    }

    // Immediate stop without animation and sound effects.
    // Used when the powerup is applied but the player initiates a retry
    private void ForceStopEffect()
    {
        animator.StopPlayback();
        StageVariantEffectTarget = null;

        sunLight.color = sunLightColor;
        sunLight.intensity = sunLightIntensity;

        mageStaff.SetActive(true);

        DisableGameObject(minions);
        DisableGameObject(magicGlowFx);
        DisableGameObject(new GameObject[]
        {
            effectMinions,
            character,
            lightningFx,
            sparksAuraFx,
            sparksAuraEndFx,
            flashOnExit,
            smokeOnDisappear
        });

        InGameHotbarInteractionStateNotifier.NotifyObserver(blockInteraction: false);
        gameObject.SetActive(false);
    }

    // Disable gameobjects synchronous to frame updates
    private void DisableGameObject(GameObject[] gameObjects)
    {
        for (var i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].SetActive(false);
        }
    }

    #endregion STATE_CONTROL

    private IEnumerator SpawnTheGrandMaster()
    {
        InteractionBlockerNotifier.NotifyObserver(show: true);

        // Create the lightning flash sequence
        LeanTween.value(gameObject, 1.0F, 0.0F, 0.1F)
                 .setOnUpdate((float val) => sunLight.intensity = val)
                 .setOnComplete(() =>
                 {
                    sunLight.color = lightningLightColor;
                    FlashLightning();
                 });

        lightningFx.SetActive(true);
        sfx.PlayOnce(sfxSpawn);
        
        // Make the character visible as well has his aura sparks
        yield return new WaitForSeconds(0.25F);
        character.SetActive(true);
        character.transform.localEulerAngles = Vector3.up * 180.0F;
        sparksAuraFx.SetActive(true);
        sfx.PlayOnce(sfxSparks);

        yield return new WaitForSeconds(1.3F);
        sparksAuraFx.SetActive(false);
        sparksAuraEndFx.SetActive(true);

        yield return new WaitForSeconds(0.32F);
        animator.SetTrigger("DoMagic");
    }

    private  void FlashLightning()
    {
        var flashTimes = 3;

        LeanTween.value(gameObject, 0.0F, lightningIntensity, 0.1F)
                 .setOnUpdate((float val) => sunLight.intensity = val)
                 .setLoopPingPong(flashTimes)
                 .setOnComplete(() =>
                 {
                    sunLight.color = sunLightColor;
                    sunLight.intensity = 1f;
                 });
    }

    public void AnimEvt_OnFlipEnd()
    {
        character.SetActive(false);
        mageStaff.SetActive(false);
        sfx.PlayOnce(sfxOnDisappear);
        smokeOnDisappear.SetActive(true);
        StartCoroutine(IEToggleGlowFx(true));
        magicHalo.transform.localScale = Vector3.one * 0.5F;

        StartCoroutine(IEBeginMagic());
    }

    private IEnumerator IEBeginMagic()
    {
        yield return new WaitForSeconds(0.3F);
        
        character.SetActive(true);
        sfx.PlayOnce(sfxLaugh);
        //
        // The character should appear floating in air by oscillating it Up and Down
        //
        var floatHeight = 0.45F;
        var floatRate = 3.0F;

        magicFloating = true;
        magicFloatCancelled = false;

        LeanTween.moveLocalY(character, character.transform.localPosition.y + floatHeight, floatRate)
                 .setEase(LeanTweenType.easeInOutSine)
                 .setLoopPingPong()
                 .setOnComplete(() => {
                    if (!magicFloating)
                        LeanTween.cancel(character);
                 });

        yield return new WaitForSeconds(1.0F);
        //
        // Show the minions
        //
        effectMinions.SetActive(true);

        // Space them minions out
        // 1        - North
        // (-1)     - South
        // 2        - East
        // (-2)     - West
        int[] spacingDirections = { 1, -1, 2, -2};
        var spacing = 0.85F;
        var spaceOutSpeed = 1.20F;

        for (int i = 0; i < minions.Length; i++)
        {
            var direction = spacingDirections[i];
            var minion = minions[i];

            switch (Math.Abs(direction))
            {
                // Vertical Directions
                case 1:
                    var spaceOutZ = Vector3.forward * (spacing * direction);
                    LeanTween.moveLocal(minion, spaceOutZ, spaceOutSpeed);
                    break;
                
                // Horizontal Directions
                case 2:
                    direction /= 2;

                    var spaceOutX = Vector3.right * (spacing * direction);
                    LeanTween.moveLocal(minion, spaceOutX, spaceOutSpeed);
                    break;
            }

            if (i == minions.Length - 1)
            {
                sfx.PlayOnce(sfxDoMagic);
                yield return new WaitForSeconds(2.0F);

                var onFinishFill = new Action(() => StartCoroutine(IEExitGrandMaster()));

                StageVariantEffectTarget.ExecuteGrandMasterEffect(onFinishFill);
            }

            yield return null;
        }

    }

    private IEnumerator IEToggleGlowFx(bool toggle)
    {
        for (var i = 0; i < magicGlowFx.Length; i++)
        {
            if (toggle)
            {
                magicGlowFx[i].SetActive(true);
                yield return null;
                continue;
            }

            magicGlowFx[i].SetActive(false);
            yield return null;
        }
    }

    private IEnumerator IEExitGrandMaster()
    {
        sfx.PlayOnce(sfxExiting);

        //
        // Close (space-in) all minions
        //
        for (int i = 0; i < minions.Length; i++)
        {
            var minion = minions[i];
            var tween = LeanTween.moveLocal(minion, Vector3.zero, 0.5F);
            
            if (i == minions.Length - 1)
                tween.setOnComplete(() => effectMinions.SetActive(false));
                
            yield return null;
        }
        
        //
        // Spin the grand master fast (3x) then vanish
        //
        
        // 1 full rotation is 360 deg. We will rotate it at full axis 
        // for three times then rotate it half an axis (180)
        var rotationY = Vector3.up * ((360.0F * 3) + 180);
        
        LeanTween.rotateLocal(character, rotationY, 3.0F)
                 .setEase(LeanTweenType.easeInCubic)
                 .setOnComplete(() => {
                    character.transform.localPosition = Vector3.zero;
                    character.SetActive(false);
                    mageStaff.SetActive(true);
                    animator.SetTrigger("ResetToIdle");
                    StartCoroutine(IEToggleGlowFx(false));
                    flashOnExit.SetActive(true);
                    sfx.PlayOnce(sfxExitPoof);
                    
                    InteractionBlockerNotifier.NotifyObserver(show: false);
                    StartCoroutine(IECompleteStage());                    
                 });

        // Scale down the halo
        LeanTween.scale(magicHalo, Vector3.zero, 4.0F);
    }

    private IEnumerator IECompleteStage()
    {
        yield return new WaitForSeconds(1.0F);

        // End the stage with result of "Passed"
        OnStageCompleted.NotifyObserver(StageCompletionType.Success);

        InGameHotbarInteractionStateNotifier.NotifyObserver(blockInteraction: false);
    }
}