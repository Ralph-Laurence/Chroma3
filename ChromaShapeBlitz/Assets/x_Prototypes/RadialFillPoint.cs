using UnityEngine;
using UnityEngine.UI;

public class RadialFillPoint : MonoBehaviour
{
    public Image radialImage;
    public RectTransform startPointMarker;
   // public RectTransform endPointMarker;

    void Update()
    {
        // Ensure the image is using Radial 360 fill method
        if (radialImage.type != Image.Type.Filled || radialImage.fillMethod != Image.FillMethod.Radial360)
        {
            Debug.LogWarning("Image must be set to Filled with Radial360 fill method.");
            return;
        }

        // Get the current fill amount (0 to 1)
        float fillAmount = radialImage.fillAmount;

        // Convert fill amount to angles (0 to 360 degrees)
        float startAngle = 0;
        float endAngle = fillAmount * 360f;

        // Calculate the radius
        float radius = radialImage.rectTransform.rect.width / 2;

        // Calculate the start point using trigonometry
        float startX = Mathf.Cos(startAngle * Mathf.Deg2Rad) * radius;
        float startY = Mathf.Sin(startAngle * Mathf.Deg2Rad) * radius;

        // Calculate the end point using trigonometry
        float endX = Mathf.Cos(endAngle * Mathf.Deg2Rad) * radius;
        float endY = Mathf.Sin(endAngle * Mathf.Deg2Rad) * radius;

        // Adjust the startPointMarker and endPointMarker positions
        startPointMarker.anchoredPosition = new Vector2(startX, startY);
        //endPointMarker.anchoredPosition = new Vector2(endX, endY);
    }
}
