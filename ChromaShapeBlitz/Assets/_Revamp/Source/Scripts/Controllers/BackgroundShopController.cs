using System.Collections.Generic;
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

    void Awake()
    {
        gsm = GameSessionManager.Instance;
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
    }

    void OnDisable()
    {
        BackgroundShopItemClickNotifier.UnbindEvent(ObserveBackgroundShopItemClicked);    
    }

    private void BuildShopMenu()
    {
        var ownedBackgrounds = gsm.UserSessionData.OwnedBackgroundIds;

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
            {
                card.SetOwned(true);
            }
        }
    }

    #region EVENT_OBSERVERS
    private void ObserveBackgroundShopItemClicked(BackgroundShopItemCard sender)
    {
        buyConfirmPrompt.gameObject.SetActive(true);
        buyConfirmPrompt.SetEventSender(sender);
        buyConfirmPrompt.Show();
    }
    #endregion
}
