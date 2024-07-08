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
    [SerializeField] [Range(0f, 30.0F)] private float minDragDistance = 20.0F;
    [SerializeField] [Range(1.0F, 10.0F)] private float rotationSpeed = 2.5f;
    [SerializeField] private float smoothTime = 0.05f; // Time it takes to reach the target rotation

    private Vector2 touchStart;
    private bool isDragging;
    private float targetRotationY;
    private float currentVelocity;
    
    [SerializeField] private bool freeze;

    public void Freeze() => freeze = true;
    public void UnFreeze() => freeze = false;

    public Camera AttachedCamera {get; private set; }
    //
    //=============================================
    // MONOBEHAVIOUR METHODS 
    //=============================================
    //
    void Start()
    {
        targetRotationY = transform.eulerAngles.y;
        CalculateAspect();
    }

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
    private void HandleDrag()
    {
        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                touchStart = touch.position;
                isDragging = false;
                break;

            case TouchPhase.Moved:
                Vector2 touchDelta = touch.position - touchStart;

                if (!isDragging && touchDelta.magnitude > minDragDistance)
                {
                    isDragging = true;
                }

                if (isDragging)
                {
                    float rotationY = touchDelta.x * rotationSpeed * Time.deltaTime;

                    // Update the target rotation (but reverse it to match drag direction)
                    targetRotationY += rotationY * -1.0F;

                    touchStart = touch.position; // Update the start position for continuous drag
                }
                break;

            case TouchPhase.Ended:
                isDragging = false;
                break;
        }

        // Smoothly interpolate to the target rotation
        float smoothedRotationY = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotationY, ref currentVelocity, smoothTime);

        // Apply the smoothed rotation while keeping the X and Z rotation the same
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, smoothedRotationY, transform.eulerAngles.z);
    }
}
