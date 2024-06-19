using UnityEngine;

public class PositionCamera : MonoBehaviour
{
    [SerializeField] private float horizontalOffset = 0f;

    public Camera mainCamera; // Assign your camera in the inspector
    public GameObject target; // Assign your target (stage) in the inspector
    public float bottomOffset = 1f; // Adjust this value to set the offset from the bottom of the screen

    void Update()
    {
        Vector3 targetPosition = target.transform.position;
        float cameraRotationX  = mainCamera.transform.rotation.eulerAngles.x;
        float orthographicSize = mainCamera.orthographicSize;

        // Calculate the vertical distance from the target based on the camera's rotation and orthographic size
        float verticalDistance = orthographicSize * Mathf.Tan(cameraRotationX * Mathf.Deg2Rad);

        // Set the camera's position to be at a point where the target appears at the bottom of the screen with the specified offset
        mainCamera.transform.position = new Vector3
        (
            targetPosition.x + horizontalOffset, 
            targetPosition.y + verticalDistance - bottomOffset, 
            mainCamera.transform.position.z
        );
    }
}