using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rv_SkinsMenuController : MonoBehaviour
{
    [Space(10)]
    [Header("Main Behaviour")]
    [SerializeField] private GameObject scrollViewContent;
    [SerializeField] private GameObject skinItemPrefab;
    [SerializeField] private Rv_ColorVariationFilter colorVariationFilter;
    private ToggleGroup toggleGroup;
    private CustomBlockSkinsHelper skinsHelper;
    private List<BlockSkinShopItem> blockSkinItems;
    private Dictionary<ColorSwatches, Sprite> itemCardFrames;

    [Space(10)]
    [Header("Item Card Frames")]
    [SerializeField] private Sprite blueItemCard;
    [SerializeField] private Sprite greenItemCard;
    [SerializeField] private Sprite magentaItemCard;
    [SerializeField] private Sprite orangeItemCard;
    [SerializeField] private Sprite purpleItemCard;
    [SerializeField] private Sprite yellowItemCard;

    void Awake()
    {
        skinsHelper = CustomBlockSkinsHelper.Instance;
        scrollViewContent.TryGetComponent(out toggleGroup);

        itemCardFrames = new Dictionary<ColorSwatches, Sprite>
        {
            { ColorSwatches.Blue    , blueItemCard    },
            { ColorSwatches.Green   , greenItemCard   },
            { ColorSwatches.Magenta , magentaItemCard },
            { ColorSwatches.Orange  , orangeItemCard  },
            { ColorSwatches.Purple  , purpleItemCard  },
            { ColorSwatches.Yellow  , yellowItemCard  }
        };
    }

    // Start is called before the first frame update.
    void Start() 
    {
        StartCoroutine
        (
            // Fill the scrollview of Skin Shop Items
            BuildShopItemsSelectionMenu(scrollViewContent)
        );

        var progressData = PlayerProgressHelper.Instance.GetProgressData();

        OnUpdatePlayerBalanceUINotifier.Publish(new PlayerBalance
        {
            TotalCoins = progressData.CurrentCoins,
            TotalGems  = progressData.CurrentGems
        });
    }

    /// <summary>
    /// Add each skin shop item into the scrollview.
    /// </summary>
    /// <param name="scrollContainer">The scrollview that should hold the items</param>
    private IEnumerator BuildShopItemsSelectionMenu(GameObject scrollContainer)
    {
        blockSkinItems?.Clear();
        blockSkinItems = new List<BlockSkinShopItem>();

        foreach(var skins in skinsHelper.SkinObjects)
        {
            var skin    = skins.Value.SkinInfo;
            var itemObj = Instantiate(skinItemPrefab);

            itemObj.TryGetComponent(out BlockSkinShopItem skinItem);
            
            var isOwned  = skinsHelper.OwnedSkinIds.Contains(skin.Id);
            var itemInfo = new BlockSkinShopItemInfo
            {
                IsOwned        = isOwned,
                Id             = skin.Id,
                Name           = skin.Name,
                Cost           = skin.Cost,
                Price          = skin.Price,
                ColorCategory  = skin.ColorCategory,
                PreviewImage   = skin.PreviewImage
            };

            skinItem.SetItemInfo(itemInfo);
            skinItem.SetToggleGroup(toggleGroup);
            skinItem.DisplayDescriptors(itemInfo);
            skinItem.SetFrameBackground(itemCardFrames[skin.ColorCategory]);

            blockSkinItems.Add(skinItem);

            itemObj.transform.SetParent(scrollContainer.transform, false);
            yield return null;
        }
        // Show only the items according to the selected color filter
        FilterSkins();
    }

    /// <summary>
    /// Show only purchasable items by the given color filter.
    /// </summary>
    /// <param name="color">
    /// The color filter which is, the color group the skin belongs to
    /// </param>
    private void FilterShopItemsByColor(ColorSwatches color)
    {
        if (blockSkinItems == null)
            return;

        var equippedSkinId = skinsHelper.SkinIdsInUse[color];

        blockSkinItems.ForEach(item => {

            var skinItemInfo = item.GetSkinInfo();
            
            if (skinItemInfo.ColorCategory != color)
            {
                item.ToggleUnequipped();
                item.Hide();
            }
            else
            {
                if (skinItemInfo.IsOwned)
                    item.HidePriceLabel();
                
                if (equippedSkinId == skinItemInfo.Id)
                    item.ToggleEquipped();
                    
                item.Show();
            }
        });
    }

    private void FilterSkins() => FilterShopItemsByColor( colorVariationFilter.SelectedColor );
}