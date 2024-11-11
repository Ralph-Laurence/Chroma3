using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PlayerCurrencyParticleAnimator : MonoBehaviour
{
    public static readonly int ANIMATION_MODE_DECREASE_AMOUNT = 0;
    public static readonly int ANIMATION_MODE_INCREASE_AMOUNT = 1;

    private TextMeshProUGUI textMeshPro; // Reference to the TextMeshProUGUI component
    [SerializeField] private float duration = 0.75f; // Duration of the fade-out animation
    [SerializeField] private float slideDownPositionOffset = 75.0F;
    
    [Space(5)]
    [Header("How far should this be from the indicator upon spawn")]
    public float SpawnYOffset = 35.0F;

    [SerializeField] private string coinDecreaseTextColor = "#FAB94D";
    [SerializeField] private string gemDecreaseTextColor = "#FF92E7";
    [SerializeField] private string currencyIncreaseTextColor = "#7DFF00";

    [SerializeField] private AudioClip sound;

    private readonly string defaultTextColor = "#FFFFFF";

    private SoundEffects sfx;
    void Awake()
    {
        TryGetComponent(out textMeshPro);
        sfx = SoundEffects.Instance;
    }

    /// <summary>
    /// Only use this when playOnAwake is false, so that
    /// we can apply values and properties before animating.
    /// </summary>
    /// 
    /// <param name="amount">The amount to be displayed</param>
    public void SetParams(int amount, CurrencyType currencyType, int mode)
    {
        var currency  = Constants.CurrencySprites.GoldCoin;
        var symbol    = mode == ANIMATION_MODE_DECREASE_AMOUNT ? "-" : "+";
        var textColor = defaultTextColor;
        
        switch (currencyType)
        {
            case CurrencyType.Gem:
                textColor = (mode == ANIMATION_MODE_INCREASE_AMOUNT) 
                          ? currencyIncreaseTextColor 
                          : gemDecreaseTextColor;
                
                currency  = Constants.CurrencySprites.GemCoin;
                break;

            case CurrencyType.Coin:
                textColor = (mode == ANIMATION_MODE_INCREASE_AMOUNT) 
                          ? currencyIncreaseTextColor 
                          : coinDecreaseTextColor;
                break;
        }

        var text = $"<color={textColor}><size=85%>{symbol}{amount}</size> <voffset=2>{currency}";
        textMeshPro.text = text;
    }

     // Function to fade out the text while sliding down
    public void Play()
    {
        sfx.PlayOnce(sound);

        // LeanTween for sliding down
        var initialPosition = textMeshPro.transform.position;
        var endPoint = new Vector3
        (
            initialPosition.x,
            initialPosition.y - slideDownPositionOffset
        );

        LeanTween.move(textMeshPro.gameObject, endPoint, duration).setEase(LeanTweenType.easeInOutQuad);
        
        // Tween for fadeout
        var initialColor = textMeshPro.color;

        LeanTween.value(gameObject, initialColor.a, 0f, duration)
                 .setOnUpdate((float val) =>
                 {
                     Color newColor = textMeshPro.color;
                     newColor.a = val;
                     textMeshPro.color = newColor;
                 })
                 .setOnComplete(() => Destroy(gameObject));
    }
}
