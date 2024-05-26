using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BlockSkinShopItem : ShopItemBase
{
    private BlockSkinShopItemInfo skinInfo;
    private Image bgImage;

    protected override void OnAwake()
    {
        base.OnAwake();

        TryGetComponent(out bgImage);
    }

    public override void HandleItemClicked()
    {
        base.HandleItemClicked();

        OnBlockSkinShopItemClickedNotifier.Publish(this);
    }
    public override void HandlePurchase() => StartCoroutine(PurchaseSkin());

    public void SetItemInfo(BlockSkinShopItemInfo skinInfo) => this.skinInfo = skinInfo;
    public BlockSkinShopItemInfo GetSkinInfo() => skinInfo;

    private IEnumerator PurchaseSkin()
    {
        var playerProgress = PlayerProgressHelper.Instance;

        // Decrease the player's current balance, then save them to disk
        yield return StartCoroutine
        (
            playerProgress.DecreasePlayerBank(skinInfo.Cost, skinInfo.Price, true)
        );

        yield return StartCoroutine
        (
            CustomBlockSkinsHelper.Instance.UseSkin( skinInfo.ColorCategory, skinInfo.Id )
        );
        
        skinInfo.IsOwned = true;
        
        ToggleEquipped();
        HidePriceLabel();

        // Notify the purchase confirmation dialog to close
        OnSkinPurchasedNotifier.Publish(skinInfo);
        
        // Get the updated player balance, then show it onto the UI
        var progressData = playerProgress.GetProgressData();

        OnUpdatePlayerBalanceUINotifier.Publish(new PlayerBalance
        {
            TotalCoins = progressData.CurrentCoins,
            TotalGems  = progressData.CurrentGems
        });

        yield return null;
    }

    public void SetFrameBackground(Sprite sprite) => bgImage.sprite = sprite;
}
