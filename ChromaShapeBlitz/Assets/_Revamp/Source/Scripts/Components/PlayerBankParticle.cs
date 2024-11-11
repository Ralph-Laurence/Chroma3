using TMPro;
using UnityEngine;

public class PlayerBankParticle : MonoBehaviour
{
    public CurrencyType Currency;
    public PlayerBankAnimationTypes AnimationType;
    public int Amount;

    [SerializeField] private Color decreaseColor = new(0.96F, 0.13F, 0.279F, 1.0F);
    [SerializeField] private Color increaseColor = new(0.14F, 0.85F, 0.03F, 1.0F);

    [SerializeField] private float fadeDuration  = 0.75F;
    [SerializeField] private float slideDistance = 75.0F;

    [SerializeField] private TextMeshProUGUI tmpText;
    [SerializeField] private TMP_StyleSheet styleSheet; // Reference to the Text Style Sheet
    [SerializeField] private string styleNameCoin = "Coin"; // Name of the style to apply
    [SerializeField] private string styleNameGem  = "Gem";

    private RectTransform rectTransform;

    private void Awake()
    {
        TryGetComponent(out rectTransform);
    }

    private void Start()
    {
        var style = Currency switch
        {
            CurrencyType.Coin => styleNameCoin,
            CurrencyType.Gem  => styleNameGem,
            _ => "Normal"
        };

        SetTextStyle(style);

        switch (AnimationType)
        {
            case PlayerBankAnimationTypes.Increase:
                tmpText.color = increaseColor;
                IncreaseMoney(Amount);
                break;

            case PlayerBankAnimationTypes.Decrease:
                tmpText.color = decreaseColor;
                DecreaseMoney(Amount);
                break;
        }
    }

    private void SetTextStyle(string styleName)
    {
        // Ensure the style sheet and style name are valid
        if (styleSheet != null && !string.IsNullOrEmpty(styleName))
        {
            // Find the style from the style sheet
            TMP_Style style = styleSheet.GetStyle(styleName);
            if (style != null)
            {
                // Apply the style to the TextMeshProUGUI component
                tmpText.fontStyle = FontStyles.Normal; // Reset any previous styles
                tmpText.textStyle = style; // Apply the new style
            }
            else
            {
                Debug.LogWarning("Style not found: " + styleName);
            }
        }
        else
        {
            Debug.LogWarning("StyleSheet or StyleName is not set.");
        }
    }

    public void DecreaseMoney(int amount)
    {
        // Step 1: Clone the original TextMeshPro
        tmpText.text = $"-{amount}";

        // LeanTween for sliding down
        var initialPosition = rectTransform.anchoredPosition;
        var endPoint = new Vector3
        (
            initialPosition.x,
            initialPosition.y - slideDistance
        );

        LeanTween.moveLocalY(gameObject, endPoint.y, fadeDuration).setEase(LeanTweenType.easeInOutQuad);

        // Tween for fadeout
        var initialColor = tmpText.color;

        LeanTween.value(gameObject, initialColor.a, 0f, fadeDuration - 0.25F)
                 .setDelay(0.25F)
                 .setOnUpdate((float val) =>
                 {
                     Color newColor = tmpText.color;
                     newColor.a = val;
                     tmpText.color = newColor;
                 })
                 .setOnComplete(() => Destroy(gameObject));
    }

    public void IncreaseMoney(int amount)
    {
        // Step 1: Clone the original TextMeshPro
        tmpText.text = $"+{amount}";

        // LeanTween for sliding down
        var initialPosition = rectTransform.anchoredPosition;
        var endPoint = new Vector3
        (
            initialPosition.x,
            initialPosition.y - slideDistance
        );
        
        LeanTween.moveLocalY(gameObject, endPoint.y, fadeDuration).setEase(LeanTweenType.easeInOutQuad);

        // Tween for fadeout
        var initialColor = tmpText.color;

        LeanTween.value(gameObject, initialColor.a, 0f, fadeDuration - 0.25F)
                 .setDelay(0.25F)
                 .setOnUpdate((float val) =>
                 {
                     Color newColor = tmpText.color;
                     newColor.a = val;
                     tmpText.color = newColor;
                 })
                 .setOnComplete(() => Destroy(gameObject));
    }
}
