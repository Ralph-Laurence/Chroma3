using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PowerupShopItemCard : ShopItemCardBase //MonoBehaviour
{
    private readonly string ACTIVATION_INSTANT  = "<sprite=0>";
    private readonly string ACTIVATION_MANUAL   = "<sprite=1>";
    private readonly string EFFECT_PERMANENT    = "<sprite=2>";
    private readonly string ITEM_STACKABLE      = "<sprite=3>";
    private readonly string BADGE_SPACE         = "<space=.1rem>";

    // [SerializeField] private TextMeshProUGUI itemNameLabel;
    // [SerializeField] private TextMeshProUGUI priceLabel;
    [SerializeField] private TextMeshProUGUI badgeLabel;

    //[SerializeField] private Image           previewBackground;
    //[SerializeField] private Image           previewImage;
    // [SerializeField] private GameObject      activeIndicator;

    private Button m_button;
    private PowerupItemData m_itemData;

    void Awake()
    {
        TryGetComponent(out m_button);

        if (m_button != null)
            m_button.onClick.AddListener(HandleClicked);
    }

    //public void SetItemData(PowerupItemData itemData)
    //{
    //    m_itemData = itemData;
    //    ApplyCardViewData(itemData);
    //}

    protected override void ApplyCardViewData(BaseItemData itemData)
    {
        base.ApplyCardViewData(itemData);

        var powerupItemData = itemData as PowerupItemData;

        badgeLabel.text = SelectBadges(powerupItemData);
    }

    //private void ApplyCardViewData(PowerupItemData itemData)
    //{
    //    itemNameLabel.text  = itemData.Name;
    //    previewImage.sprite = itemData.PreviewImage;

    //    var cost = itemData.Cost == CurrencyType.Coin
    //             ? $"<style=\"Coin\">{itemData.Price}"
    //             : $"<style=\"Gem\">{itemData.Price}";

    //    priceLabel.text = cost;
    //    badgeLabel.text = SelectBadges(itemData);
    //}

    private string SelectBadges(PowerupItemData itemData)
    {
        var mode = itemData.ActivationMode switch
        {
            PowerupActivation.Instant   => ACTIVATION_INSTANT,
            PowerupActivation.Manual    => ACTIVATION_MANUAL,
            _ => string.Empty
        };

        var output = new List<string>{ {mode} };

        if (itemData.ItemType == PowerupType.Permanent)
            output.Add(EFFECT_PERMANENT);

        if (itemData.ItemType == PowerupType.ConsumableStackable)
            output.Add(ITEM_STACKABLE);
        
        return string.Join(BADGE_SPACE, output);
    }

    private void HandleClicked()
    {
        PowerupShopItemClickNotifier.NotifyObserver(this);
    }
}
