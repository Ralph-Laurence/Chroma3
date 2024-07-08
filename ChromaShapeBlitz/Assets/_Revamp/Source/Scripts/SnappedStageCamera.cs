using System.Collections;
using UnityEngine;

public class SnappedStageCamera : MonoBehaviour
{
    [Space(5)] [Header("Responsive View Scaling")]
    public float MinOrthoSize = 5.60F;
    public float MaxOrthoSize = 7.55F;

    [Space(5)] 
    [Header("View Swiping")]
    [SerializeField]
    [Range(0f, 300.0F)]
    private float swipeThreshold = 120.0F;

    [SerializeField]
    [Range(1.0F, 10.0F)]
    private float rotationSpeed = 5.0f;
    // Initial view is the default Y rotation of camera,
    // which must always start at center (0)
    private readonly float _initialView = 0.0F;
    public float ViewTargetLeft  = 60F;
    public float ViewTargetRight = -60F;
    private float originalOrthographicSize;
    private float originalAspectRatio;

    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;

    [SerializeField] private bool freeze;

    public void Freeze() => freeze = true;
    public void UnFreeze() => freeze = false;

    public bool IsRotating { private set; get; }

    public Camera AttachedCamera {get; private set; }
    //
    //=============================================
    // MONOBEHAVIOUR METHODS 
    //=============================================
    //
    void Start()      => CalculateAspect();
    void LateUpdate() => UpdateOrthoSize();

    void Awake()
    {
        TryGetComponent(out Camera cam);

        if (cam != null)
            AttachedCamera = cam;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
            HandleSwiped();
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
    /// Detect swipe gestures and handle the view switching accordingly
    /// </summary>
    private void HandleSwiped()
    {
        var touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:

                // Register the touch position when the finger landed on the screen
                fingerDownPosition = touch.position;

                break;

            case TouchPhase.Ended:

                // Get the touch position when the finger leaves the screen
                fingerUpPosition = touch.position;

                // We need to detect the swipe only when the swipe distance is greater than
                // the swipe (threshold) sensitivity. This is to prevent false swipe as any
                // distance that is non-zero is considered as "Swipe".
                var swipeLength = Vector2.Distance(fingerDownPosition, fingerUpPosition);

                if (Mathf.Abs(swipeLength) > swipeThreshold)
                    SwitchViewDirection(fingerUpPosition);

                // Reset the fingerUp position into fingerDown position.
                // This is to prepare for the next consecutive swipes.
                fingerUpPosition = fingerDownPosition;

                break;
        }
    }

    /// <summary>
    /// Determine which direction we should rotate according to swipe
    /// </summary>
    private void SwitchViewDirection(Vector2 fingerUpPosition)
    {
        if (freeze)
            return;

        var rotationY = transform.rotation.eulerAngles.y.WrapAngles();

        // Swiped LEFT
        if (fingerUpPosition.x < fingerDownPosition.x && !IsRotating)
        {
            // Determine the current viewing rotation by subtracting the current Y rotation 
            // from the last direction ("initial" or "targetLeft"). Wrap the result into a
            // positive value and if it is less than 0.01, we assume we're facing that
            // direction and intend to rotate the opposite way.

            if (Mathf.Abs(rotationY - _initialView) < 0.01f)

                // From Center going to Right
                StartCoroutine(RotateToTarget(ViewTargetRight));

            else if (Mathf.Abs(rotationY - ViewTargetLeft) < 0.01f)

                // From Left back to Center
                StartCoroutine(RotateToTarget(_initialView));

        }

        // Swiped RIGHT
        if (fingerUpPosition.x > fingerDownPosition.x && !IsRotating)
        {
            if (Mathf.Abs(rotationY - _initialView) < 0.01f)

                // From Center going to Left
                StartCoroutine(RotateToTarget(ViewTargetLeft));

            else if (Mathf.Abs(rotationY - ViewTargetRight) < 0.01f)

                // From Right back to Center
                StartCoroutine(RotateToTarget(_initialView));
        }
    }

    /// <summary>
    /// Interpolate the camera to rotate at the target rotation
    /// </summary>
    private IEnumerator RotateToTarget(float target)
    {
        // Setting this flag to TRUE prevents interrupting the
        // interpolation while the rotation is still going on.
        // This means we cant rotate to a different direction
        // during ongoing rotation interpolation.
        IsRotating = true;

        var startRotation = transform.rotation;

        var endRotation   = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            target,
            transform.rotation.eulerAngles.z
        );

        // Interoplation time from startRotation to endRotation
        var t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        // Interpolation completed
        IsRotating = false;
    }
}
