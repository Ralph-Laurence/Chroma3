using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrophyRewardAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform trophyRect;
    [SerializeField] private Image starBurstRays;
    [SerializeField] private GameObject confettiVfx;
    [SerializeField] private AudioClip confettiSfx;
    [SerializeField] private TextMeshProUGUI rewardsText;
    [SerializeField] private TextMeshProUGUI levelsText;
    [SerializeField] private Button continueButton;
    
    [Tooltip("Array of RectTransforms to pulse sequentially")]
    [SerializeField] private RectTransform[] rectTransformSequence;

    [SerializeField] private GameObject trophyMesh;
    [SerializeField] private float trophySlideInRate = 1.25F;
    [SerializeField] private float trophySpinRate = 90.0F;
    
    [Tooltip("Maximum displacement from the start position")]
    [SerializeField] private float trophyOscillationAmplitude = 0.25F;
    
    [Tooltip("Oscillations per second")]
    [SerializeField] private float trophyOscillationFrequency = 1.0F;

    [SerializeField] private float starBurstRaysOpacity = 0.16F;
    [SerializeField] private float raysFadeRate = 0.25F;

    private SoundEffects soundEffects;
    private Vector2 trophyStartPosition;
    private Vector2 trophyPositionOnShown;
    private RectTransform raysRect;
    private bool beginBounceTrophy;

    private readonly float pulsedMaxScale = 1.25F;
    private readonly float pulseDuration = 0.15F;

    void Awake()
    {
        soundEffects = SoundEffects.Instance;
        starBurstRays.TryGetComponent(out raysRect);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize();
        SlideInTrophy();
    }

    // Update is called once per frame
    void Update()
    {
        trophyMesh.transform.Rotate(Time.deltaTime * trophySpinRate * Vector3.up);

        if (beginBounceTrophy)
            OscillateTrophy();
    }

    private void Initialize()
    {
        foreach(var rects in rectTransformSequence)
        {
            rects.localScale = Vector3.zero;
        }

        trophyStartPosition = new Vector2
        (
            trophyRect.anchoredPosition.x,
            -Screen.height / 2.0F - trophyRect.rect.height / 2.0F
        );

        trophyMesh.transform.Rotate(Vector3.up * -180.0F);
    }

    /// <summary>
    /// Show the trophy object
    /// </summary>
    private void SlideInTrophy()
    {
        // Set the initial position to the left of the screen
        trophyRect.anchoredPosition = trophyStartPosition;
        trophyRect.gameObject.SetActive(true);
        
        // Slide-In to the middle of the screen
        LeanTween.moveY(trophyRect, 0.0F, trophySlideInRate)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() => OnTrophyShown());
    }

    private void OnTrophyShown()
    {
        // Show the rays
        LeanTween.alpha(raysRect, starBurstRaysOpacity, raysFadeRate);

        trophyPositionOnShown = trophyRect.anchoredPosition;

        // Make the trophy go oscillate up and down
        beginBounceTrophy = true;

        // Show the confetti
        confettiVfx.SetActive(true);

        if (soundEffects != null)
            soundEffects.PlayOnce(confettiSfx);

        // Show the UI elements
        StartCoroutine(PulseSequentially());
    }

    private void OscillateTrophy()
    {
        var newY = trophyPositionOnShown.y + Mathf.Sin(Time.time * trophyOscillationFrequency * 2 * Mathf.PI) * trophyOscillationAmplitude;

        trophyRect.anchoredPosition = new Vector2(trophyPositionOnShown.x, newY);
    }

    private IEnumerator ShowRectTransformPulsed(RectTransform rectTransform)
    {
       var animationCompleted = false;

        rectTransform.localScale = Vector3.zero; // Start at scale 0

        LeanTween.scale(rectTransform, new Vector3(pulsedMaxScale, pulsedMaxScale, 1), pulseDuration)
                 .setEaseOutQuad()
                 .setOnComplete(() =>
                 {
                     LeanTween.scale(rectTransform, new Vector3(1, 1, 1), pulseDuration)
                              .setEaseInQuad()
                              .setOnComplete(() => animationCompleted = true);
                 });

        // Wait until the animation completes
        yield return new WaitUntil(() => animationCompleted);
    }

    private IEnumerator PulseSequentially()
    {
        foreach (var rectTransform in rectTransformSequence)
        {
            yield return StartCoroutine(ShowRectTransformPulsed(rectTransform));
        }
    }

    public void SetParams(LevelDifficulties level, int totalLevels, int rewardGoldCoin, int rewardGemCoin, Action continueButtonAction)
    {
        var icnCoin  = Constants.CurrencySprites.GoldCoin;
        var icnGem   = Constants.CurrencySprites.GemCoin;
        var multiply = '\u00d7';

        var reward = $"<size=130%>{icnCoin}<size=100%>{multiply}{rewardGoldCoin}<space=1.5em>" + 
                     $"<size=130%>{icnGem}<size=100%>{multiply}{rewardGemCoin}";

        rewardsText.text = reward;
        levelsText.text = $"{level}<space=0.25rem>1-{totalLevels}";

        if (continueButton != null && continueButtonAction != null)
            continueButton.onClick.AddListener(() => continueButtonAction.Invoke());
    }
}
