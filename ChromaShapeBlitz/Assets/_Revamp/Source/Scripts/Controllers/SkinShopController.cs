using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private Dictionary<int, BlockSkinsAsset> skinsLookupTable;
    private List<SkinShopItemCard> skinItemCards;

    private readonly Color viewportTransparent = new(1.0F, 1.0F, 1.0F, 0.0F);
    private readonly Color viewportOpaque = new(1.0F, 1.0F, 1.0F, 1.0F);

    private ToggleGroup skinsItemsToggleGroup;

    private GameSessionManager gsm;
    private UserDataHelper userDataHelper;

    void Awake()
    {
        skinsLookupTable = new();
        skinItemCards = new();

        gsm = GameSessionManager.Instance;
        userDataHelper = UserDataHelper.Instance;

        skinsScrollViewContent.TryGetComponent(out skinsItemsToggleGroup);

        blockSkinGroups.ForEach(group => group.AddTo(skinsLookupTable));
    }

    void Start()
    {
        BuildSkinShopMenu();
        StartCoroutine(FilterItems(skinColorFilter.Value));

        skinColorFilter.OnColorSelected += (color) => StartCoroutine(FilterItems(color));
    }

    private void BuildSkinShopMenu()
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
        }
    }

    private IEnumerator FilterItems(ColorSwatches colorFilter)
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
            var activate = card.GetItemData().ColorCategory == colorFilter;
            card.gameObject.SetActive(activate);
            var id = card.GetItemData().ID;

            if (userData.OwnedBlockSkinIDs.Contains(id))
            {
                // Although an item was already owned, we cant mark it as active
                // unless it is currently in use.
                var markActive = activeBlockSkins.Contains(id);

                card.SetOwned(markActive);
                activeItemCard = card.transform;
            }
            else
                card.Toggle(false);

            yield return null;
        }

        // Move the active item card to the very top of scrollview
        if (activeItemCard != null)
            activeItemCard.SetSiblingIndex(0);

        ResetScrollView(scrollRect);

        yield return new WaitForSeconds(1.0F);

        CommonEventNotifier.NotifyObserver(CommonEventTags.INDEFINITE_LOADER_HIDE);
        skinsScrollViewport.color = viewportOpaque;
        skinColorFilter.SetInteractable(true);
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

    #region EVENT_OBSERVERS
    void OnEnable()
    {
        BlockSkinShopItemClickNotifier.BindEvent(ObserveBlockSkinItemClicked);
        BlockSkinItemPurchaseNotifier.BindEvent(ObserveBlockSkinPurchase);
    }

    void OnDisable()
    {
        BlockSkinShopItemClickNotifier.UnbindEvent(ObserveBlockSkinItemClicked);
        BlockSkinItemPurchaseNotifier.UnbindEvent(ObserveBlockSkinPurchase);
    }

    /// <summary>
    /// Listen to the event when the user buys a skin. This happens when the
    /// "BUY" button was clicked.
    /// </summary>
    /// <param name="sender"></param>
    private void ObserveBlockSkinPurchase(SkinShopItemCard sender)
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
        if (!userData.OwnedBlockSkinIDs.Contains(data.ID))
            userData.OwnedBlockSkinIDs.Add(data.ID);

        // Equip the skin
        StartCoroutine(UseSkin(data, userData, onComplete: () => {

            PlayerCurrencyNotifier.NotifyObserver(new PlayerCurrencyEventArgs
            {
                Currency = data.CostCurrency,
                Amount = playerBalance
            });

            buySkinPrompt.Hide();
            sender.SetOwned(true);
            CommonEventNotifier.NotifyObserver(CommonEventTags.INDEFINITE_LOADER_HIDE);
        }));
    }

    private void ObserveBlockSkinItemClicked(SkinShopItemCard sender)
    {
        var skinData = sender.GetItemData();

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
            CommonEventNotifier.NotifyObserver(CommonEventTags.INDEFINITE_LOADER_HIDE);
        }));
    }
    #endregion EVENT_OBSERVERS
}