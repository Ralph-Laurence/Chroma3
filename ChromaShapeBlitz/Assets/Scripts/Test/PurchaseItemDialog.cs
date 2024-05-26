using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseItemDialog : MonoBehaviour
{
    [SerializeField] private GameObject overlayContainer;
    [SerializeField] private Image previewImage;
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI cantBuyButtonText;
    [SerializeField] private GameObject cantBuyButton;
    [SerializeField] private GameObject buyButton;

    private BaseItemInfo itemInfo;

    private ShopItemBase eventSender;

    public void SetItemDetails(BaseItemInfo itemInfo) => this.itemInfo = itemInfo;
    public void SetEventSender(ShopItemBase eventSender) => this.eventSender = eventSender;

    private void InitializeComponent()
    {
        previewImage.sprite = itemInfo.PreviewImage;
        itemName.text       = itemInfo.Name;
        costLabel.text      = itemInfo.Cost.PrefixWithCurrencyIcon( itemInfo.Price );
    }

    public void Show()
    {
        InitializeComponent();
        overlayContainer.SetActive(true);
    }

    public void Close() => overlayContainer.SetActive(false);

    // Attach this into the UI event
    public void BuyItem() => eventSender.HandlePurchase();

    /// <summary>
    /// Tell if there are enough coins or gems to buy the item.
    /// If so, unlock the Buy button, or else, lock it.
    /// </summary>
    public void SetCanBuy(bool canBuy)
    {
        if (!canBuy)
        {
            var costText = "Funds";
            
            switch (itemInfo.Cost) 
            {
                case CurrencyType.Coin: costText = "Coins"; break;
                case CurrencyType.Gem:  costText = "Gems"; break;
            };

            buyButton.SetActive(false);
            cantBuyButtonText.text = $"Need more {costText}";
            cantBuyButton.SetActive(true);
            return;
        }

        cantBuyButton.SetActive(false);
        buyButton.SetActive(true);
    }
}
