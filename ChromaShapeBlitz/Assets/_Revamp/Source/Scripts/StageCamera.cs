using System;
using System.Collections;
using UnityEngine;

public class StageCamera : MonoBehaviour
{
    [Header("Responsive View Scaling")]
    public float MinOrthoSize = 5.60F;
    public float MaxOrthoSize = 7.55F;
    private float originalOrthographicSize;
    private float originalAspectRatio;

    [Header("Camera Dragging")]
    [SerializeField][Range(0.0F, 10.0F)] private float rotationSpeed = 0.08F;

    [SerializeField] private bool freeze;
    //public bool AllowDrag = true;
    public void Freeze() => freeze = true;
    public void UnFreeze() => freeze = false;

    public Camera AttachedCamera { get; private set; }

    //public void EnableLookAround(bool enable) => AllowDrag = enable;
    //
    //=============================================
    // MONOBEHAVIOUR METHODS 
    //=============================================
    //
    private Quaternion initialRotation;
    void Start()
    {
        initialRotation = transform.rotation;
        CalculateAspect();
    }

    void Awake()
    {
        TryGetComponent(out Camera cam);

        if (cam != null)
            AttachedCamera = cam;
    }

    private void LateUpdate()
    {
        if (!isViewingFromAbove)
            UpdateOrthoSize();

        if (Input.touchCount > 0)
            HandleDrag();
    }

    //
    //=============================================
    // BUSINESS LOGIC
    //=============================================
    //
    private void CalculateAspect()
    {
        originalOrthographicSize = Camera.main.orthographicSize;

        // 16:9
        if (Camera.main.aspect >= 1.7)
            originalAspectRatio = 16.0F / 9.0F;

        // 3:2
        else if (Camera.main.aspect >= 1.5)
            originalAspectRatio = 3.0F / 2.0F;

        // 4:3
        else
            originalAspectRatio = 4.0F / 3.0F;
    }

    /// <summary>
    /// Adjust the view to fit the stage object at the center of screen
    /// </summary>
    private void UpdateOrthoSize()
    {
        var size = originalOrthographicSize * (originalAspectRatio / Camera.main.aspect);

        if (size > MaxOrthoSize)
            size = MaxOrthoSize;

        else if (size < MinOrthoSize)
            size = MinOrthoSize;

        Camera.main.orthographicSize = size;
    }

    /// <summary>
    /// Detect drag gestures and handle the view switching accordingly
    /// </summary>
    private float previousTouchPositionX;
    private bool isTouching = false;

    private void HandleDrag()
    {
        if (freeze)// || !AllowDrag)
            return;

        var touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                isTouching = true;
                previousTouchPositionX = touch.position.x;
                break;

            case TouchPhase.Moved:
                if (isTouching)
                {
                    float deltaX = touch.position.x - previousTouchPositionX;
                    float rotationY = deltaX * rotationSpeed;

                    // Only rotate around the Y-axis
                    transform.Rotate(0, -rotationY, 0);

                    // Maintain the original X and Z rotations
                    Vector3 eulerAngles = transform.eulerAngles;
                    eulerAngles.x = initialRotation.eulerAngles.x;
                    eulerAngles.z = initialRotation.eulerAngles.z;
                    transform.eulerAngles = eulerAngles;

                    previousTouchPositionX = touch.position.x;
                }
                break;

            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                isTouching = false;
                break;
        }
    }

    public void ResetView(bool unfreeze = false)
    {
        var currentAngles = transform.rotation.eulerAngles;
        currentAngles.y = 0.0F;

        transform.eulerAngles = currentAngles;

        if (unfreeze)
            UnFreeze();
    }

    private bool isViewingFromAbove;
    private float initialOrtho;
    private Vector3 rotationBeforeViewAbove;

    /// <summary>
    /// Invoked from a powerup hint, rotates the stage camera above the stage at 90 degrees X.
    /// This should follow the stage variant's current rotation so that the camera appears center-zeroed.
    /// After that, it reverts back to its default rotation.
    /// </summary>
    /// <param name="followStageRotation">When set, the stage camera rotates its Y rotation the same as stage variant's Y</param>
    /// <param name="transitionRate">How fast the rotation from above go.</param>
    /// <param name="addViewHeight">Move the camera a little bit above</param>
    /// <param name="onDone">Callback when transition was done</param>
    public IEnumerator IEViewFromAbove
    (
        float followStageYRotation = 0.0F,
        float transitionRate       = 0.25F,
        float addViewHeight        = 1.45F
        // Action onDone              = null
    )
    {
        if (isViewingFromAbove)
            yield break;

        isViewingFromAbove = true;

        // Cache original parameters before viewing from above
        initialOrtho = AttachedCamera.orthographicSize;
        rotationBeforeViewAbove = transform.localEulerAngles;
        
        // Move Above (by adding more Ortho)
        var moveUpTo = AttachedCamera.orthographicSize + addViewHeight;

        // Increase the ortho size
        LeanTween.value
        (
            gameObject,
            OnUpdateTweenOrthoSize,
            initialOrtho,
            moveUpTo,
            transitionRate
        );

        var topdownRotTarget = new Vector3(90.0F, followStageYRotation, 0.0F);
        
        // Rotate top-down
        LeanTween.rotate(gameObject, topdownRotTarget, transitionRate);
                 //.setOnComplete(() => onDone?.Invoke());

        while (LeanTween.isTweening(gameObject))
        {
            yield return null;
        }
    }
    // public void ViewFromAbove
    // (
    //     float followStageYRotation = 0.0F,
    //     float transitionRate       = 0.25F,
    //     float addViewHeight        = 1.45F,
    //     Action onDone              = null
    // )
    // {
    //     if (isViewingFromAbove)
    //         return;

    //     isViewingFromAbove = true;

    //     // Cache original parameters before viewing from above
    //     initialOrtho = AttachedCamera.orthographicSize;
    //     rotationBeforeViewAbove = transform.localEulerAngles;
        
    //     // Move Above (by adding more Ortho)
    //     var moveUpTo = AttachedCamera.orthographicSize + addViewHeight;

    //     // Increase the ortho size
    //     LeanTween.value
    //     (
    //         gameObject,
    //         OnUpdateTweenOrthoSize,
    //         initialOrtho,
    //         moveUpTo,
    //         transitionRate
    //     );

    //     var topdownRotTarget = new Vector3(90.0F, followStageYRotation, 0.0F);
        
    //     // Rotate top-down
    //     LeanTween.rotate(gameObject, topdownRotTarget, transitionRate)
    //              .setOnComplete(() => onDone?.Invoke());
    // }

    // public void UnviewFromAbove(float transitionRate = 0.25F, Action onDone = null)
    // {
    //     // Reset the ortho size
    //     LeanTween.value
    //     (
    //         gameObject,
    //         OnUpdateTweenOrthoSize,
    //         AttachedCamera.orthographicSize,
    //         initialOrtho,
    //         transitionRate
    //     );

    //     // Rotate top-down
    //     LeanTween.rotate(gameObject, rotationBeforeViewAbove, transitionRate)
    //              .setOnComplete(() =>
    //              {
    //                 isViewingFromAbove = false;
    //                 onDone?.Invoke();
    //              });
    // }

    public IEnumerator IEUnviewFromAbove(float transitionRate = 0.25F)
    {
        // Reset the ortho size
        LeanTween.value
        (
            gameObject,
            OnUpdateTweenOrthoSize,
            AttachedCamera.orthographicSize,
            initialOrtho,
            transitionRate
        );

        // Rotate top-down
        LeanTween.rotate(gameObject, rotationBeforeViewAbove, transitionRate);

        while (LeanTween.isTweening(gameObject))
        {
            yield return null;
        }

        isViewingFromAbove = false;
    }

    // Callback on ortho tween update
    private void OnUpdateTweenOrthoSize(float value) => AttachedCamera.orthographicSize = value;
}
