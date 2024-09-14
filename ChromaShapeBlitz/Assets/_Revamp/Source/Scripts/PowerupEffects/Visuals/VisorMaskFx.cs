using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VisorMaskFx : MonoBehaviour
{
    [SerializeField] private RectTransform scannerLine;
    [SerializeField] private RectTransform scannerRange;
    [SerializeField] private TextMeshProUGUI scanCaption;
    [SerializeField] private Slider scanProgress;
    [SerializeField] private AudioClip sfxVisorOn;
    [SerializeField] private AudioClip sfxVisorOff;

    private SoundEffects sfx;
    private bool transitionBegan;


    void Awake()
    {
        sfx = SoundEffects.Instance;
    }

    void OnEnable()
    {
        if (!transitionBegan)
            BeginTransition();
    }

    public void BeginTransition()
    {
        OnBeforeScan();

        sfx.PlayOnce(sfxVisorOn);

        var scanDuration = Constants.PowerupEffectValues.VISOR_SCAN_DURATION;

        // Start the tween for the slider and the moving object
        LeanTween.value(gameObject, UpdateTween, 0f, 1f, scanDuration).setOnComplete(OnScanComplete);
    }

    // This method will be called by LeanTween to update the slider value and move the object
    private void UpdateTween(float value)
    {
        // Update the slider value
        scanProgress.value = value;

        var scanPercent = Mathf.RoundToInt(scanProgress.value * 100.0F);
        scanCaption.text = $"Scanning... {scanPercent}%";

        // Calculate the new position for the moving object.
        // Linear direction, single transition
        // float parentHeight = scannerRange.rect.height;
        // float newY = Mathf.Lerp(0, -parentHeight, value);
        // scannerLine.anchoredPosition = new Vector2(scannerLine.anchoredPosition.x, newY);

        // Calculate the new position using PingPong for oscillation
        float parentHeight = scannerRange.rect.height;

        // The value will oscillate between 0 and 1 during the tween
        float pingPongValue = Mathf.PingPong(value * 2, 1f); // Multiply by 2 to complete an up and down oscillation in 1 second

        // Lerp between the top (0) and bottom (-parentHeight)
        float newY = Mathf.Lerp(0, -parentHeight, pingPongValue);

        // Apply the new position to the scanner line
        scannerLine.anchoredPosition = new Vector2(scannerLine.anchoredPosition.x, newY);
    }

    private void OnBeforeScan()
    {
        scannerLine.gameObject.SetActive(true);
        scanCaption.text = "Scanning...";
        scanProgress.value = 0.0F;

        transitionBegan = true;
    }

    private void OnScanComplete()
    {
        scannerLine.gameObject.SetActive(false);
        scanCaption.text = "Scan Complete!";
        scanProgress.value = 1.0F;

        StartCoroutine(EndTransition());
    }

    private IEnumerator EndTransition()
    {
        yield return new WaitForSeconds(0.45F);
        sfx.PlayOnce(sfxVisorOff);
        
        transitionBegan = false;

        gameObject.SetActive(false);
        yield return null;
    }

    // Update is called once per frame
    // void Update()
    // {
    //     if (Input.GetKeyUp(KeyCode.F1))
    //         BeginTransition();
    // }
}
