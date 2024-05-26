using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class BuySkinDialog : MonoBehaviour
{
    [Space(5)]
    [Header("Dialog Styles")]
    [SerializeField] private BuySkinDialogStylesSO blueDialogStyle;
    [SerializeField] private BuySkinDialogStylesSO greenDialogStyle;
    [SerializeField] private BuySkinDialogStylesSO magentaDialogStyle;
    [SerializeField] private BuySkinDialogStylesSO orangeDialogStyle;
    [SerializeField] private BuySkinDialogStylesSO purpleDialogStyle;
    [SerializeField] private BuySkinDialogStylesSO yellowDialogStyle;

    [Space(5)]
    [Header("Data Presentation")]
    [SerializeField] private Image mainOverlay;
    [SerializeField] private Image dialogBackground;
    [SerializeField] private Image previewImage;
    [SerializeField] private Image controlButtonBackground;
    [SerializeField] private Image[] itemNameBackgrounds;
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private GameObject buyButton;

    [Space(5)]
    [SerializeField] private GameObject cantBuyPrompt;
    [SerializeField] private TextMeshProUGUI cantBuyPromptText;
    [SerializeField] private AudioClip cantBuySfx;
    
    private BaseItemInfo itemInfo;
    private ShopItemBase eventSenderData;
    private Dictionary<ColorSwatches, BuySkinDialogStylesSO> dialogAppearance;

    private bool canBuy;

    public void SetItemDetails(BaseItemInfo itemInfo) => this.itemInfo = itemInfo;
    public void SetEventSenderData(ShopItemBase eventSender) => eventSenderData = eventSender;

    private void InitializeComponent()
    {
        if (dialogAppearance == null)
        {
            dialogAppearance = new Dictionary<ColorSwatches, BuySkinDialogStylesSO>
            {
                { ColorSwatches.Blue    , blueDialogStyle    },
                { ColorSwatches.Green   , greenDialogStyle   },
                { ColorSwatches.Magenta , magentaDialogStyle },
                { ColorSwatches.Orange  , orangeDialogStyle  },
                { ColorSwatches.Purple  , purpleDialogStyle  },
                { ColorSwatches.Yellow  , yellowDialogStyle  },
            };
        }

        previewImage.sprite = itemInfo.PreviewImage;
        itemName.text       = itemInfo.Name;
        costLabel.text      = itemInfo.Cost.PrefixWithCurrencyIcon( itemInfo.Price );

        var style = dialogAppearance[itemInfo.ColorCategory];

        mainOverlay.color               = style.OverlayColor;
        dialogBackground.sprite         = style.DialogBackground;
        controlButtonBackground.color   = style.ControlButtonsBackground;
        promptText.color                = style.PromptTextColor;

        foreach (var bg in itemNameBackgrounds)
        {
            bg.color = style.TitleColor;
        }

        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);
    }

    public void Show()
    {
        InitializeComponent();
        mainOverlay.gameObject.SetActive(true);
    }

    public void Close() => mainOverlay.gameObject.SetActive(false);

    // Attach this into the UI event
    // public void BuyItem() => eventSender.HandlePurchase();
    public void BuyItem()
    {
        if (!canBuy)
        {
            gameObject.SetActive(false);
            cantBuyPromptText.text = $"Not enough {itemInfo.Cost.ToCurrencyName()}";
            cantBuyPrompt.SetActive(true);

            SfxManager.Instance.PlaySfx(cantBuySfx);

            return;
        }

        eventSenderData.HandlePurchase();
    }
    /// <summary>
    /// Tell if there are enough coins or gems to buy the item.
    /// </summary>
    public void SetCanBuy(bool canBuy) => this.canBuy = canBuy;
}