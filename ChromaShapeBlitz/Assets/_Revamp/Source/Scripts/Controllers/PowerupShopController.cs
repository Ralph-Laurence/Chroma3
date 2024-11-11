using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerupShopController : MonoBehaviour, IShopController
{
    [SerializeField] private PowerupsIO         powerupsIO;
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

    private readonly Dictionary<int, PowerupsAsset>  powerupsLookUp = new();
    private GameSessionManager  gsm;
    private UserDataHelper      userDataHelper;

    public List<PowerupShopItemCard> PowerupItemCards { get; private set; }

    void Awake()
    {
        gsm = GameSessionManager.Instance;

        userDataHelper   = UserDataHelper.Instance;
        PowerupItemCards = new();

        powerupAssetsContainer.AddToLookup(powerupsLookUp);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(BuildShopMenu());
    }

    void OnEnable()
    {
        PowerupShopItemClickNotifier.BindObserver(ObservePowerupItemClicked);
        PowerupPurchaseNotifier.BindEvent(ObservePowerupPurchase);
    }

    void OnDisable()
    {
        PowerupShopItemClickNotifier.UnbindObserver(ObservePowerupItemClicked);
        PowerupPurchaseNotifier.UnbindEvent(ObservePowerupPurchase);
    }
    
    private void ObservePowerupItemClicked(PowerupShopItemCard sender)
    {
        if (sender == null)
            return;
        
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
        var data   = sender.GetItemData();
        var canBuy = true;

        var notEnoughWhat = "Coins";

        switch (data.Cost)
        {
            case CurrencyType.Coin:

                if (data.Cost == CurrencyType.Coin && data.Price > gsm.UserSessionData.TotalCoins)
                    canBuy = false;

                break;

            case CurrencyType.Gem:

                if (data.Price > gsm.UserSessionData.TotalGems)
                {
                    canBuy = false;
                    notEnoughWhat = "Gems";
                }
                break;
        }

        if (!canBuy)
        {
            var msg = $"You don't have enough <color=#EC4531>{notEnoughWhat}</color> to buy this item.";
            
            buyConfirmPrompt.Close();
            buyResultDialog.ShowFailResult(msg, data.PreviewImage);
            return;
        }

        StartCoroutine(IEHandlePurchase(sender));
    }

    private IEnumerator IEHandlePurchase(PowerupShopItemCard sender)
    {
        ProgressLoaderNotifier.NotifyFourSegment(true);

        var powerupItemData     = sender.GetPowerupItemData(); //GetItemData();
        var userData            = gsm.UserSessionData;
        int playerBalance       = default;
        var ownedPowerups       = userData.Inventory.OwnedPowerups ??= new();

        // TASK: Remember the item as owned.
        //
        // It would be better to use .Any() as it is more efficient in our requirement
        // to just check for the existence of an item in the list without retrieving it.
        // userData.Inventory.OwnedPowerups.FirstOrDefault(powerup => powerup.PowerupID == data.Id);
        var isOwned = ownedPowerups.Any(powerup => powerup.PowerupID == powerupItemData.Id);

        // If we already own a powerup, just update its current amount.
        // In this case, we will update the amount stored into the disk
        if (isOwned)
        {
            for (var i = 0; i < ownedPowerups.Count; i++)
            {
                if (ownedPowerups[i].PowerupID == powerupItemData.Id)
                {
                    var ownedPowerup = ownedPowerups[i];
                    var powerupAsset = powerupsLookUp[powerupItemData.Id];

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
                PowerupID = powerupItemData.Id,
                CurrentAmount = 1
            });
        }
        
        // Permanent powerups dont need item card count indicator as
        // their effects are mostly perk/skill/ability based.
        if (powerupItemData.ItemType == PowerupType.Permanent)
            HandlePermanentPowerup(userData, powerupItemData);
        
        // Increase the card's displayed amount by 1
        else
            powerupItemData.CurrentAmount++;

        powerupItemData.IsOwned = true;
        
        // Apply the updated item card data
        sender.SetItemData(powerupItemData);

        // Decrease player's bank
        switch (powerupItemData.Cost)
        {
           case CurrencyType.Coin:
               userData.TotalCoins -= powerupItemData.Price;
               playerBalance = userData.TotalCoins;
               break;

           case CurrencyType.Gem:
               userData.TotalGems -= powerupItemData.Price;
               playerBalance = userData.TotalGems;
               break;
        }

        yield return new WaitForSeconds(0.75F);
        buyConfirmPrompt.Close();
        
        ProgressLoaderNotifier.NotifyFourSegment(false);
        yield return new WaitForSeconds(0.25F);

        PlayerCurrencyNotifier.NotifyObserver(new PlayerCurrencyEventArgs
        {
            Currency        = powerupItemData.Cost,
            Amount          = playerBalance,
            Animate         = true,
            AnimationType   = PlayerCurrencyParticleAnimator.ANIMATION_MODE_DECREASE_AMOUNT
        });

        // Save the changes into disk
        yield return StartCoroutine(IESaveOwnedPowerups(gsm.UserSessionData, onComplete: () =>
        {
            gsm.UserSessionData = userData;

            gsm.SetInventoryPageNeedsReload(true);
            buyResultDialog.ShowSuccessResult(powerupItemData.PreviewImage, "Head over to your inventory to equip this item");
        }));
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

            if (ownedPowerups != null && ownedPowerups.Count > 0)
            {
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
            }

            card.SetItemData(powerupData);

            var cardBg = SelectCardBackground(itemData.CardColor);

            if (cardBg != null)
                card.SetItemPreviewBackground(cardBg);

            PowerupItemCards.Add(card);

            yield return null;
        }

        // ShopMenuPageCommonNotifier.NotifyObserver(ShopMenuPageEventNames.PowerupsPageFullyShown);
    }

    public void OnBecameFullyVisible()
    {
        ShopMenuPageCommonNotifier.NotifyObserver(ShopMenuPageEventNames.PowerupsPageFullyShown);
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

    /// <summary>
    /// Save the updated user data onto disk
    /// </summary>
    private IEnumerator IESaveOwnedPowerups(UserData userData, Action onComplete = null)
    {
        // Write the changes to file
        yield return StartCoroutine(UserDataHelper.Instance.SaveUserData(userData, (d) =>
        {
            onComplete?.Invoke();   
        }));
    }

    private void HandlePermanentPowerup(UserData userData, PowerupItemData powerupItemData)
    {
        var powerup = powerupsLookUp[powerupItemData.Id];

        switch (powerup.PowerupCategory)
        {
            case PowerupCategories.FillRatePerk:
                
                //var rate = Constants.BlockFillRates[powerup.EffectValue];
                // userData.SequenceFillRate = rate;
                userData.SequenceFillRate = powerup.EffectValue;
                break;
        }
    }
}