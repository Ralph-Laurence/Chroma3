using UnityEngine;
using System.Collections;

public class MastBeacon : MonoBehaviour
{
    [SerializeField] private float blinkSpeed = 2.0f;
    [SerializeField] private GameObject beacon;
    [SerializeField] private Material emissiveMaterial;

    [Header("Beacon orthocam adjustment")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float beaconScreenCornerThreshold = 0.7F;
    [SerializeField] private float beaconFlareMaxIntensity = 9.0F;
    [SerializeField] private float beaconFlareMinIntensity = 2.5F;

    private bool beginBlink = false;
    private LensFlare beaconFlare;

    void OnDisable()
    {
        beginBlink = false;
        StopCoroutine(nameof(Blink));
    }

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        beacon.TryGetComponent(out beaconFlare);
        beaconFlare.brightness = beaconFlareMinIntensity;

        beginBlink = true;
        StartCoroutine(nameof(Blink));
    }

    private void Update() => ForceBeaconLensFlareVisible();

    private IEnumerator Blink()
    {
        var blink = false;

        while (beginBlink)
        {
            blink = !blink;

            if (!blink)
                emissiveMaterial.DisableKeyword("_EMISSION");
            else
                emissiveMaterial.EnableKeyword("_EMISSION");

            beacon.SetActive(blink);
            
            yield return new WaitForSeconds(blinkSpeed);
        }
    }

    /// <summary>
    /// Make the lens flare visible when it is closer to the upper portion of screen.
    /// </summary>
    private void ForceBeaconLensFlareVisible()
    {
        var beaconPosition = mainCamera.WorldToViewportPoint(beacon.transform.position);

        // Viewport space is normalized and relative to the camera. 
        // The bottom-left of the camera is (0,0); the top-right is (1,1).
        var isNearCorner = beaconPosition.y >= beaconScreenCornerThreshold;

        beaconFlare.brightness = isNearCorner ? 
                                 beaconFlareMaxIntensity : 
                                 beaconFlareMinIntensity;
    }
}
