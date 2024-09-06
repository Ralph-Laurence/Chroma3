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

    void Awake()
    {
        TryGetComponent(out m_button);

        if (m_button != null)
            m_button.onClick.AddListener(HandleClicked);

        activeIndicator.TryGetComponent(out activeIndicatorImg);
    }

    protected override void ApplyCardViewData(BaseItemData itemData)
    {
        base.ApplyCardViewData(itemData);

        var powerupItemData = itemData as PowerupItemData;
        m_itemData = powerupItemData;
        badgeLabel.text = SelectBadges(powerupItemData);

        if (powerupItemData.IsOwned)
            SetOwned();
    }

    public override void SetOwned()
    {
        if (m_itemData == null)
            return;

        // Show the indicator icon
        activeIndicator.SetActive(true);

        // If it is a permanent effect, dont show the stack count
        // but show a white check
        if (m_itemData.ItemType == PowerupType.Permanent)
        {
            activeIndicatorImg.sprite = indicatorPermanent;
            stackLabel.text = string.Empty;
        }
        else
        {
            activeIndicatorImg.sprite = indicatorNormal;
            stackLabel.text = m_itemData.CurrentAmount.ToString();
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
}
