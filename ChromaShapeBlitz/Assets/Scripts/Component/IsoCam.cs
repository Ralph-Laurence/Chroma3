using System;
using System.Collections;
using UnityEngine;

public class IsoCam : MonoBehaviour
{
    float originalOrthographicSize;
    float originalAspectRatio;

    [Tooltip("When true, this will automatically find GameManager instance")]
    [SerializeField] private bool autoAssignGameManager = true;
    [SerializeField] private GameManager gameManager;

    [Space(5)] [Header("View Scaling")]
    [SerializeField] private float minOrthoSize = 5.60F;
    [SerializeField] private float maxOrthoSize = 6.16F;

    [Space(5)] [Header("Camera View Swiping")]
    [SerializeField]
    [Range(0f, 300.0F)]
    private float swipeThreshold = 120.0F;

    [SerializeField] private float targetLeft    = 50f;
    [SerializeField] private float targetRight   = -40f;
    [SerializeField] private float initial       = 0f;

    [SerializeField]
    [Range(1.0F, 10.0F)]
    private float rotationSpeed = 5.0f;

    private Vector2 fingerDownPosition;
    private Vector2 fingerUpPosition;

    public bool IsRotating { private set; get; }
    public IsoCamViewingAngles CurrentViewAngle { private set; get; }

    private void Start()
    {
        if (autoAssignGameManager)
        {
            GameObject.FindWithTag(Constants.Tags.GameManager)
            .TryGetComponent(out gameManager);
        }

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

    private void Update()
    {
        if (Input.touchCount > 0)
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
                    // the swipe (threshold) sensitivity. This is to prevent false swipe such as
                    // any distance that is non-zero is considered as "Swipe".

                    var swipeLength = Vector2.Distance(fingerDownPosition, fingerUpPosition);

                    if (Mathf.Abs(swipeLength) > swipeThreshold)
                    {
                        SwitchViewDirection(fingerUpPosition);
                    }

                    // Reset the fingerUp position into fingerDown position.
                    // This is to prepare for the next consecutive swipes.
                    fingerUpPosition = fingerDownPosition;

                    break;
            }
        }
    }

    private void SwitchViewDirection(Vector2 fingerUpPosition)
    {
        if (gameManager != null && (gameManager.IsLevelPartiallyFinished || gameManager.IsGameOver()))
            return;

        var rotationY = transform.rotation.eulerAngles.y.WrapAngles();

        // Get the swipe direction

        // Swiped LEFT
        if (fingerUpPosition.x < fingerDownPosition.x && !IsRotating)
        {
            // To identify our current viewing rotation, we subtract
            // our current Y rotation from the last rotated direction
            // i.e. "initial" or "targetLeft". We then wrap it to
            // positive value and check if it is less than 0.01, we
            // can assume that we are in that direction and we wanted
            // to go the opposite direction.

            if (Mathf.Abs(rotationY - initial) < 0.01f)

                // From Center going to Right
                StartCoroutine(RotateToTarget(targetRight));

            else if (Mathf.Abs(rotationY - targetLeft) < 0.01f)

                // From Left back to Center
                StartCoroutine(RotateToTarget(initial));

        }

        // Swiped RIGHT
        if (fingerUpPosition.x > fingerDownPosition.x && !IsRotating)
        {
            if (Mathf.Abs(rotationY - initial) < 0.01f)

                // From Center going to Left
                StartCoroutine(RotateToTarget(targetLeft));

            else if (Mathf.Abs(rotationY - targetRight) < 0.01f)

                // From Right back to Center
                StartCoroutine(RotateToTarget(initial));
        }
    }

    public void LateUpdate()
    {
        var size = originalOrthographicSize * (originalAspectRatio / Camera.main.aspect);

        if (size > maxOrthoSize)
            size = maxOrthoSize;

        else if (size < minOrthoSize)
            size = minOrthoSize;

        Camera.main.orthographicSize = size;
    }

    private IEnumerator RotateToTarget(float target)
    {
        IsRotating = true;
        var startRotation = transform.rotation;

        var endRotation   = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            target,
            transform.rotation.eulerAngles.z
        );

        // Interoplation from startRotation to endRotation
        var t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }

        // Determine the viewing angle after rotation
        var targetRot = Convert.ToInt32(target);

        if (targetRot == Convert.ToInt32(targetLeft))
            CurrentViewAngle = IsoCamViewingAngles.Left;

        else if (targetRot == Convert.ToInt32(targetRight))
            CurrentViewAngle = IsoCamViewingAngles.Right;

        else if (targetRot == initial)
            CurrentViewAngle = IsoCamViewingAngles.Middle;

        Debug.LogWarning($"Viewangle => {CurrentViewAngle}");
        IsRotating = false;
    }
}