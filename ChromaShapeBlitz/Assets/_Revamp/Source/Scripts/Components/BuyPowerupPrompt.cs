using TMPro;
using UnityEngine;

public class BuyPowerupPrompt : BuyItemPromptBase
{
    [Space(10)]
    [Header("Non-Inherited Properties")]
    [SerializeField] private TextMeshProUGUI badgeLabel;
    [SerializeField] private TextMeshProUGUI descLabel;

    protected override void OnAwake()
    {
        BaseItemName = "powerup";

        OnBeforeShow = () =>
        {
            descLabel.text = ItemData.Description;
        };
    }
}
