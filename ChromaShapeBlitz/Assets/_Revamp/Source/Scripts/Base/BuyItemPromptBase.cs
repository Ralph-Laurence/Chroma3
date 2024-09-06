using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Base class that handles the common business logic of the common store prompt dialog variants.
/// </summary>
public abstract class BuyItemPromptBase : MonoBehaviour
{
    [Header("Main Behaviour")]
    [SerializeField] protected Button     closeButton;
    [SerializeField] protected GameObject dialogContent;
    [SerializeField] protected AudioClip  soundOnShow;
    [SerializeField] protected AudioClip  soundOnHide;

    [Space(10)]
    [Header("Data Presentation")]
    [SerializeField] protected TextMeshProUGUI  itemLabel;
    [SerializeField] protected TextMeshProUGUI  promptMessage;
    [SerializeField] protected Image            itemPreview;
    [SerializeField] protected Button           buyButton;
    [SerializeField] protected TextMeshProUGUI  buyButtonText;

    protected SoundEffects  sfx;
    protected RectTransform dialogRect;
    protected Vector2       dialogPosition;

    /// <summary>
    /// The common "item name" that should be in the dialog's
    /// confirmation prompt such as "Do you want to buy this #item# ?"
    /// </summary>
    protected string BaseItemName { get; set; } = "item";

    /// <summary>
    /// Called after Awake() initialization has finished
    /// </summary>
    protected abstract void OnAwake();

    /// <summary>
    /// Invoked before transitioning to show the prompt
    /// </summary>
    protected Action OnBeforeShow;

    /// <summary>
    /// Invoked when the prompt was fully shown and done transitioning
    /// </summary>
    protected Action OnPromptShown;

    protected abstract void OnBuyButtonClick();

    public BaseItemData ItemData { get; set; }
    
    protected ShopItemCardBase EventSender;

    private bool isInitialized;
    
    private void Awake()
    {
        Initialize();
        OnAwake();
    }

    private void Initialize()
    {
        if (isInitialized)
            return;

        sfx = SoundEffects.Instance;

        dialogContent.TryGetComponent(out dialogRect);

        if (closeButton != null)
            closeButton.onClick.AddListener(Close);

        if (buyButton != null)
            buyButton.onClick.AddListener(() => OnBuyButtonClick());

        dialogPosition = new Vector2
        (
            dialogRect.anchoredPosition.x,
            -Screen.height + dialogRect.rect.height / 2.0F
        );

        isInitialized = true;
    }

    /// <summary>
    /// Grab a reference to the shop item card that triggered this prompt
    /// </summary>
    /// <param name="sender">The shop item card that triggered the prompt</param>
    public void SetEventSender(ShopItemCardBase sender)
    {
        EventSender = sender;
        ItemData    = sender.GetItemData();
    }

    /// <summary>
    /// Show the dialog with Slide In effect
    /// </summary>
    public void Show()
    {
        promptMessage.text  = $"Do you want to buy this {BaseItemName} for {FormatCostPrice(ItemData)} ?";
        itemPreview.sprite  = ItemData.PreviewImage;
        itemLabel.text      = ItemData.Name;

        OnBeforeShow?.Invoke();

        // ## BEGIN THE TRANSITION ## //

        // Set the initial position to the bottom of the screen
        dialogRect.anchoredPosition = dialogPosition;
        dialogContent.SetActive(true);

        PlaySfx(soundOnShow);

        // Slide-In to the middle of the screen
        LeanTween.moveY(dialogRect, 0.0F, 0.25F)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() => {
                     // buyButton.interactable = true;
                     OnPromptShown?.Invoke();
                 });
    }

    /// <summary>
    /// Close the dialog with Slide Out effect
    /// </summary>
    public void Close()
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
    /// Concatenate the price label with its required currency glyph
    /// </summary>
    /// <returns>Price string with glyph</returns>
    private string FormatCostPrice(BaseItemData itemData)
    {
        var cost = itemData.Cost == CurrencyType.Gem
                 ? $"<space=0.2em><color=#FF0CCB>{itemData.Price}\u00d7</color><size=140%>{Constants.CurrencySprites.GemCoin}</size>"
                 : $"<space=0.2em><color=#81D621>{itemData.Price}\u00d7</color><size=140%>{Constants.CurrencySprites.GoldCoin}</size>";

        return cost;

    }

    /// <summary>
    /// Play dialog slide sounds
    /// </summary>
    private void PlaySfx(AudioClip clip)
    {
        if (sfx == null)
            return;

        sfx.PlayOnce(clip);
    }

    /// <summary>
    /// Give the buy button a new text, with an option to make it interactable.
    /// </summary>
    /// <param name="text">The new text</param>
    /// <param name="interactable">Should the button be disabled or enabled?</param>
    public void SetBuyButtonText(string text, bool interactable = true)
    {
        buyButtonText.text = text;
        buyButton.interactable = interactable;
    }

    /// <summary>
    /// Revert the buy button's text back to "Buy" and make it interactable again.
    /// </summary>
    public void ResetBuyButton()
    {
        buyButtonText.text = "Buy";
        buyButton.interactable = true;
    }
}
