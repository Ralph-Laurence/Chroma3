using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundShopController : MonoBehaviour
{
    [SerializeField] private List<BackgroundsAsset> backgroundAssets;
    [SerializeField] private GameObject itemCard;
    [SerializeField] private RectTransform scrollRectContent;
    [SerializeField] private BuyBackgroundPrompt buyConfirmPrompt;
    // [SerializeField] private ScrollRect scrollRect;

    private Dictionary<int, BackgroundsAsset> backgroundsLookUp;
    private ToggleGroup toggleGroup;

    private GameSessionManager gsm;
    private UserDataHelper userDataHelper;

    private BackgroundShopItemCard m_activeItemCard;

    void Awake()
    {
        gsm = GameSessionManager.Instance;
        userDataHelper = UserDataHelper.Instance;

        scrollRectContent.TryGetComponent(out toggleGroup);

        backgroundsLookUp = new();
        backgroundAssets.ForEach(asset => backgroundsLookUp.Add(asset.Id, asset));
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BuildShopMenu();
    }

    void OnEnable()
    {
        BackgroundShopItemClickNotifier.BindEvent(ObserveBackgroundShopItemClicked);
        BackgroundPurchaseNotifier.BindEvent(ObserveBackgroundPurchase);

        ResetMenu();
    }

    void OnDisable()
    {
        BackgroundShopItemClickNotifier.UnbindEvent(ObserveBackgroundShopItemClicked);    
        BackgroundPurchaseNotifier.UnbindEvent(ObserveBackgroundPurchase);
    }

    private void BuildShopMenu()
    {
        var ownedBackgrounds = gsm.UserSessionData.OwnedBackgroundIds;
        var activeBackground = gsm.UserSessionData.ActiveBackgroundID;
        
        Transform activeBackgroundTransform = null;
        BackgroundShopItemCard activeCard = null;

        foreach (var kvp in backgroundsLookUp)
        {
            Instantiate(itemCard, scrollRectContent).TryGetComponent(out BackgroundShopItemCard card);

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
        }

        if (activeBackgroundTransform != null)
            activeBackgroundTransform.SetAsFirstSibling();

        if (activeCard)
            activeCard.Toggle(true);
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
        CommonEventNotifier.NotifyObserver(CommonEventTags.INDEFINITE_LOADER_SHOW);

        var data = sender.GetItemData();
        var userData = gsm.UserSessionData;
        int playerBalance = default;

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
            CommonEventNotifier.NotifyObserver(CommonEventTags.INDEFINITE_LOADER_HIDE);
        }));
    }

    private void ObserveBackgroundShopItemClicked(BackgroundShopItemCard sender)
    {
        var skinData = sender.GetItemData();

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
