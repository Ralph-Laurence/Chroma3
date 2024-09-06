using TMPro;
using UnityEngine;

public class BuyPowerupPrompt : BuyItemPromptBase
{
    [Space(10)]
    [Header("Non-Inherited Properties")]
    [SerializeField] private TextMeshProUGUI badgeLabel;
    [SerializeField] private TextMeshProUGUI descLabel;

    private readonly string MSG_ALREADY_OWNED   = "You already own this powerup!";
    private readonly string MSG_ALREADY_MAXED   = "You're full on this item! You can't carry any more than <color=#EC4531>$max$</color>.";
    private readonly string STR_BTN_TEXT_OWNED  = "Owned";
    private readonly string STR_BTN_TEXT_FULL   = "Full";

    protected override void OnAwake()
    {
        BaseItemName = "powerup";

        OnBeforeShow = () =>
        {
            var evSender    = EventSender as PowerupShopItemCard;
            var itemData    = ItemData as PowerupItemData;

            UpdateLabels(itemData, evSender);
            UpdateBuyButtonState(itemData);
        };
    }

    private void UpdateLabels(PowerupItemData itemData, PowerupShopItemCard evSender)
    {
        descLabel.text  = itemData.Description;
        badgeLabel.text = evSender.SelectBadges(itemData);
    }

    private void UpdateBuyButtonState(PowerupItemData itemData)
    {
        // We just Check if the selected item was already owned.
        // In this phase, we arent sure yet if it is a permanent item, stackable or non-stackable.
        // Its up to the HandleOwnedItem() to process.
        if (itemData.IsOwned)
        {
            HandleOwnedItem(itemData);
            return;
        }

        // Assume that the item isnt owned yet (new), or might be already owned
        // but the current stack qty isn't maxed out yet. In that case, we
        // should enable the "Buy" button.
        ResetBuyButton();
    }

    private void HandleOwnedItem(PowerupItemData itemData)
    {
        // Handle the case for player-owned Stackable powerups
        if (itemData.ItemType == PowerupType.ConsumableStackable)
        {
            HandleStackableItem(itemData);
            return;
        }

        // Handle the case for player-owned Permanent powerups
        // or Single-item (Non-Stackable) powerups
        SetBuyButtonText(STR_BTN_TEXT_OWNED, false);
        promptMessage.text = MSG_ALREADY_OWNED;
    }

    private void HandleStackableItem(PowerupItemData itemData)
    {
        // The selected item is of full stacks; We can't buy any more than the max allowed
        if (itemData.CurrentAmount >= itemData.MaxCount)
        {
            SetBuyButtonText(STR_BTN_TEXT_FULL, false);
            promptMessage.text = MSG_ALREADY_MAXED.Replace("$max$", itemData.MaxCount.ToString());
            return;
        }

        // The item is available to buy as our stack isnt full
        ResetBuyButton();
    }

    /// <summary>
    /// Notify the controller that subscribes to the event and tell
    /// it that we are going to buy the selected item.
    /// </summary>
    protected override void OnBuyButtonClick()
    {
        var evSender = EventSender as PowerupShopItemCard;

        PowerupPurchaseNotifier.NotifyObserver(evSender);
    }
}
