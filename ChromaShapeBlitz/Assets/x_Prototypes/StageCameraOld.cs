using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using System;

public class StageCameraOld : MonoBehaviour
{
    [Header("Camera Dragging")]
    [SerializeField][Range(0.0F, 10.0F)] private float rotationSpeed = 0.08F;

    [SerializeField][Range(0.0F, 30.0F)] private float minDragDistance = 20.0F;
    [SerializeField] private float smoothTime = 0.05F; // Time it takes to reach the target rotation

    private Vector2 touchStart;
    private bool isDragging;
    private float targetRotationY;
    private float currentVelocity;

    private void Start() => targetRotationY = transform.eulerAngles.y;

    [Obsolete]
    private void HandleDragOld()
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