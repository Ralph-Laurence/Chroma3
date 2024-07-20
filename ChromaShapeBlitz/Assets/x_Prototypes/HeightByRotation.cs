using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class HeightByRotation : MonoBehaviour
{
    public RectTransform rectTransform;
    public float initialSize = 120f;

    void Start()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        // Get the current z rotation in radians
        float zRotation = rectTransform.localEulerAngles.z * Mathf.Deg2Rad;

        // Calculate the new height
        float newHeight = CalculateHeight(zRotation, initialSize);
        newHeight = Mathf.Clamp(newHeight, 120, 145);
        // Set the new height
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);
    }

    float CalculateHeight(float angle, float size)
    {
        // Calculate the diagonal of the square
        float diagonal = size * Mathf.Sqrt(2);

        // Calculate the height based on the rotation angle
        float newHeight = Mathf.Abs(Mathf.Cos(angle) * size) + Mathf.Abs(Mathf.Sin(angle) * diagonal);

        return newHeight;
    }
}
