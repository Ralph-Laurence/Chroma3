using Revamp;
using UnityEngine;

public class StageBackgroundManager : MonoBehaviour
{
    [SerializeField] private StageCamera mainCamera;
    private MobilePostProcessing cameraPostFx;
    private Skybox camAttachedSkybox;
    private GameSessionManager gsm;
    private GameObject backgroundGameObject;
    private Transform createdStageVariantTransform;
    private StageBackground _stageBackground;

    private bool isBackgroundAppplied;
    private bool isBackgroundCreated;

    void Awake()
    {
        gsm = GameSessionManager.Instance;
        mainCamera.TryGetComponent(out cameraPostFx);
        mainCamera.TryGetComponent(out camAttachedSkybox);
    }
    
    void OnEnable() => OnStageCreated.BindObserver(ObserveStageCreated);
    void OnDisable() => OnStageCreated.UnbindObserver(ObserveStageCreated);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() => ApplyBackground();

    private void ObserveStageCreated(StageCreatedEventArgs stage)
    {
        TransformBackground(stage.StageTransform);
    }    

    private void ApplyBackground()
    {
        if (gsm == null)
            return;

        if (!isBackgroundCreated)
        {
            // Load the background asset from the resources folder
            var backgroundName = gsm.UserSessionData.ActiveBackgroundID;
            var background = Resources.Load<GameObject>($"Backgrounds/{backgroundName}");

            backgroundGameObject = Instantiate(background);
            backgroundGameObject.TryGetComponent(out _stageBackground);

            cameraPostFx.enabled = _stageBackground.UsePostProcess;

            if (_stageBackground.Skybox != null)
                camAttachedSkybox.material = _stageBackground.Skybox;

            if (_stageBackground.UsePostProcess)
                ApplyPostProcess(_stageBackground);

            isBackgroundCreated = true;
        }

        if (backgroundGameObject == null)
            return;
    }

    private void ApplyPostProcess(StageBackground bgPostProcess)
    {
        if (bgPostProcess.EnableBloom)
        {
            cameraPostFx.Bloom          = true;
            cameraPostFx.BloomAmount    = bgPostProcess.BloomIntensity;
            cameraPostFx.BloomColor     = bgPostProcess.BloomColor;
        }

        if (bgPostProcess.EnableLut)
        {
            cameraPostFx.LUT        = bgPostProcess.EnableLut;
            cameraPostFx.LutAmount  = bgPostProcess.LutIntensity;
            cameraPostFx.SourceLut  = bgPostProcess.LutSource;
        }

        if (bgPostProcess.EnableVignette)
        {
            cameraPostFx.Vignette           = bgPostProcess.EnableVignette;
            cameraPostFx.VignetteAmount     = bgPostProcess.VignetteIntensity;
            cameraPostFx.VignetteColor      = bgPostProcess.VignetteColor;
            cameraPostFx.VignetteSoftness   = bgPostProcess.VignetteSoftness;
        }
    }

    private void TransformBackground(Transform createdStageVariantTransform)
    {
        var backgroundTargetPos = backgroundGameObject.transform.position;
        var backgroundTargetRot = backgroundGameObject.transform.eulerAngles;

        backgroundTargetPos.y = _stageBackground.YPositionOffset;
        backgroundTargetRot.y = createdStageVariantTransform.eulerAngles.y;

        backgroundGameObject.transform.SetPositionAndRotation
        (
            backgroundTargetPos,
            Quaternion.Euler(backgroundTargetRot)
        );
    }
}