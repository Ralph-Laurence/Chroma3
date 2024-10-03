using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EmpBombFx : MonoBehaviour
{
    [Header("Visual Effects")]
    [SerializeField] private Image fadingOverlay;
    [SerializeField] private float fadingSpeed = 2.0F;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Transform empBomb;
    [SerializeField] private GameObject empBombInnerMesh;
    [SerializeField] private GameObject empExplosionVfx;
    [SerializeField] private float empCruiseSpeed = 1.7F;

    [Space(10)] [Header("Sound Effects")]
    [SerializeField] private AudioClip spaceSound;
    [SerializeField] private AudioClip empBeginLookAt;
    [SerializeField] private AudioClip empSuspense;
    [SerializeField] private AudioClip empExplosionSfx;

    [Space(10)] [Header("Male Voices")]
    [SerializeField] private AudioClip[] maleAttackCommands;
    [SerializeField] private AudioClip[] maleStatusCommands;
    private const int IDENTIFIER_MALE = 0;

    [Space(10)] [Header("Female Voices")]
    [SerializeField] private AudioClip[] femaleAttackCommands;
    [SerializeField] private AudioClip[] femaleStatusCommands;
    private const int IDENTIFIER_FEMALE = 1;

    // Begin main effect transitions
    private AudioClip earthVoice_status;
    private AudioClip satOneVoice_attack;
    
    private SoundEffects sfx;

    // State flags
    private bool empDetonated;
    private bool beginFollowEmp;
    private bool cinematicHorrorBegan;
    private float empMoveSpeed;

    void Awake()
    {
        sfx = SoundEffects.Instance;
        empMoveSpeed = empCruiseSpeed;

        fadingOverlay.color = Color.black;
    }

    void Start()
    {
        // Which earth voice should come first?
        var whoComesFirst = Random.Range(0, 2);

        // If the sender voice (earth) is Male, 
        // the reciever voice (satellite) must be female
        switch (whoComesFirst)
        {
            case IDENTIFIER_MALE:
                earthVoice_status  = SelectRandomClip(maleStatusCommands);
                satOneVoice_attack = SelectRandomClip(femaleAttackCommands);
                break;

            case IDENTIFIER_FEMALE:
                earthVoice_status  = SelectRandomClip(femaleStatusCommands);
                satOneVoice_attack = SelectRandomClip(maleAttackCommands);
                break;
        }

        BeginEffect();
    }

    void Update()
    {
        if (!empDetonated && beginFollowEmp)
        {
            // Begin the cruising of EMP missile
            LookAtEMP();
            empBomb.transform.Translate(empMoveSpeed * Time.deltaTime * transform.forward);

            // If the EMP is somewhere or almost at the middle of the view, slow it down
            // then show some cinematic suspense
            if (empBomb.position.z >= -12.5F && !cinematicHorrorBegan)
                StartCoroutine(BeginCinematicHorror());

            // If the EMP hits the earth, detonate it
            else if(empBomb.position.z >= -2.0F)
                StartCoroutine(BeginDetonation());
        }
    }

    private AudioClip SelectRandomClip(AudioClip[] clips) => clips[ Random.Range(0, clips.Length) ];
    
    private void PlaySfx(AudioClip clip)
    {
        if (sfx != null)
            sfx.PlayOnce(clip);
    }

    public void BeginEffect()
    {
        PlaySfx(spaceSound);

        var fromAlpha = fadingOverlay.color.a;

        LeanTween.value(fadingOverlay.gameObject, FadeCallback, fromAlpha, 0.0F, fadingSpeed)
                 .setOnComplete(() => {

                    // Rotate the camera 90deg Y to face the satellite
                    LeanTween.rotateY(mainCamera.gameObject, 90.0F, 5.0F)
                             .setDelay(2.25F)
                             .setEase(LeanTweenType.easeInOutQuad)
                             .setOnComplete(() => StartCoroutine(BeginChatterFx()));
                 });
    }

    private IEnumerator BeginChatterFx()
    {
        PlaySfx(earthVoice_status);
        yield return new WaitForSeconds(3.85F);

        PlaySfx(satOneVoice_attack);

        yield return new WaitForSeconds(satOneVoice_attack.length);

        var rotation = GetEmpLookAtTrackingRotation();
        var duration = 1.25F;
        
        empBomb.gameObject.SetActive(true);

        PlaySfx(empBeginLookAt);

        LeanTween.rotate(mainCamera.gameObject, rotation.eulerAngles, duration)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() => {
                        PlaySfx(empSuspense);
                        beginFollowEmp = true;
                 });

        yield return null;
    }

    private IEnumerator BeginCinematicHorror()
    {
        if (cinematicHorrorBegan)
            yield break;
        
        cinematicHorrorBegan = true;
        empMoveSpeed = 0.125F;

        LeanTween.rotateAroundLocal(empBombInnerMesh, Vector3.forward, 260.0F, 3.0F)
                 .setOnComplete(() => empMoveSpeed = empCruiseSpeed + 2.5F);

        yield return new WaitForSeconds(3.0F);
    }

    private IEnumerator BeginDetonation()
    {
        if (empDetonated)
            yield break;

        empDetonated = true;

        empBomb.gameObject.SetActive(false);

        // Rotate the camera 0deg Y to face the earth
        LeanTween.rotateY(mainCamera.gameObject, 0.0F, 0.25F)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() => {

                    empExplosionVfx.SetActive(true);
                    PlaySfx(empExplosionSfx);

                 });
        
        yield return new WaitForSeconds(2.0F);

        // Fade out the screen
        var fromAlpha = fadingOverlay.color.a;

        LeanTween.value(fadingOverlay.gameObject, FadeCallback, fromAlpha, 1.0F, fadingSpeed / 2.0F)
                 .setOnComplete(() => StartCoroutine(CloseCutscene()));
    }

    private void FadeCallback(float value)
    {
        var fadeColor = fadingOverlay.color;
        fadeColor.a = value;

        fadingOverlay.color = fadeColor;
    }

    private void LookAtEMP()
    {
        var rotation = GetEmpLookAtTrackingRotation();
        mainCamera.rotation = rotation;
    }

    private Quaternion GetEmpLookAtTrackingRotation()
    {
        var lookDirection = empBomb.position - mainCamera.position;
        lookDirection.y = 0;

        var rotation = Quaternion.LookRotation(lookDirection);

        return rotation;
    }

    private IEnumerator CloseCutscene()
    {
        yield return new WaitForSeconds(0.2F);

        var sceneName = gameObject.scene.name;
        SceneManager.UnloadSceneAsync(sceneName);
    }
}