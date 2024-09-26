using System;
using System.Collections;
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
        // if (Input.GetKeyUp(KeyCode.F1))
        // {
        //     StartCoroutine(SpawnTheGrandMaster());
        // }
        // else if (Input.GetKeyUp(KeyCode.F2))
        // {
        //     magicFloating = false;
        //     StartCoroutine(IEExitGrandMaster());
        // }

        if (!magicFloating && !magicFloatCancelled)
        {
            LeanTween.cancel(character);
            magicFloatCancelled = true;
        }

        effectMinions.transform.Rotate(90.0F * Time.deltaTime * Vector3.up);
    }

    private IEnumerator SpawnTheGrandMaster()
    {
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
        var spaceOutSpeed = 1.65F;

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

            yield return null;
        }
        // LeanTween.scale(effectMinions, Vector3.one, 2.0F);
        sfx.PlayOnce(sfxDoMagic);
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
                 });

        // Scale down the halo
        LeanTween.scale(magicHalo, Vector3.zero, 4.0F);
    }
}