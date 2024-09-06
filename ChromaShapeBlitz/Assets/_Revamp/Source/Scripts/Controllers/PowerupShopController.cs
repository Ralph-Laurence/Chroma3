using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class PowerupShopController : MonoBehaviour
{
    [SerializeField] private PowerupsAssetGroup powerupAssetsContainer;
    [SerializeField] private GameObject         itemCard;

    [Header("Item Card Backgrounds")]
    [SerializeField] private Sprite             itemCardBgBlue;
    [SerializeField] private Sprite             itemCardBgGreen;
    [SerializeField] private Sprite             itemCardBgOrange;
    [SerializeField] private Sprite             itemCardBgPink;
    [SerializeField] private Sprite             itemCardBgPurple;

    [SerializeField] private RectTransform      scrollRectContent;
    [SerializeField] private BuyPowerupPrompt   buyConfirmPrompt;

    private Dictionary<int, PowerupsAsset>  powerupsLookUp = new();
    private GameSessionManager              gsm;
    private UserDataHelper                  userDataHelper;

    void Awake()
    {
        gsm            = GameSessionManager.Instance;
        userDataHelper = UserDataHelper.Instance;
        
        powerupAssetsContainer.AddToLookup(powerupsLookUp);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(BuildShopMenu());
    }

    void OnEnable()
    {
        PowerupShopItemClickNotifier.BindEvent(ObservePowerupItemClicked);
        PowerupPurchaseNotifier.BindEvent(ObservePowerupPurchase);
    }

    void OnDisable()
    {
        PowerupShopItemClickNotifier.UnbindEvent(ObservePowerupItemClicked);
        PowerupPurchaseNotifier.UnbindEvent(ObservePowerupPurchase);
    }

    private void ObservePowerupItemClicked(PowerupShopItemCard sender)
    {
        if (sender == null)
        {
            Debug.Log("No sender");
            return;
        }
        var itemData = sender.GetItemData();

        if (itemData == null)
        {
            Debug.Log("No item data!");
            return;
        }

        buyConfirmPrompt.gameObject.SetActive(true);
        buyConfirmPrompt.SetEventSender(sender);
        buyConfirmPrompt.Show();
    }

    private void ObservePowerupPurchase(PowerupShopItemCard sender)
    {
        CommonEventNotifier.NotifyObserver(CommonEventTags.INDEFINITE_LOADER_SHOW);

        var data            = sender.GetItemData();
        var userData        = gsm.UserSessionData;
        int playerBalance   = default;

        // Decrease player's bank
        //switch (data.Cost)
        //{
        //    case CurrencyType.Coin:
        //        userData.TotalCoins -= data.Price;
        //        playerBalance = userData.TotalCoins;
        //        break;

        //    case CurrencyType.Gem:
        //        userData.TotalGems -= data.Price;
        //        playerBalance = userData.TotalGems;
        //        break;
        //}

        // Remember the item as owned
        //if (!userData.OwnedBackgroundIds.Contains(data.ID))
        //    userData.OwnedBackgroundIds.Add(data.ID);

        // Equip the background
        //StartCoroutine(UseBackground(data, userData, onComplete: () => {

        //    PlayerCurrencyNotifier.NotifyObserver(new PlayerCurrencyEventArgs
        //    {
        //        Currency = data.CostCurrency,
        //        Amount = playerBalance,
        //        Animate = true,
        //        AnimationType = PlayerCurrencyParticleAnimator.ANIMATION_MODE_DECREASE_AMOUNT
        //    });

        //    buyConfirmPrompt.Hide();
        //    sender.SetOwned(true);
        //    m_activeItemCard = sender;
        //    CommonEventNotifier.NotifyObserver(CommonEventTags.INDEFINITE_LOADER_HIDE);
        //}));
    }

    public IEnumerator BuildShopMenu()
    {
        var ownedPowerups = gsm.UserSessionData.Inventory.OwnedPowerups;

        foreach (var kvp in powerupsLookUp)
        {
            Instantiate(itemCard, scrollRectContent).TryGetComponent(out PowerupShopItemCard card);
            
            var itemData = kvp.Value;
            var powerupData = new PowerupItemData
            {
                Id              = itemData.Id,
                Name            = itemData.Name,
                Cost            = itemData.Cost,
                Description     = itemData.Description,
                Price           = itemData.Price,
                PreviewImage    = itemData.PreviewImage,
                ActivationMode  = itemData.ActivationMode,
                ItemType        = itemData.ItemType,
                MaxCount        = itemData.MaxCount
            };

            for (var i = 0; i < ownedPowerups.Count; i++)
            {
                var p = ownedPowerups[i];

                if (p.PowerupID == powerupData.Id)
                {
                    powerupData.CurrentAmount = p.CurrentAmount;
                    powerupData.IsOwned = true;
                    break;
                }
            }

            card.SetItemData(powerupData);

            var cardBg = SelectCardBackground(itemData.CardColor);

            if (cardBg != null)
                card.SetItemPreviewBackground(cardBg);

            yield return null;
        }
    }

    private Sprite SelectCardBackground(PowerupItemCardColor color) => color switch
    {
        PowerupItemCardColor.Blue => itemCardBgBlue,
        PowerupItemCardColor.Green => itemCardBgGreen,
        PowerupItemCardColor.Pink => itemCardBgPink,
        PowerupItemCardColor.Orange => itemCardBgOrange,
        PowerupItemCardColor.Purple => itemCardBgPurple,
        _ => null
    };
}
