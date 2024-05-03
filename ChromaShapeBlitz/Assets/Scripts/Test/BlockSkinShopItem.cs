using System.Collections;

public class BlockSkinShopItem : ShopItem
{
    private SkinInfo skinInfo;
    public override void HandleItemClicked() => OnPurchasableSkinClickedNotifier.Publish(this);
    public override void HandlePurchase() => StartCoroutine(PurchaseSkin());

    public override void InitializeComponent()
    {
        ItemPreviewImage.sprite = skinInfo.PreviewImage;
        ItemNameLabel.text      = skinInfo.Name;
        ItemPriceLabel.text     = skinInfo.Cost.PrefixWithCurrencyIcon(skinInfo.Price);
    }

    public void SetSkinInfo(SkinInfo skinInfo)
    {
        this.skinInfo = skinInfo;
        InitializeComponent();
    }
    
    public SkinInfo GetSkinInfo() => skinInfo;

    private IEnumerator PurchaseSkin()
    {
        var playerProgress = PlayerProgressHelper.Instance;

        // Save the changes into disk
        yield return StartCoroutine
        (
            playerProgress.DecreasePlayerBank(skinInfo.Cost, skinInfo.Price, true)
        );

        yield return StartCoroutine(themeHelper.TakeSkinOwnership(skinInfo.Id, skinInfo.ColorCategory));
        
        skinInfo.IsOwned = true;
        
        ToggleEquipped();
        HidePriceLabel();

        // Notify the purchase confirmation dialog to close
        OnSkinPurchasedNotifier.Publish();
        
        // Get the updated player balance, then show it onto the UI
        var progressData = playerProgress.GetProgressData();

        OnUpdatePlayerBalanceUINotifier.Publish(new PlayerBalance{
            TotalCoins = progressData.CurrentCoins,
            TotalGems  = progressData.CurrentGems
        });

        yield return null;
    }
}
