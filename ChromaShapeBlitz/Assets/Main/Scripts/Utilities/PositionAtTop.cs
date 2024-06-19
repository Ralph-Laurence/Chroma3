using UnityEngine;

public class PositionAtTop : MonoBehaviour
{
    [SerializeField] private float horizontalOffset = 0f;
    public Camera mainCamera; // Assign your camera in the inspector
    public float topOffset = 1f; // Adjust this value to set the offset from the top of the screen
    
    void Update()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        float cameraRotationX  = mainCamera.transform.rotation.eulerAngles.x;
        float orthographicSize = mainCamera.orthographicSize;

        // Calculate the vertical distance from the camera based on its rotation and orthographic size
        float verticalDistance = orthographicSize * Mathf.Tan(cameraRotationX * Mathf.Deg2Rad);

        // Set the model's position to be at the top of the screen with the specified offset
        transform.position = new Vector3
        (
            cameraPosition.x + horizontalOffset, 
            cameraPosition.y + verticalDistance - topOffset, 
            cameraPosition.z
        );
    }
}
