using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkinsMenuController : MonoBehaviour
{
    [Space(5)]
    [Header("ScrollView")]
    [SerializeField] private ScrollRect scrollViewRect;

    [Space(5)]
    [Header("Color Filtering")]
    [SerializeField] private ToggleGroup skinColorFiltersGroup;
    [SerializeField] private SkinFilterIndicator skinFilterIndicator;

    [Space(5)]
    [Header("Purchasable Skin Items")]
    [SerializeField] private ToggleGroup purchasableSkinsToggleGroup;
    [SerializeField] private GameObject skinsSelectionScrollContent;
    [SerializeField] private GameObject skinItemPrefab;
    [SerializeField] private PurchaseSkinDialog purchaseSkinDialog;

    private CustomizationsHelper themeHelper;

    private List<Toggle> skinColorFilters;
    private List<PurchasableSkinItem> purchasableSkinItems;

    // Start is called before the first frame update
    void Start()
    {
        themeHelper = CustomizationsHelper.Instance;

        // Get all toggles inside the toggle group
        skinColorFilters = new List<Toggle>(skinColorFiltersGroup.GetComponentsInChildren<Toggle>());

        // Fill the scrollview of Skin Shop Items
        StartCoroutine( BuildPurchasableSkinItemsSelectionMenu(skinsSelectionScrollContent) );
    }

    /// <summary>
    /// This is called at UI Event level
    /// </summary>
    public void FilterSkinVariations()
    {
        var filter = GetSelectedSkinColorFilter();

        if (filter != null)
            FilterSkinsByColor(filter.ColorValue);
    }

    /// <summary>
    /// Get the skin variation filter attached to the selected toggle.
    /// </summary>
    /// <returns></returns>
    private SkinVariationFilter GetSelectedSkinColorFilter()
    {
        //var selectedFilter = skinColorFilters.FirstOrDefault(toggle => toggle.isOn);
        
        Toggle selectedFilter = default;

        foreach (var toggle in skinColorFilters)
        {
            if (toggle.isOn)
            {
                selectedFilter = toggle;
                break;
            }
        }

        // No active filters
        if (selectedFilter == null)
            return null;

        selectedFilter.TryGetComponent(out SkinVariationFilter option);
        return option;
    }

    /// <summary>
    /// Show only purchasable items by the given color filter.
    /// </summary>
    /// <param name="color">
    /// The color filter which is the color group
    /// the skin belongs to
    /// </param>
    private void FilterSkinsByColor(BlockColors color)
    {
        var equippedSkinId = themeHelper.GetEquipedSkinIds()[color];

        purchasableSkinItems.ForEach(item => {

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

        skinFilterIndicator.DrawIndicator(color);
    }
    
    /// <summary>
    /// Add each purchasable skin item into the scrollview.
    /// </summary>
    /// <param name="scrollContainer"></param>
    /// <returns></returns>
    private IEnumerator BuildPurchasableSkinItemsSelectionMenu(GameObject scrollContainer)
    {
        purchasableSkinItems = new List<PurchasableSkinItem>();

        var defaultSkins     = themeHelper.GetDefaultSkinIds();
        var ownedSkinIds     = themeHelper.GetOwnedSkinIds();

        foreach(var skins in themeHelper.GraphicSkins)
        {
            var blockSkin = skins.Value;
            var itemObj   = Instantiate(skinItemPrefab);

            itemObj.TryGetComponent(out PurchasableSkinItem skinItem);
            
            var isOwned = (
                ownedSkinIds.Contains(blockSkin.Id) ||
                defaultSkins.Contains(blockSkin.Id)
            );

            skinItem.SetSkinInfo(new SkinInfo
            {
                IsOwned       = isOwned,
                Id            = blockSkin.Id,
                Name          = blockSkin.Name,
                Cost          = blockSkin.Cost,
                Price         = blockSkin.Price,
                ColorCategory = blockSkin.Category,
                PreviewImage  = blockSkin.PreviewImage
            });
                
            skinItem.SetToggleGroup(purchasableSkinsToggleGroup);
            skinItem.InitializeComponent();

            purchasableSkinItems.Add(skinItem);

            itemObj.transform.SetParent(scrollContainer.transform, false);
            yield return null;
        }

        // Show only the items according to the default color filter
        FilterSkinVariations();
    }
}
