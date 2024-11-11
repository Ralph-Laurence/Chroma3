using System.Collections.Generic;
using UnityEngine;

public abstract class ITutorialStage : MonoBehaviour
{
    [SerializeField] protected AudioClip             bgmUpbeat;
    [SerializeField] protected AudioClip             sfxFill;
    [SerializeField] protected TutorialBlockFiller[] sequenceFillers;
    [SerializeField] protected StageCamera           stageCamera;
    [SerializeField] protected bool                  autoFindCamera;
    [SerializeField] protected Vector2               StageOffset;
    
    private List<Block>     wrongBlocks;
    private SoundEffects    sfx;
    protected BackgroundMusic bgm;
    private bool            shouldStickToBottom = true;
    private bool            isInitialized;
    private int             sequencesFilled;

    public bool ValidateStageOnFullyFilled;

    //
    //========================================================
    #region  OVERRIDABLE_METHODS
    //========================================================
    //

    /// <summary>
    /// Invoked when validation results to "Passed"
    /// </summary>
    public virtual void OnStagePassed() {}

    /// <summary>
    /// Invoked when validation results to "Failed"
    /// </summary>
    /// <param name="wrongBlocks"></param>
    public virtual void OnStageFailed(List<Block> wrongBlocks) {}

    /// <summary>
    /// Invoked before validation when the stage has completed filling its sequences
    /// </summary>
    public virtual void OnStageFillComplete() 
    {
        //if (bgm != null)
        //    bgm.Stop();
    }

    public virtual void BindEventObservers() 
    {
        TutorialEventNotifier.BindObserver(ObserveBlockFilled);
        TutorialEventNotifier.BindObserver(ObserveSequenceFilled);
        TutorialEventNotifier.BindObserver(ObserveSequenceValidated);
    }

    public virtual void UnBindEventObservers()
    {
        TutorialEventNotifier.UnbindObserver(ObserveBlockFilled);
        TutorialEventNotifier.UnbindObserver(ObserveSequenceFilled);
        TutorialEventNotifier.UnbindObserver(ObserveSequenceValidated);
    }
    //
    //
    #endregion OVERRIDABLE_METHODS

    //
    //========================================================
    #region  MONOBEHAVIOUR METHODS
    //========================================================
    //
    void Start()
    {
        if (autoFindCamera)
        {
            var cameraObj = GameObject.FindWithTag(Constants.Tags.MainCamera);
            cameraObj.TryGetComponent(out stageCamera);
        }
    }

    void Update()
    {
        if (stageCamera.AttachedCamera != null && shouldStickToBottom)
            StickToBottom();
    }

    void OnEnable()
    {
        Initialize();
        BindEventObservers();
    }

    void OnDisable() => UnBindEventObservers();

    #endregion
    //
    //========================================================
    #region EVENT_OBSERVERS
    //========================================================
    //
    private void ObserveBlockFilled(string key, object data)
    {
        if (sfx == null)
            return;

        if (key.Equals(TutorialEventNames.BlockFilled))
            sfx.PlayOnce(sfxFill);
    }

    private void ObserveSequenceFilled(string key, object data)
    {
        if (!key.Equals(TutorialEventNames.SequenceFilled))
            return;

        sequencesFilled++;

        // If all sequences in the stage had been fully colorized,
        // begin the validation
        if (sequencesFilled == sequenceFillers.Length)
        {
            OnStageFillComplete();
            ValidateStage();
        }
    }

    private void ObserveSequenceValidated(string key, object data)
    {
        if (!key.Equals(TutorialEventNames.SequenceValidated))
            return;

        if (data == null)
                return;

        var blocks = (List<Block>)data;
        wrongBlocks.AddRange(blocks);

        Debug.Log("Wrong Blocks => " + wrongBlocks.Count);
    }
    #endregion EVENT_OBSERVERS

    //
    //========================================================
    #region  BUSINESS_LOGIC
    //========================================================
    //

    private void Initialize()
    {
        if (isInitialized)
            return;

        sfx = SoundEffects.Instance;
        bgm = BackgroundMusic.Instance;

        wrongBlocks   = new();
        isInitialized = true;

        bgm.SetClip(bgmUpbeat);
        bgm.Play();
    }

    public void SetStickToBottom(bool stick) => shouldStickToBottom = stick;

    private void ValidateStage()
    {
        if (!ValidateStageOnFullyFilled)
            return;

        wrongBlocks?.Clear();

        for (var i = 0; i < sequenceFillers.Length; i++)
        {
            var sequence = sequenceFillers[i];
            sequence.ValidateSequence(out List<Block> wrongBlocks);

            if (wrongBlocks.Count > 0)
                this.wrongBlocks.AddRange(wrongBlocks);
        }

        // There are wrong blocks, thus FAILED
        if (wrongBlocks?.Count > 0)
        {
            OnStageFailed(wrongBlocks);
            return;
        }
        // Otherwise, PASSED
        OnStagePassed();
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
    //
    //
    //
    #endregion BUSINESS_LOGIC
}