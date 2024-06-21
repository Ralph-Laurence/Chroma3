using System.Collections.Generic;
using UnityEngine;

public partial class StageVariant : MonoBehaviour
{
    private StageCamera stageCamera;

    [Space(10)] [Header("Set By Fabricator; Don't Touch!")]
    public LevelDifficulties VariantDifficulty;
    public int VariantNumber;
    public string VariantName;
    public Material IncorrectBlockMaterial;

    [Space(10)] [Header("Level Design")]
    public bool AutoFindCamera;
    public Vector2 StageOffset;
    public float ViewAngleLeft;
    public float ViewAngleRight;
    public float MinCameraOrthoSize;
    public float MaxCameraOrthoSize;
    public AudioClip BgmClip;

    [Space(10)] [Header("Mechanics")]
    public Sprite PatternObjective;
    public int TotalStageTime;
    public int MinStageTime;
    public int MaxStageTime;
    public RewardTypes RewardType;
    public int TotalReward;

    [Space(10)] 
    public List<BlockSequenceController> SequenceSet = new List<BlockSequenceController>();

    //
    //==========================================
    // MONOBEHAVIOUR METHODS
    //==========================================
    //

    // Start is called before the first frame update
    void Start()
    {
        if (AutoFindCamera && stageCamera == null)
            FindCamera();    
    }

    // Update is called once per frame
    void Update()
    {
        if (stageCamera.AttachedCamera != null)
            StickToBottom();
    }
    //
    //==========================================
    // STAGE VIEWING CAMERA SETUP
    //==========================================
    //
    private void FindCamera()
    {
        GameObject.FindWithTag(Constants.Tags.MainCamera)
            .TryGetComponent(out StageCamera cam);

        if (cam == null)
        {
            Debug.LogWarning("Stage camera not found!");
            return;
        }

        stageCamera = cam;
        stageCamera.ViewTargetLeft  = ViewAngleLeft;
        stageCamera.ViewTargetRight = ViewAngleRight;
        stageCamera.MinOrthoSize    = MinCameraOrthoSize;
        stageCamera.MaxOrthoSize    = MaxCameraOrthoSize;
    }

    /// <summary>
    /// This stage gameobject should always stick to the bottom of screen
    /// </summary>
    private void StickToBottom()
    {
        var cameraPosition   = stageCamera.AttachedCamera.transform.position;
        var cameraRotationX  = stageCamera.AttachedCamera.transform.rotation.eulerAngles.x;
        var orthographicSize = stageCamera.AttachedCamera.orthographicSize;

        // Calculate the vertical distance from the camera based on its rotation and orthographic size
        var verticalDistance = orthographicSize * Mathf.Tan(cameraRotationX * Mathf.Deg2Rad);

        // Set the model's position to be at the bottom of the screen with the specified offset
        transform.position = new Vector3
        (
            cameraPosition.x + StageOffset.x,
            cameraPosition.y - verticalDistance + StageOffset.y,
            cameraPosition.z
        );
    }
}