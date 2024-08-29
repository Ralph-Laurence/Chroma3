using System.Collections;
using System.Collections.Generic;
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
    //private BackgroundShopItemCard          m_activeItemCard;

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
        PowerupShopItemClickNotifier.BindEvent(ObservePowerupPurchase);
    }

    void OnDisable()
    {
        PowerupShopItemClickNotifier.UnbindEvent(ObservePowerupPurchase);
    }

    private void ObservePowerupPurchase(PowerupShopItemCard sender)
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

    public IEnumerator BuildShopMenu()
    {
        foreach (var kvp in powerupsLookUp)
        {
            Instantiate(itemCard, scrollRectContent).TryGetComponent(out PowerupShopItemCard card);
            
            var itemData = kvp.Value;

            card.SetItemData(new PowerupItemData
            {
                Name            = itemData.Name,
                Cost            = itemData.Cost,
                Description     = itemData.Description,
                Price           = itemData.Price,
                PreviewImage    = itemData.PreviewImage,
                ActivationMode  = itemData.ActivationMode,
                ItemType        = itemData.ItemType,
            });

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
