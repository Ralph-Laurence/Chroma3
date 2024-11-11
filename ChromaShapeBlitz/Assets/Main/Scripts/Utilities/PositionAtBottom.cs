using UnityEngine;

public class PositionAtBottom : MonoBehaviour
{
    [SerializeField] private float horizontalOffset = 0f;
    [SerializeField] private bool autoFindMaincamera = true;

    public Camera mainCamera; // Assign your camera in the inspector
    public float bottomOffset = 1f; // Adjust this value to set the offset from the bottom of the screen
    

    void Start()
    {
        if (autoFindMaincamera || mainCamera == null)
            GameObject.FindWithTag(Constants.Tags.MainCamera).TryGetComponent(out mainCamera);
    }

    void Update()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        float cameraRotationX  = mainCamera.transform.rotation.eulerAngles.x;
        float orthographicSize = mainCamera.orthographicSize;

        // Calculate the vertical distance from the camera based on its rotation and orthographic size
        float verticalDistance = orthographicSize * Mathf.Tan(cameraRotationX * Mathf.Deg2Rad);

        // Set the model's position to be at the bottom of the screen with the specified offset
        transform.position = new Vector3
        (
            cameraPosition.x + horizontalOffset, 
            cameraPosition.y - verticalDistance + bottomOffset, 
            cameraPosition.z
        );
    }
}