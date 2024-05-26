using System.Collections;
using UnityEngine;

/// <summary>
/// This will subscribe to the event notifier to listen for when
/// a purchasable skin item was clicked
/// 
/// This script must be attached into the Skin Menu Controller gameobject
/// </summary>
public class OnBlockSkinShopItemClickedObserver : MonoBehaviour
{
    //[SerializeField] private PurchaseItemDialog purchaseSkinDialog;
    [SerializeField] private BuySkinDialog purchaseSkinDialog;

    private CustomBlockSkinsHelper skinHelper;

    private void Awake()
    {
        skinHelper = CustomBlockSkinsHelper.Instance;    
    }

    private void OnEnable() => OnBlockSkinShopItemClickedNotifier.Event.AddListener(Subscribe);

    private void OnDisable() => OnBlockSkinShopItemClickedNotifier.Event.RemoveListener(Subscribe);

    public void Subscribe(BlockSkinShopItem sender)
    {
        // If the selected skin item was already purchased,
        // or is a default skin, equip it.
        if (sender.GetSkinInfo().IsOwned)
        {
            Debug.Log("Item already owned");
            StartCoroutine(EquipSkin(sender));
        }
        // Otherwise, show the purchase confirmation dialog
        else
        {
            Debug.Log("Should purchase first");
            PromptPurchase(sender);
        }
    }

    private IEnumerator EquipSkin(BlockSkinShopItem sender)
    {
        if (skinHelper == null)
            yield break;

        var skinInfo = sender.GetSkinInfo();
        
        yield return StartCoroutine
        (
            skinHelper.UseSkin(skinInfo.ColorCategory, skinInfo.Id)
        );

        sender.ToggleEquipped();
        yield return null;
    }
    
    private void PromptPurchase(BlockSkinShopItem sender)
    {
        var progressData = PlayerProgressHelper.Instance.GetProgressData();
        var skinInfo     = sender.GetSkinInfo();
        var canBuyItem   = false;

        if (skinInfo.Cost == CurrencyType.Coin)
            canBuyItem = progressData.CurrentCoins >= skinInfo.Price;
        
        else if (skinInfo.Cost == CurrencyType.Gem)
            canBuyItem = progressData.CurrentGems >= skinInfo.Price;

        // These parameters will be used for displaying the
        // skin details into the dialog.
        purchaseSkinDialog.SetItemDetails(new BlockSkinShopItemInfo
        {
            Id      = skinInfo.Id,
            Cost    = skinInfo.Cost,
            Price   = skinInfo.Price,
            Name    = skinInfo.Name,
            ColorCategory = skinInfo.ColorCategory,
            PreviewImage  = skinInfo.PreviewImage,
        });

        // Tell if we can buy this item. If not, then
        // disable the Buy button, which is handled
        // internally by this function:
        purchaseSkinDialog.SetCanBuy(canBuyItem); //(skinInfo.Price >= 20);

        // We need this event sender so that we can have a
        // reference back to the selected skin item. The
        // purchase dialog has a "Buy" button that we should
        // listen to when it was clicked.
        purchaseSkinDialog.SetEventSenderData(sender);

        // Display the dialog
        purchaseSkinDialog.Show();
    }
}
