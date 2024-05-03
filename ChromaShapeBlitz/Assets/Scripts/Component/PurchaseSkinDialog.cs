using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseSkinDialog : MonoBehaviour
{
    [SerializeField] private GameObject overlayContainer;
    [SerializeField] private Image previewImage;
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI cantBuyButtonText;
    [SerializeField] private GameObject cantBuyButton;
    [SerializeField] private GameObject buyButton;

    private SkinInfo skinInfo;
    private BlockSkinShopItem purchasableSkinItem;

    public void SetSkinDetails(SkinInfo skinInfo) => this.skinInfo = skinInfo;
    public void SetEventSender(BlockSkinShopItem sender) => purchasableSkinItem = sender;

    private void InitializeComponent()
    {
        previewImage.sprite = skinInfo.PreviewImage;
        itemName.text       = skinInfo.Name;
        costLabel.text      = skinInfo.Cost.PrefixWithCurrencyIcon( skinInfo.Price );
    }

    public void Show()
    {
        InitializeComponent();
        overlayContainer.SetActive(true);
    }

    public void Close() => overlayContainer.SetActive(false);

    // Attach this into the UI event
    public void BuySkin() => purchasableSkinItem.HandlePurchase();

    /// <summary>
    /// Tell if there are enough coins or gems to buy the item.
    /// If so, unlock the Buy button.
    /// Or else, lock it.
    /// </summary>
    public void SetCanBuyItem(bool canBuyItem)
    {
        if (!canBuyItem)
        {
            var costText = "Funds";
            
            switch (skinInfo.Cost) 
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
