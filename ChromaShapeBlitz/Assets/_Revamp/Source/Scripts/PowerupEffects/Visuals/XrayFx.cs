using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XrayFx : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private RawImage vcr;
    [SerializeField] private float vcrScrollRate;
    [SerializeField] private RectTransform scanRing;
    [SerializeField] private RectTransform outerRing;
    [SerializeField] private RectTransform innerRing;
    [SerializeField] private RectTransform crosshairX;
    [SerializeField] private RectTransform crosshairY;

    [SerializeField] private int scanFrequency = 5;
    [SerializeField] private float scanDuration = 4.0F;
    [SerializeField] private float scanRangeBoundOffset = 20.0F;
    private float vcrScroll;

    [Space(5)]
    [SerializeField] private AudioClip sfxBeginScan;
    [SerializeField] private AudioClip sfxEndScan;

    [Space(5)]
    [SerializeField] private TextMeshProUGUI scanCaption;
    [SerializeField] private GameObject scanCaptionComplete;
    [SerializeField] private Slider scanProgress;

    [Space(5)]
    [SerializeField] private GameObject postProcess;

    private bool isScanning;
    private SoundEffects sfx;

    void Awake()
    {
        sfx = SoundEffects.Instance;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        crosshairX.sizeDelta = new Vector2(Screen.width, 3.0F);
        crosshairY.sizeDelta = new Vector2(3.0F, Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        vcrScroll -= vcrScrollRate * Time.deltaTime;
        vcr.uvRect = new Rect(0, vcrScroll, 1.0F, 1.0F);
    }

    void OnEnable()
    {
        StartCoroutine(BeginScan());
    }

    private IEnumerator BeginScan()
    {   
        if (isScanning)
            yield break;

        isScanning = true;

        var scanPoints = new List<Vector2>(scanFrequency);

        for (var i = 0; i < scanFrequency; i++)
        {
            var scanPoint = GetRandomScanPoint();
            scanPoints.Add(scanPoint);
        }

        scanCaption.text = $"<color=#DDDAFF>SCANNING...</color> ";
        scanProgress.value = 0;

        sfx.PlayOnce(sfxBeginScan);
        
        ToggleScanUI(true);
        postProcess.SetActive(true);
        scanCaptionComplete.SetActive(false);
        vcrScroll = 0.0F;

        // Track the scan progress
        LeanTween.value(gameObject, 0.0F, 100.0F, scanDuration).setOnUpdate((float value) => {
            
            var scanPercent = Mathf.RoundToInt(value);
            scanCaption.text = $"<color=#DDDAFF>SCANNING...</color> <color=#AF9CFC>{scanPercent}%</color>";
            scanProgress.value = scanPercent;
        })
        .setEase(LeanTweenType.easeInOutQuad)
        .setOnComplete(() => {
            StartCoroutine(HandleCompleted());
        });

        var duration = scanDuration / scanPoints.Count;

        // Move the scanner ring to scan points
        for (var i = 0; i < scanPoints.Count; i++)
        {
            // The last scan point would be to reset the ring from its origin
            var scanPoint = (i == scanPoints.Count - 1) ? Vector2.zero : scanPoints[i];

            // Upwards Right must rotate the ring clockwise
            // Downwards Left must rotate the ring counterclockwise
            var spin = (scanPoint.x > 0.0F && scanPoint.y > 0.0F) ? 1 : -1;

            // Move the ring (UI elements needs to be moved locally)
            LeanTween.moveLocal(scanRing.gameObject, scanPoint, duration)
                     .setEase(LeanTweenType.easeInOutQuad)
                     .setOnUpdate(OnRingMoved);

            // Rotate the rings as the scanner moves
            RotateRing(spin, duration);

            yield return new WaitForSeconds(duration);
        }

        isScanning = false;
    }

    private void ToggleScanUI(bool toggle)
    {
        scanCaption.gameObject.SetActive(toggle);
        scanProgress.gameObject.SetActive(toggle);
        crosshairX.gameObject.SetActive(toggle);
        crosshairY.gameObject.SetActive(toggle);
        scanRing.gameObject.SetActive(toggle);
    }

    private IEnumerator HandleCompleted()
    {
        yield return new WaitForSeconds(0.15F);
        sfx.PlayOnce(sfxEndScan);
        ToggleScanUI(false);
        scanCaptionComplete.SetActive(true);

        yield return new WaitForSeconds(0.5F);

        isScanning = false;
        postProcess.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnRingMoved(float value)
    {
        crosshairX.anchoredPosition = new Vector2
        (
            0.0F,
            scanRing.anchoredPosition.y
        );

        crosshairY.anchoredPosition = new Vector2
        (
            scanRing.anchoredPosition.x,
            0.0F
        );
    }

    private void RotateRing(int direction, float duration)
    {
        var rotationAngle = 60.0F;

        // Get the current Z rotation of the outer image
        var outerRingCurrentZ = outerRing.eulerAngles.z;

        // Add the new rotation to the current Z rotation
        LeanTween.rotateZ(outerRing.gameObject, outerRingCurrentZ + (rotationAngle * direction), duration);

        // Get the current Z rotation of the inner image
        var innerRingCurrentZ = innerRing.eulerAngles.z;

        // Reverse the rotation of dash, opposite of outerRing
        LeanTween.rotateZ(innerRing.gameObject, innerRingCurrentZ + (rotationAngle * -direction), duration);
    }

    private Vector2 GetRandomScanPoint(bool wholeNumbers = true)
    {
        var screenScaledSize = GetUIBounds();

        // Allowed scan range are given in these offsets:
        // A screen width of 260 - offset (16) = 244
        var screenX = screenScaledSize.x - scanRangeBoundOffset;

        // We divide the screen height to account for 
        // the goggle's visual dark part (forehead and nose)
        var screenY = (screenScaledSize.y / 2.0F) - scanRangeBoundOffset;

        // Generate random scan points from those ranges
        var randomPointX = Random.Range(-screenX, screenX);
        var randomPointY = Random.Range(-screenY, screenY);

        // Just to be sure, we clamp the values to the screen size
        randomPointX = Mathf.Clamp(randomPointX, -screenX, screenX);
        randomPointY = Mathf.Clamp(randomPointY, -screenY, screenY);

        if (wholeNumbers)
        {
            randomPointX = Mathf.RoundToInt(randomPointX);
            randomPointY = Mathf.RoundToInt(randomPointY);
        }

        return new Vector2(randomPointX, randomPointY);
    }

    /// <summary>
    /// Gets the screen size which is already scaled by the canvas.
    /// This is NOT the physical screen size, but the canvas dimensions
    /// acting as "Screen Size".
    /// </summary>
    /// <returns>Canvas Scaled Screen Size</returns>
    private Vector2 GetUIBounds()
    {
        // Get the RectTransform of the Canvas (or root element that defines the whole screen area)
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // Get the scaled size of the canvas (this is the actual size in UI coordinates)
        Vector2 canvasSize = canvasRect.rect.size;

        // Divide by 2 to get the range from center to the edge (since the canvas is centered)
        Vector2 uiBounds = canvasSize / 2f;

        return uiBounds; // These are the boundary limits (e.g., X: ±260, Y: ±460 in your case)
    }

    /*
    private Vector2 GetUIBounds()
{
    // Get the RectTransform of the Canvas (or root element that defines the whole screen area)
    RectTransform canvasRect = canvas.GetComponent<RectTransform>();

    // Get the CanvasScaler component
    CanvasScaler canvasScaler = canvas.GetComponent<CanvasScaler>();

    // Get the screen size
    Vector2 screenSize = new Vector2(Screen.width, Screen.height);

    // Calculate the reference resolution and the current resolution scale factor
    Vector2 referenceResolution = canvasScaler.referenceResolution;
    float scaleFactor = Mathf.Max(screenSize.x / referenceResolution.x, screenSize.y / referenceResolution.y);

    // Calculate the canvas size in pixels
    Vector2 canvasSize = canvasRect.rect.size * scaleFactor;

    // Divide by 2 to get the range from center to the edge (since the canvas is centered)
    Vector2 uiBounds = canvasSize / 2f;

    return uiBounds; // These are the boundary limits
}

    */
}
