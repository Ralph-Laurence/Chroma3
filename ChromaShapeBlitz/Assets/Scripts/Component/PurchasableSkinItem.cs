using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PurchasableSkinItem : MonoBehaviour, IPointerClickHandler
{
    //[SerializeField] private UnityEvent whenClicked;

    public void OnPointerClick(PointerEventData eventData) => HandleClicked();
    // {
    //     if (whenClicked != null)
    //         whenClicked.Invoke();
    // }
    
    [SerializeField] private TextMeshProUGUI itemNameLabel;
    [SerializeField] private TextMeshProUGUI itemPriceLabel;
    [SerializeField] private Image           itemPreviewImage;

    private Toggle equipToggle;
    private SkinInfo skinInfo;
    private CustomizationsHelper themeHelper;

    void Awake()
    {
        TryGetComponent(out equipToggle);

        if (equipToggle == null)
            Debug.LogWarning("Equip toggle is null");

        themeHelper = CustomizationsHelper.Instance;
    }

    public void SetSkinInfo(SkinInfo skinInfo)
    {
        this.skinInfo = skinInfo;
        InitializeComponent();
    }
    
    public SkinInfo GetSkinInfo() => skinInfo;

    public void InitializeComponent()
    {
        itemPreviewImage.sprite = skinInfo.PreviewImage;
        itemNameLabel.text      = skinInfo.Name;
        itemPriceLabel.text     = skinInfo.Cost.PrefixWithCurrencyIcon(skinInfo.Price);
    }

    public void SetToggleGroup(ToggleGroup toggleGroup) => equipToggle.group = toggleGroup;
    // public void OnToggled(Action<PurchasableSkinItem> skinItem) 
    // {
    //     equipToggle.onValueChanged.AddListener(delegate { skinItem() });
    // }
    
    
    public void ToggleEquipped(bool silent = false)
    {
        // When silent = true, it does not trigger the value changed event
        if (silent)
        {
            equipToggle.SetIsOnWithoutNotify(true);
            return;
        }
        equipToggle.isOn = true;
    }
    public void ToggleUnequipped(bool silent = false)
    {
        // When silent = true, it does not trigger the value changed event
        if (silent)
        {
            equipToggle.SetIsOnWithoutNotify(false);
            return;
        }

        equipToggle.isOn = false;
    }

    public bool IsToggled => equipToggle.isOn;

    public void Hide() => gameObject.SetActive(false);
    public void Show() => gameObject.SetActive(true);

    public void HidePriceLabel() => itemPriceLabel.transform.parent.gameObject.SetActive(false);



    //==================================//
    // NEW FUNCTIONS //

    // Must be assigned in UI Event Trigger | Pointer Click
    public void HandleClicked() => OnPurchasableSkinClickedNotifier.Publish(this);

    // Called when the "Buy" button (from the dialog) was clicked
    public void HandlePurchase()
    {
        StartCoroutine(PurchaseSkin());
    }

    private IEnumerator PurchaseSkin()
    {
        // if (themeHelper == null || progressHelper == null)
        // {
        //     Failed..
        // }

        var playerProgress = PlayerProgressHelper.Instance;
        var progressData = playerProgress.GetProgressData();

        switch (skinInfo.Cost)
        {
            case CurrencyType.Coin:
                if (progressData.CurrentCoins < skinInfo.Price)
                {
                    Debug.LogWarning("Not enough coins");
                    yield break;
                }
                yield return StartCoroutine(playerProgress.DecreasePlayerBank(CurrencyType.Coin, skinInfo.Price, true));
                break;

            case CurrencyType.Gem:
                if (progressData.CurrentGems < skinInfo.Price)
                {
                    Debug.LogWarning("Not enough coins");
                    yield break;
                }
                yield return StartCoroutine(playerProgress.DecreasePlayerBank(CurrencyType.Gem, skinInfo.Price, true));
                break;
        }

        yield return StartCoroutine(themeHelper.TakeSkinOwnership(skinInfo.Id, skinInfo.ColorCategory));
        
        equipToggle.isOn = true;
        HidePriceLabel();

        skinInfo.IsOwned = true;

        OnSkinPurchasedNotifier.Publish();
        
        // Get the updated player balance,
        // then show it onto the UI
        progressData = playerProgress.GetProgressData();

        OnUpdatePlayerBalanceUINotifier.Publish(new PlayerBalance{
            TotalCoins = progressData.CurrentCoins,
            TotalGems  = progressData.CurrentGems
        });

        yield return null;
    }
}
