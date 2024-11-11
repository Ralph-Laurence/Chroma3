using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class XrayVisionFx : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform maskBackground;
    [SerializeField] private RectTransform parentMaskStencil;
    [SerializeField] private RectTransform ringScanner;
    [SerializeField] private RectTransform outerRing;
    [SerializeField] private RectTransform innerDash;
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private RectTransform crosshairX;
    [SerializeField] private RectTransform crosshairY;
    [SerializeField] private TextMeshProUGUI scanCaption;

    [SerializeField] private int maxScanPoints = 6;
    [SerializeField] private float scanDuration = 3.0F;

    [SerializeField] private float scanRadius;

    [Space(5)]
    [SerializeField] private AudioClip sfxBeginScan;
    [SerializeField] private AudioClip sfxEndScan;

    private bool isScanning;
    private Vector2 backgroundSize;
    private SoundEffects sfx;

    void Awake()
    {
        sfx = SoundEffects.Instance;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
    //     RepositionBackground();
    // }

    void OnEnable()
    {
        RepositionBackground();
        
        if (!isScanning)
            StartCoroutine( BeginScanRandomPoints() );
    }

    // void Update()
    // {
    //     if (!isScanning && Input.GetKeyUp(KeyCode.F1))
    //         StartCoroutine( BeginScanAtRandomPoints() );
    // }

    /// <summary>
    /// Make the background stretch the whole screen whenever the parent moves
    /// </summary>
    private void RepositionBackground()
    {
        backgroundSize = new
        (
            Screen.width + (Screen.width / 2.0F),
            Screen.height
        );

        maskBackground.anchoredPosition = Vector2.zero;
        maskBackground.sizeDelta = backgroundSize;
    }

    private void RepositionCrosshair()
    {
        crosshairX.sizeDelta = new Vector2(Screen.width, 3.0F);
        crosshairY.sizeDelta = new Vector2(3.0F, Screen.height);

        crosshairX.anchoredPosition = new Vector2
        (
            0.0F,
            ringScanner.anchoredPosition.y
        );

        crosshairY.anchoredPosition = new Vector2
        (
            ringScanner.anchoredPosition.x,
            0.0F
        );
    }

    private IEnumerator BeginScanRandomPoints()
    {
        isScanning = true;

        sfx.PlayOnce(sfxBeginScan);

        // Track the scan progress
        LeanTween.value(gameObject, 0.0F, 100.0F, scanDuration).setOnUpdate((float value) => {
            
            var scanPercent = Mathf.RoundToInt(value);
            scanCaption.text = $"<color=#035B76>SCANNING...</color> <color=#FCA906>{scanPercent}%</color>";
        })
        .setEase(LeanTweenType.easeInOutQuad);

        // Animate the scanning
        var duration  = scanDuration / maxScanPoints;

        for (int i = 0; i < maxScanPoints; i++)
        {
            if (i == maxScanPoints - 1)
            {
                // Move back to origin
                LeanTween.move(parentMaskStencil, Vector2.zero, duration)
                         .setOnUpdate(OnTweenUpdate);

                // Rotate the image back to 0 degrees on the Z-axis
                LeanTween.rotateZ(outerRing.gameObject, 0, duration).setOnComplete(() => {

                    sfx.PlayOnce(sfxEndScan);
                    isScanning = false;

                    gameObject.SetActive(false);
                });
            }
            else
            {
                var randomScanPoint = GetRandomPointAtBottomHalfScreen(); //GetRandomPoint();
                // Upwards Right must rotate the ring clockwise
                // Downwards Left must rotate the ring counterclockwise
                var spin = (randomScanPoint.x > 0.0F && randomScanPoint.y > 0.0F) 
                         ? 1
                         : -1;

                // Move the scanner ring to scan points
                LeanTween.move(parentMaskStencil, randomScanPoint, duration)
                         .setEase(LeanTweenType.easeInOutQuad)
                         .setOnUpdate(OnTweenUpdate);

                // Rotate the ring as the scanner moves
                RotateImage(spin, duration);
            }

            yield return new WaitForSeconds(duration);
        }

        yield return null;
    }

    private void RotateImage(int direction, float duration)
    {
        var rotationAngle = 60.0F;

        // Get the current Z rotation of the outer image
        var outerRingCurrentZ = outerRing.eulerAngles.z;

        // Add the new rotation to the current Z rotation
        LeanTween.rotateZ(outerRing.gameObject, outerRingCurrentZ + (rotationAngle * direction), duration);

        // Get the current Z rotation of the dash image
        var dashRingCurrentZ = innerDash.eulerAngles.z;

        // Reverse the rotation of dash, opposite of outerRing
        LeanTween.rotateZ(innerDash.gameObject, dashRingCurrentZ + (rotationAngle * -direction), duration);
    }

    private void OnTweenUpdate(float value)
    {
        RepositionBackground();
        RepositionCrosshair();
        ringScanner.anchoredPosition = parentMaskStencil.anchoredPosition;
    }

    // private void OscillateCrosshair()
    // {
    //      LeanTween.scale(crosshair, new Vector3(1.5F, 1.5F, 1.0F), 0.25F)
    //               .setEaseInOutSine()   // Smooth ease in and out effect
    //               .setLoopPingPong();   // Looping back and forth
    // }

    private Vector2 GetRandomPoint()
    {
        // Get a random point from a circle of given radius
        var point = Random.insideUnitCircle * scanRadius;

        // Ensure that values are not lower than 1
        //point.x = Mathf.Max(1, point.x);
        //point.y = Mathf.Max(1, point.y);

        // Make sure that the points are whole numbers
        point.x = Mathf.RoundToInt(point.x);
        point.y = Mathf.RoundToInt(point.y);

        return point;
    }

    private Vector2 GetRandomPointAtBottomHalfScreen()
    {
        // Get the screen width and height in canvas units
        float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;

        // Calculate the center point for the radius
        float centerX = canvasWidth / 2;
        float centerY = canvasHeight / 4; // Center of the bottom half

        // Generate a random angle
        float angle = Random.Range(0f, Mathf.PI * 2);

        // Generate a random distance within the radius
        float distance = Random.Range(0f, scanRadius);

        // Calculate the random position within the radius
        float randomX = centerX + distance * Mathf.Cos(angle);
        float randomY = centerY + distance * Mathf.Sin(angle);

        // Ensure the position is within the canvas bounds
        randomX = Mathf.Clamp(randomX, 0, canvasWidth);
        randomY = Mathf.Clamp(randomY, 0, canvasHeight / 2);

        // Set the new position for the UI Image
        // uiImage.anchoredPosition = new Vector2(randomX, randomY);

        // Set the new position for the UI Image
        return new Vector2(randomX, randomY);
    }
}
