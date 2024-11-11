using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SkinShopController : MonoBehaviour
{
    private const string RuntimeMaterialsPath = "Materials/BlockSkins";

    [SerializeField] private List<BlockSkinsAssetGroup> blockSkinGroups;

    [Space(10)]
    [Header("Block Skin Item Cards")]
    [SerializeField] private GameObject skinItemCard;
    [Space(5)]
    [SerializeField] private Sprite skinItemCardBgBlue;
    [SerializeField] private Sprite skinItemCardBgGreen;
    [SerializeField] private Sprite skinItemCardBgOrange;
    [SerializeField] private Sprite skinItemCardBgMagenta;
    [SerializeField] private Sprite skinItemCardBgPurple;
    [SerializeField] private Sprite skinItemCardBgYellow;


    [Space(5)]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private BuySkinPrompt buySkinPrompt;
    [SerializeField] private RectTransform skinsScrollViewContent;
    [SerializeField] private Image skinsScrollViewport;
    [SerializeField] private ColorSelect skinColorFilter;

    [FormerlySerializedAs("navbarObj")]
    [SerializeField] private GameObject navbarObj;

    [SerializeField] private BuySkinsResultDialog buySkinsResultDialog;

    private Dictionary<int, BlockSkinsAsset> skinsLookupTable;
    private List<SkinShopItemCard> skinItemCards;
    private SkinShopItemCard m_activeItemCard;

    private readonly Color viewportTransparent = new(1.0F, 1.0F, 1.0F, 0.0F);
    private readonly Color viewportOpaque = new(1.0F, 1.0F, 1.0F, 1.0F);

    private ToggleGroup skinsItemsToggleGroup;
    private GameSessionManager gsm;
    private UserDataHelper userDataHelper;
    private bool isInitialized;

    void Awake()
    {
        skinsLookupTable = new();
        skinItemCards = new();

        gsm = GameSessionManager.Instance;
        userDataHelper = UserDataHelper.Instance;

        skinsScrollViewContent.TryGetComponent(out skinsItemsToggleGroup);

        blockSkinGroups.ForEach(group => group.AddTo(skinsLookupTable));
    }

    private void HandleShopMenuRootShown() => StartCoroutine(Initialize());

    private IEnumerator Initialize()
    {
        if (isInitialized)
            yield break;
        
        navbarObj.SetActive(false);

        yield return new WaitForSeconds(0.1F);

        yield return StartCoroutine(BuildSkinShopMenu());
        yield return StartCoroutine(FilterItems(skinColorFilter.Value, () => navbarObj.SetActive(true)));

        // skinColorFilter.OnColorSelected += (color) => StartCoroutine(FilterItems(color));
        isInitialized = true;
    }

    private IEnumerator BuildSkinShopMenu()
    {
        var cardBackgroundMapping = new Dictionary<ColorSwatches, Sprite>
        {
            { ColorSwatches.Blue,       skinItemCardBgBlue    },
            { ColorSwatches.Green,      skinItemCardBgGreen   },
            { ColorSwatches.Orange,     skinItemCardBgOrange  },
            { ColorSwatches.Magenta,    skinItemCardBgMagenta },
            { ColorSwatches.Purple,     skinItemCardBgPurple  },
            { ColorSwatches.Yellow,     skinItemCardBgYellow  },
        };

        foreach (var kvp in skinsLookupTable)
        {
            var skinData = kvp.Value;

            Instantiate(skinItemCard, skinsScrollViewContent)
                .TryGetComponent(out SkinShopItemCard card);

            var data = new SkinItemData
            {
                ID              = skinData.Id,
                Name            = skinData.Name,
                PreviewImage    = skinData.PreviewImage,
                Price           = skinData.Price,
                CostCurrency    = skinData.Cost,
                ColorCategory   = skinData.ColorCategory,
                MaterialFile    = skinData.MaterialFilename,
            };

            card.SetBackground(cardBackgroundMapping[skinData.ColorCategory]);
            card.SetItemData(data);
            card.SetToggleGroup(skinsItemsToggleGroup);

            skinItemCards.Add(card);
            yield return null;
        }
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

    private IEnumerator FilterItems(ColorSwatches colorFilter, Action afterFilter = null)
    {
        skinColorFilter.SetInteractable(false);
        skinsScrollViewport.color = viewportTransparent;
        CommonEventNotifier.NotifyObserver(CommonEventTags.INDEFINITE_LOADER_SHOW);

        // We will use this to identify which item card was currently active.
        Transform activeItemCard = default;
        var userData = gsm.UserSessionData;
        var activeBlockSkins = gsm.UserSessionData.ActiveBlockSkins.ToList();
        
        foreach (var card in skinItemCards)
        {
            var cardData = card.GetItemData();
            var activate = cardData.ColorCategory == colorFilter;

            card.gameObject.SetActive(activate);

            var id = cardData.ID;

            if (userData.OwnedBlockSkinIDs.Contains(id))
            {
                // Although an item was already owned, we cant mark it as active
                // unless it is currently in use.
                var markActive = activeBlockSkins.Contains(id);

                card.SetOwned(markActive);

                if (cardData.ColorCategory == colorFilter && activeBlockSkins.Contains(id))
                {
                    activeItemCard = card.transform;
                    m_activeItemCard = card;
                }
            }

            yield return null;
        }

        // Move the active item card to the very top of scrollview
        if (activeItemCard != null)
            activeItemCard.SetAsFirstSibling();

        ResetScrollView(scrollRect);

        yield return new WaitForSeconds(1.0F);

        CommonEventNotifier.NotifyObserver(CommonEventTags.INDEFINITE_LOADER_HIDE);
        skinsScrollViewport.color = viewportOpaque;
        skinColorFilter.SetInteractable(true);

        ShopMenuPageCommonNotifier.NotifyObserver(ShopMenuPageEventNames.SkinsPageFullyShown);
        afterFilter?.Invoke();
    }

    /// <summary>
    /// Reset the scroll view back to top.
    /// 
    /// <list type="bullet">
    ///     <item>
    ///         <term>0</term>
    ///         <description>Bottom</description>
    ///     </item>
    ///     <item>
    ///         <term>1</term>
    ///         <description>Top</description>
    ///     </item>
    /// </list>
    /// </summary>
    public void ResetScrollView(ScrollRect scrollRect) => scrollRect.verticalNormalizedPosition = 1.0F;

    private IEnumerator UseSkin(SkinItemData data, UserData userData, Action onComplete = null)
    {
        // Change the active block skin to this id
        switch (data.ColorCategory)
        {
            case ColorSwatches.Blue: userData.ActiveBlockSkins.Blue = data.ID; break;
            case ColorSwatches.Green: userData.ActiveBlockSkins.Green = data.ID; break;
            case ColorSwatches.Magenta: userData.ActiveBlockSkins.Magenta = data.ID; break;
            case ColorSwatches.Orange: userData.ActiveBlockSkins.Orange = data.ID; break;
            case ColorSwatches.Purple: userData.ActiveBlockSkins.Purple = data.ID; break;
            case ColorSwatches.Yellow: userData.ActiveBlockSkins.Yellow = data.ID; break;
        }

        // Load and apply the purchased skin's material
        var materialPath = $"{RuntimeMaterialsPath}/{data.ColorCategory}/{data.MaterialFile}";
        var material = Resources.Load<Material>(materialPath);

        gsm.SetActiveBlockSkinMaterial(data.ColorCategory, material);
        gsm.UserSessionData = userData;

        // Write the changes to file
        yield return StartCoroutine(userDataHelper.SaveUserData(userData, (d) => onComplete?.Invoke() ));
    }

    public List<SkinShopItemCard> SkinItems => skinItemCards;

    #region EVENT_OBSERVERS
    void OnEnable()
    {
        BlockSkinShopItemClickNotifier.BindObserver(ObserveBlockSkinItemClicked);
        BlockSkinItemPurchaseNotifier.BindEvent(ObserveBlockSkinPurchase);
        ShopMenuShownNotifier.BindEvent(HandleShopMenuRootShown);
        ColorSelectEventNotifier.BindObserver(ObserveColorFilterEvent);

        ResetMenu();
    }

    void OnDisable()
    {
        BlockSkinShopItemClickNotifier.UnbindObserver(ObserveBlockSkinItemClicked);
        BlockSkinItemPurchaseNotifier.UnbindEvent(ObserveBlockSkinPurchase);
        ShopMenuShownNotifier.UnbindEvent(HandleShopMenuRootShown);
        ColorSelectEventNotifier.UnbindObserver(ObserveColorFilterEvent);
    }

    /// <summary>
    /// Listen to the event when the user buys a skin. This happens when the
    /// "BUY" button was clicked.
    /// </summary>
    /// <param name="sender"></param>
    private void ObserveBlockSkinPurchase(SkinShopItemCard sender)
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
            
            buySkinPrompt.Hide();
            buySkinsResultDialog.ShowFailResult(msg, data.PreviewImage);
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
        if (!userData.OwnedBlockSkinIDs.Contains(data.ID))
            userData.OwnedBlockSkinIDs.Add(data.ID);

        // Equip the skin
        StartCoroutine(UseSkin(data, userData, onComplete: () => {

            PlayerCurrencyNotifier.NotifyObserver(new PlayerCurrencyEventArgs
            {
                Currency      = data.CostCurrency,
                Amount        = playerBalance,
                Animate       = true,
                AnimationType = PlayerCurrencyParticleAnimator.ANIMATION_MODE_DECREASE_AMOUNT
            });

            buySkinPrompt.Hide();
            sender.SetOwned(true);
            m_activeItemCard = sender;
            
            buySkinsResultDialog.ShowSuccessResult(data.PreviewImage, $"This skin has been automatically applied for {data.ColorCategory} blocks");
            ProgressLoaderNotifier.NotifyFourSegment(false);
        }));
    }

    private void ObserveBlockSkinItemClicked(SkinShopItemCard sender)
    {
        Debug.Log("Who sent --> " + sender.gameObject.name);

        var skinData = sender.GetItemData();

        var nullcheck = new Dictionary<string, object>
        {
            {"Skin Data", skinData},
            {"OwnedIDS", gsm.UserSessionData.OwnedBlockSkinIDs}
        };

        foreach (var kvp in nullcheck)
        {
            if (kvp.Value == null)
            {
                Debug.Log($"{kvp.Key} is NULL!");
            }
        }

        if (!gsm.UserSessionData.OwnedBlockSkinIDs.Contains(skinData.ID))
        {
            buySkinPrompt.gameObject.SetActive(true);
            buySkinPrompt.SetEventSender(sender);
            buySkinPrompt.Show();
            return;
        }

        CommonEventNotifier.NotifyObserver(CommonEventTags.INDEFINITE_LOADER_SHOW);

        StartCoroutine(UseSkin(skinData, gsm.UserSessionData, () => {
            sender.Toggle(true);
            m_activeItemCard = sender;
            CommonEventNotifier.NotifyObserver(CommonEventTags.INDEFINITE_LOADER_HIDE);
        }));
    }

    private void ObserveColorFilterEvent(ColorSelectEventNames eventName, object data)
    {
        if (eventName.Equals(ColorSelectEventNames.ColorSelected))
        {
            if (data == null)
                return;
            
            var selectData = (ColorSelectValueCallback)data;

            // var color = (ColorSwatches)data;
            StartCoroutine(FilterItems(selectData.ColorValue));
        }
    }
    #endregion EVENT_OBSERVERS
}