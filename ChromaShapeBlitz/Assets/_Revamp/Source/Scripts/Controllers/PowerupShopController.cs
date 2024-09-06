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

    [Header("Powerup Shop UI")]
    [SerializeField] private RectTransform      scrollRectContent;
    [SerializeField] private BuyPowerupPrompt   buyConfirmPrompt;

    [SerializeField] private BuyPowerupResultDialog buyResultDialog;

    private Dictionary<int, PowerupsAsset>  powerupsLookUp = new();
    private GameSessionManager  gsm;
    private UserDataHelper      userDataHelper;

    void Awake()
    {
        gsm = GameSessionManager.Instance;

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

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.F1))
    //         

    //     if (Input.GetKeyDown(KeyCode.F2))
    //         buyResultDialog.ShowFailResult("Fail Test", powerupsLookUp[109].PreviewImage);
    // }

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
        StartCoroutine(IEHandlePurchase(sender));
        //    buyConfirmPrompt.Hide();
        //    sender.SetOwned(true);
        //    m_activeItemCard = sender;
        //    CommonEventNotifier.NotifyObserver(CommonEventTags.INDEFINITE_LOADER_HIDE);
        //}));
    }

    private IEnumerator IEHandlePurchase(PowerupShopItemCard sender)
    {
        ProgressLoaderNotifier.NotifyFourSegment(true);

        var data            = sender.GetItemData();
        var userData        = gsm.UserSessionData;
        int playerBalance   = default;
        var ownedPowerups   = userData.Inventory.OwnedPowerups;

        // Decrease player's bank
        switch (data.Cost)
        {
           case CurrencyType.Coin:
               userData.TotalCoins -= data.Price;
               playerBalance = userData.TotalCoins;
               break;

           case CurrencyType.Gem:
               userData.TotalGems -= data.Price;
               playerBalance = userData.TotalGems;
               break;
        }

        // TASK: Remember the item as owned.
        //
        // It would be better to use .Any() as it is more efficient in our requirement
        // to just check for the existence of an item in the list without retrieving it.
        // userData.Inventory.OwnedPowerups.FirstOrDefault(powerup => powerup.PowerupID == data.Id);
        var isOwned = ownedPowerups.Any(powerup => powerup.PowerupID == data.Id);

        // If we already own a powerup, just update its current amount.
        if (isOwned)
        {
            for (var i = 0; i < ownedPowerups.Count; i++)
            {
                if (ownedPowerups[i].PowerupID == data.Id)
                {
                    var ownedPowerup = ownedPowerups[i];
                    var powerupAsset = powerupsLookUp[data.Id];

                    ownedPowerup.CurrentAmount += 1; //powerupAsset.Amount; // or set it to a specific value
                    ownedPowerups[i] = ownedPowerup;
                    
                    // Exit the loop once the powerup is found and updated.
                    break;
                }

                yield return null;
            }
        }

        // Otherwise add it
        else
        {
            ownedPowerups.Add(new PowerupInventory
            {
                PowerupID = data.Id,
                CurrentAmount = 1
            });
        }
        
        yield return new WaitForSeconds(1.5F);
        buyConfirmPrompt.Close();
        
        ProgressLoaderNotifier.NotifyFourSegment(false);
        yield return new WaitForSeconds(0.25F);

        PlayerCurrencyNotifier.NotifyObserver(new PlayerCurrencyEventArgs
        {
            Currency = data.Cost,
            Amount = playerBalance,
            Animate = true,
            AnimationType = PlayerCurrencyParticleAnimator.ANIMATION_MODE_DECREASE_AMOUNT
        });

        gsm.UserSessionData = userData;
        gsm.SetInventoryPageNeedsReload(true);

        buyResultDialog.ShowSuccessResult(data.PreviewImage);
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
