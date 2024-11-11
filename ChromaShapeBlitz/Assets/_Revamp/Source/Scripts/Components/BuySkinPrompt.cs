using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuySkinPrompt : MonoBehaviour
{
    [Header("Main Behaviour")]
    [SerializeField] private GameObject dialogContent;
    [SerializeField] private AudioClip soundOnShow;
    [SerializeField] private AudioClip soundOnHide;

    [Space(10)] [Header("Data Presentation")]
    [SerializeField] private TextMeshProUGUI itemLabel;
    [SerializeField] private TextMeshProUGUI promptMessage;
    [SerializeField] private Image itemPreview;
    [SerializeField] private Button buyButton;

    private SoundEffects sfx;
    private RectTransform dialogRect;
    private Vector2 dialogPosition;
    private SkinItemData itemData;
    private SkinShopItemCard eventSender;

    private bool isInitialized;

    private void Initialize()
    {
        if (isInitialized)
            return;

        dialogContent.TryGetComponent(out dialogRect);
        sfx = SoundEffects.Instance;

        dialogPosition = new Vector2
        (
            dialogRect.anchoredPosition.x,
            -Screen.height + dialogRect.rect.height / 2.0F
        );

        isInitialized = true;
    }

    public void SetEventSender(SkinShopItemCard sender)
    {
        eventSender = sender;
        itemData = sender.GetItemData();
    }

    public void Show()
    {
        Initialize();
        var spriteGem = 3;
        var spriteCoin = 2;

        var cost = itemData.CostCurrency == CurrencyType.Gem
                 ? $"<space=0.2em><color=#CF00FF>{itemData.Price}\u00d7</color><size=140%><sprite={spriteGem}></size>"
                 : $"<space=0.2em><color=#81D621>{itemData.Price}\u00d7</color><size=140%><sprite={spriteCoin}></size>";

        promptMessage.text = $"Do you want to buy this block skin for {cost} ?";
        itemPreview.sprite = itemData.PreviewImage;
        itemLabel.text     = itemData.Name;
        
        // Set the initial position to the bottom of the screen
        dialogRect.anchoredPosition = dialogPosition;
        dialogContent.SetActive(true);

        PlaySfx(soundOnShow);

        // Slide-In to the middle of the screen
        LeanTween.moveY(dialogRect, 0.0F, 0.25F)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() => {
                    buyButton.interactable = true;
                 });
    }

    /// <summary>
    /// Assign this into the close button's click event
    /// </summary>
    public void Ev_HidePrompt() => Hide();

    public void Hide()
    {
        PlaySfx(soundOnHide);

        // Slide-In to the middle of the screen
        LeanTween.moveY(dialogRect, dialogPosition.y, 0.25F)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() => {

                    dialogContent.SetActive(false);
                    gameObject.SetActive(false);
                 });
    }

    /// <summary>
    /// Assign this into Buy button's click event
    /// </summary>
    public void Ev_Buy()
    {
        buyButton.interactable = false;
        BlockSkinItemPurchaseNotifier.NotifyObserver(eventSender);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (sfx == null)
            return;

        sfx.PlayOnce(clip);
    }
}
