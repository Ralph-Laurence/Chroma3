using System.Collections;
using UnityEngine;

/// <summary>
/// This will subscribe to the event notifier to listen for when
/// a purchasable skin item was clicked
/// 
/// This script must be attached into the Skin Menu Controller gameobject
/// </summary>
public class OnPurchasableSkinClickedObserver : MonoBehaviour
{
    [SerializeField] private PurchaseSkinDialog purchaseSkinDialog;

    private CustomizationsHelper themeHelper;

    private void Awake()
    {
        themeHelper = CustomizationsHelper.Instance;    
    }

    private void OnEnable() => OnPurchasableSkinClickedNotifier.Event.AddListener(Subscribe);

    private void OnDisable() => OnPurchasableSkinClickedNotifier.Event.RemoveListener(Subscribe);

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
        if (themeHelper == null)
            yield break;

        var skinInfo = sender.GetSkinInfo();
        yield return StartCoroutine
        (
            themeHelper.EquipSkin(skinInfo.Id, skinInfo.ColorCategory)
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
        purchaseSkinDialog.SetSkinDetails(new SkinInfo
        {
            Id      = skinInfo.Id,
            Cost    = skinInfo.Cost,
            Price   = skinInfo.Price,
            Name    = skinInfo.Name,

            PreviewImage = skinInfo.PreviewImage,
        });

        // Tell if we can buy this item. If not, then
        // disable the Buy button, which is handled
        // internally by this function:
        purchaseSkinDialog.SetCanBuyItem(canBuyItem);

        // We need this event sender so that we can have a
        // reference back to the selected skin item. The
        // purchase dialog has a "Buy" button that we should
        // listen to when it was clicked.
        purchaseSkinDialog.SetEventSender(sender);

        // Display the dialog
        purchaseSkinDialog.Show();
    }
}
