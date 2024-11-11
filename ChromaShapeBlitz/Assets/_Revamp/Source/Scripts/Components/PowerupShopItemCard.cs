using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PowerupShopItemCard : ShopItemCardBase
{
    private readonly string ACTIVATION_INSTANT  = "<sprite=0>";
    private readonly string ACTIVATION_MANUAL   = "<sprite=1>";
    private readonly string EFFECT_PERMANENT    = "<sprite=2>";
    private readonly string ITEM_STACKABLE      = "<sprite=3>";
    private readonly string BADGE_SPACE         = "<space=.1rem>";

    [SerializeField] private TextMeshProUGUI badgeLabel;
    [SerializeField] private TextMeshProUGUI stackLabel;

    [Space(5)]
    [SerializeField] private Sprite indicatorPermanent;
    [SerializeField] private Sprite indicatorNormal;

    private Button m_button;
    private PowerupItemData m_itemData;
    private Image activeIndicatorImg;

    //private int m_Amount;

    void Awake()
    {
        TryGetComponent(out m_button);

        if (m_button != null)
            m_button.onClick.AddListener(HandleClicked);

        activeIndicator.TryGetComponent(out activeIndicatorImg);
    }

    /// <summary>
    /// Give the card an internal data, then render it
    /// </summary>
    protected override void ApplyCardViewData(BaseItemData itemData)
    {
        base.ApplyCardViewData(itemData);

        var powerupItemData = itemData as PowerupItemData;
        m_itemData = powerupItemData;

        badgeLabel.text = SelectBadges(powerupItemData);

        if (powerupItemData.IsOwned)
            SetOwned();
    }

    /// <summary>
    /// A more specific base item data of type "PowerupItemData".
    /// </summary>
    /// <returns>Powerup Item Data</returns>
    public PowerupItemData GetPowerupItemData() => m_itemData;

    /// <summary>
    /// Handle the logic when the item is owned such as after buying or
    /// when listed into the listview. This will automatically decide
    /// which to display, either the "Count" or "Infinite" indicators
    ///
    public override void SetOwned()
    {
        if (m_itemData == null)
            return;

        // Show the indicator icon
        activeIndicator.SetActive(true);

        // If it is a permanent effect, dont show the stack count
        // but show an infinite indicator
        if (m_itemData.ItemType == PowerupType.Permanent)
        {
            activeIndicatorImg.sprite = indicatorPermanent;
            stackLabel.text = string.Empty;

            // Given that permanent items arent stackable,
            // we replace its price text with "Active".
            priceLabel.text = "<color=#FBA136>Active</color>";
        }
        else
        {
            activeIndicatorImg.sprite = indicatorNormal;
            stackLabel.text = m_itemData.CurrentAmount.ToString();
            
            // If the item stack is currently full, we wont show the price label.
            // We'll show "FULL" instead.
            if (m_itemData.CurrentAmount >= m_itemData.MaxCount)
                priceLabel.text = "<color=#00F996>Full</color>";
        }
    }

    public string SelectBadges(PowerupItemData itemData)
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

    protected override void HandleClicked()
    {
        PowerupShopItemClickNotifier.NotifyObserver(this);
    }

    /// <summary>
    /// Invoke the click event manually. This is useful during tutorial
    /// </summary>
    public void InvokeClick() => PowerupShopItemClickNotifier.NotifyObserver(this);
}
