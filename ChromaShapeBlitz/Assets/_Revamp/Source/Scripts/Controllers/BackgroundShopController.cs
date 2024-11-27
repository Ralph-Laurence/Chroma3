using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundShopController : MonoBehaviour, IShopController
{
    [SerializeField] private List<BackgroundsAsset> backgroundAssets;
    [SerializeField] private GameObject itemCard;
    [SerializeField] private RectTransform scrollRectContent;
    [SerializeField] private BuyBackgroundPrompt buyConfirmPrompt;
    [SerializeField] private BuyBackgroundResultDialog buyResultDialog;

    private Dictionary<int, BackgroundsAsset> backgroundsLookUp;
    private ToggleGroup toggleGroup;

    private GameSessionManager gsm;
    private UserDataHelper userDataHelper;

    private BackgroundShopItemCard m_activeItemCard;
    public List<BackgroundShopItemCard> BackgroundItemCards {get; private set;}
    //public void ClearBackgroundItemCards() => BackgroundItemCards?.Clear();

    void Awake()
    {
        gsm = GameSessionManager.Instance;
        userDataHelper = UserDataHelper.Instance;
        
        scrollRectContent.TryGetComponent(out toggleGroup);
        
        BackgroundItemCards = new();
        backgroundsLookUp   = new();
        
        backgroundAssets.ForEach(asset => backgroundsLookUp.Add(asset.Id, asset));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BuildShopMenu();
    }

    void OnEnable()
    {
        BackgroundShopItemClickNotifier.BindObserver(ObserveBackgroundShopItemClicked);
        BackgroundPurchaseNotifier.BindObserver(ObserveBackgroundPurchase);

        ResetMenu();
    }

    void OnDisable()
    {
        BackgroundShopItemClickNotifier.UnbindObserver(ObserveBackgroundShopItemClicked);    
        BackgroundPurchaseNotifier.UnbindObserver(ObserveBackgroundPurchase);
    }

    private void BuildShopMenu()
    {
        var ownedBackgrounds = gsm.UserSessionData.OwnedBackgroundIds;
        var activeBackground = gsm.UserSessionData.ActiveBackgroundID;
        
        Transform activeBackgroundTransform = null;
        BackgroundShopItemCard activeCard   = null;

        foreach (var kvp in backgroundsLookUp)
        {
            var cardObj = Instantiate(itemCard, scrollRectContent);
            
            cardObj.TryGetComponent(out BackgroundShopItemCard card);

            var itemData = kvp.Value;
            var data = new BackgroundItemData
            {
                ID              = itemData.Id,
                Name            = itemData.Name,
                PreviewImage    = itemData.PreviewImage,
                Price           = itemData.Price,
                CostCurrency    = itemData.Cost
            };

            card.SetItemData(data);
            card.SetToggleGroup(toggleGroup);
            
            if (ownedBackgrounds.Contains(itemData.Id))
                card.SetOwned(true);
            
            if (itemData.Id == activeBackground)
            {
                // card.Toggle(true);
                activeCard = card;
                activeBackgroundTransform = card.transform;
            }

            // This is used mainly for tutorial.
            // Otherwise it has no other uses in this context
            BackgroundItemCards.Add(card);
        }

        if (activeBackgroundTransform != null)
            activeBackgroundTransform.SetAsFirstSibling();

        if (activeCard)
            activeCard.Toggle(true);
    }

    public void OnBecameFullyVisible()
    {
        ShopMenuPageCommonNotifier.NotifyObserver(ShopMenuPageEventNames.BackgroundsPageFullyShown);
    }

    /// <summary>
    /// This must be called everytime this gameobject gets active.
    /// The menu must be reset so that the active item card goes to the top
    /// of the scrollview as the first sibling, and also marking it active.
    /// </summary>
    private void ResetMenu()
    {
        if (m_activeItemCard == null)
            return;

        if (m_activeItemCard.transform.GetSiblingIndex() != 0)
            m_activeItemCard.transform.SetAsFirstSibling();

        if (!m_activeItemCard.IsMarkedActive)
            m_activeItemCard.Toggle(true);
    }

    #region EVENT_OBSERVERS
    private void ObserveBackgroundPurchase(BackgroundShopItemCard sender)
    {
        var data          = sender.GetItemData();
        var userData      = gsm.UserSessionData;
        int playerBalance = default;
        var canBuy        = true;
        var notEnoughWhat = "Coins";

        switch (data.CostCurrency)
        {
            case CurrencyType.Coin:

                if (data.CostCurrency == CurrencyType.Coin && data.Price > gsm.UserSessionData.TotalCoins)
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
            
            buyConfirmPrompt.Hide();
            buyResultDialog.ShowFailResult(msg, data.PreviewImage);
            return;
        }

        ProgressLoaderNotifier.NotifyFourSegment(true);

        // Decrease player's bank
        switch (data.CostCurrency)
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

        // Remember the item as owned
        if (!userData.OwnedBackgroundIds.Contains(data.ID))
            userData.OwnedBackgroundIds.Add(data.ID);

        // Equip the background
        StartCoroutine(UseBackground(data, userData, onComplete: () => {

            PlayerCurrencyNotifier.NotifyObserver(new PlayerCurrencyEventArgs
            {
                Currency = data.CostCurrency,
                Amount = playerBalance,
                Animate = true,
                AnimationType = PlayerCurrencyParticleAnimator.ANIMATION_MODE_DECREASE_AMOUNT
            });

            buyConfirmPrompt.Hide();
            sender.SetOwned(true);
            m_activeItemCard = sender;
            
            ProgressLoaderNotifier.NotifyFourSegment(false);
            buyResultDialog.ShowSuccessResult(data.PreviewImage, $"{data.Name} has been automatically set as the background.");
        }));
    }

    private void ObserveBackgroundShopItemClicked(BackgroundShopItemCard sender)
    {
        var skinData = sender.GetItemData();

        if (skinData == null)
        {
            Debug.Log("No skin data!");
        }

        if (gsm.UserSessionData.OwnedBackgroundIds == null)
        {
            Debug.Log("NO saved data");
        }

        // If the background isnt purchased yet, prompt the user to buy it
        if (!gsm.UserSessionData.OwnedBackgroundIds.Contains(skinData.ID))
        {
            buyConfirmPrompt.gameObject.SetActive(true);
            buyConfirmPrompt.SetEventSender(sender);
            buyConfirmPrompt.Show();
            return;
        }

        // Otherwise, just equip it
        CommonEventNotifier.NotifyObserver(CommonEventTags.INDEFINITE_LOADER_SHOW);

        StartCoroutine(UseBackground(skinData, gsm.UserSessionData, () => {
            sender.Toggle(true);
            m_activeItemCard = sender;
            CommonEventNotifier.NotifyObserver(CommonEventTags.INDEFINITE_LOADER_HIDE);
        }));
    }
    #endregion

    private IEnumerator UseBackground(BackgroundItemData data, UserData userData, Action onComplete = null)
    {
        // Change the active background into this id
        userData.ActiveBackgroundID = data.ID;

        gsm.UserSessionData = userData;

        // Write the changes to file
        yield return StartCoroutine(userDataHelper.SaveUserData(userData, (d) => onComplete?.Invoke() ));
    }
}
