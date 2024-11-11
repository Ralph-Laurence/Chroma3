
using InspectorUtils;
using System;
using TMPro;
using UnityEngine;

public enum PlayerBankAnimationTypes
{
    Increase,
    Decrease
}

public class PlayerBankAnimation : MonoBehaviour
{
    [Help("This is needed for the clone particle to display on top of anything.")]
    [SerializeField] private Transform particleSpawnParent;
    [SerializeField] private RectTransform canvasRectTransform;

    [Space(10)]
    [SerializeField] private GameObject textParticleAnimation;
    [SerializeField] private AudioClip sfxDecreased;
    [SerializeField] private AudioClip sfxIncreased;

    private readonly string defaultTextColor = "#FFFFFF";
    private LTDescr tweenID;
    private RectTransform rectTransform;
    private SoundEffects sfx;

    private void Awake()
    {
        sfx = SoundEffects.Instance;
        TryGetComponent(out rectTransform);
    }

    [SerializeField] TextMeshProUGUI tmpText;
    public CurrencyType Currency { private get; set; }

    private void OnEnable()
    {
        PlayerBankAnimationNotifier.BindObserver(ObserveAnimationEvents);
    }

    private void OnDisable()
    {
        PlayerBankAnimationNotifier.UnbindObserver(ObserveAnimationEvents);
    }

    private void ObserveAnimationEvents(PlayerBankAnimationParams data)
    {
        if (data.Currency != Currency)
            return;

        var sound = data.AnimationType == PlayerBankAnimationTypes.Increase ? sfxIncreased : sfxDecreased;
        sfx.PlayOnce(sound);

        AnimateOriginal(data.CurrentValue, data.Amount, data.AnimationType);
        AnimateParticle(data.AnimationType, data.Currency, data.Amount);
    }

    private void AnimateParticle(PlayerBankAnimationTypes animType, CurrencyType currency, int amount)
    {
        var obj = Instantiate(textParticleAnimation, particleSpawnParent);
        obj.TryGetComponent(out RectTransform rect);   
        obj.TryGetComponent(out PlayerBankParticle particle);

        MatchCloneToOriginalPosition(rect, rectTransform);

        particle.AnimationType = animType;
        particle.Currency      = currency;
        particle.Amount        = amount;
    }

    private void AnimateOriginal(int current, int amount, PlayerBankAnimationTypes animationType)
    {
        int targetMoney = animationType == PlayerBankAnimationTypes.Decrease
                        ? current - amount
                        : current + amount;

        tweenID?.reset();
        tweenID = LeanTween.value(gameObject, UpdateMoneyText, current, targetMoney, 0.75F)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() => current = targetMoney);
    }

    private void UpdateMoneyText(float value)
    {
        tmpText.text = Mathf.RoundToInt(value).ToString();
    }

    /// <summary>
    /// Position the clone exactly at the original target's position
    /// </summary>
    public void MatchCloneToOriginalPosition(RectTransform clone, RectTransform original)
    {
        // Step 1: Get the target's world position
        var worldPosition = original.TransformPoint(Vector3.zero);

        // Step 2: Calculate the center position offset of the target in local space
        var targetCenterOffset = new Vector2
        (
            (0.5F - original.pivot.x) * original.rect.width,
            (0.5F - original.pivot.y) * original.rect.height
        );

        // Step 3: Convert the target's center to world space
        var targetCenterWorldPosition = worldPosition + original.TransformVector(targetCenterOffset);

        // Step 4: Convert the world position of the center to the canvas local space
        RectTransformUtility.ScreenPointToLocalPointInRectangle
        (
            canvasRectTransform,
            RectTransformUtility.WorldToScreenPoint(null, targetCenterWorldPosition),
            null,
            out Vector2 localPoint
        );

        // Change the anchor of the cloned element into the middle screen.
        // The middle anchor should serve as the element's origin / pivot
        // so that we can move it anywhere and perform calculations easily.
        var center = Vector2.one * 0.5F;

        clone.pivot = center;
        clone.anchorMin = center;
        clone.anchorMax = center;
        clone.sizeDelta = new Vector3(original.rect.width, original.rect.height, 1.0F); //original.sizeDelta;

        // Step 5: Assign this position to the cursor's anchored position
        clone.anchoredPosition = localPoint;
    }

    public Vector2 GetUIBounds(bool whole = false)
    {
        // Get the scaled size of the canvas (this is the actual size in UI coordinates)
        Vector2 canvasSize = canvasRectTransform.rect.size;

        // Divide by 2 to get the range from center to the edge (since the canvas is centered)
        Vector2 uiBounds = canvasSize / 2f;

        // These are the boundary limits (e.g., X: �260, Y: �460 in your case)
        if (!whole)
            return uiBounds;

        // Make the values whole number
        uiBounds.x = Convert.ToInt32(uiBounds.x);
        uiBounds.y = Convert.ToInt32(uiBounds.y);

        return uiBounds;
    }
}
