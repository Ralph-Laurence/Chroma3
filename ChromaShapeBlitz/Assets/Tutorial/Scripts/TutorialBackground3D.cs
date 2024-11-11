using UnityEngine;

public class TutorialBackground3D : MonoBehaviour
{
    [SerializeField] private Transform stage;
    [SerializeField] private float yPositionOffset = 9.0F;

    private void TransformBackground()
    {
        // Cache transforms
        var backgroundPos = transform.position;
        var backgroundRot = transform.eulerAngles;

        // Adjust final position and rotations
        backgroundPos.y = stage.position.y - yPositionOffset;
        backgroundRot.y = stage.eulerAngles.y;

        // Finally, set the background's position
        transform.SetPositionAndRotation
        (
            backgroundPos,
            Quaternion.Euler(backgroundRot)
        );
    }

    // Update is called once per frame
    void LateUpdate() => TransformBackground();
}
