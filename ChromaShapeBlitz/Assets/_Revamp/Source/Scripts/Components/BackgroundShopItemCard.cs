using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackgroundShopItemCard : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector]
    public void OnPointerClick(PointerEventData eventData) => HandleClicked();
    
    [SerializeField] private TextMeshProUGUI itemNameLabel;
    [SerializeField] private TextMeshProUGUI priceLabel;
    [SerializeField] private Image previewImage;
    [SerializeField] private GameObject activeIndicator;
    [SerializeField] private GameObject ownedIndicator;
    private BackgroundItemData _itemData;

    private Toggle toggle;

    private bool isInitialized;
    public bool IsOwned;

    void OnEnable() => Initialize();

    void Start() => Initialize();

    private void Initialize()
    {
        if (isInitialized)
            return;

        TryGetComponent(out toggle);
        ApplyVisualData(_itemData);
        
        isInitialized = true;
    }

    public void SetToggleGroup(ToggleGroup toggleGroup) => toggle.group = toggleGroup;
    public void Toggle(bool toggled) => toggle.isOn = toggled;
    public bool IsMarkedActive => toggle.isOn;
    public void SetItemData(BackgroundItemData data)
    {
        _itemData = data;
        ApplyVisualData(_itemData);
    }

    public BackgroundItemData GetItemData() => _itemData;

    /// <summary>
    /// Apply the data into the UI
    /// </summary>
    /// <param name="data">The data</param>
    private void ApplyVisualData(BackgroundItemData data)
    {
        if (data == null)
            return;

        previewImage.sprite = data.PreviewImage;
        itemNameLabel.text = data.Name;

        var cost = data.CostCurrency == CurrencyType.Coin
                 ? $"<style=\"Coin\">{data.Price}"
                 : $"<style=\"Gem\">{data.Price}";

        priceLabel.text = cost;
    }

    public void SetOwned(bool toggleActive = false)
    {
        IsOwned = true;
        toggle.isOn = toggleActive;
        priceLabel.gameObject.SetActive(false); //text = "Owned";
        ownedIndicator.SetActive(true);
    }

    private void HandleClicked()
    {
        // If this item is currently active, ignore clicks
        if (toggle.isOn)
            return;

        BackgroundShopItemClickNotifier.NotifyObserver(this);
    }

    /// <summary>
    /// Invoke the click event manually. This is useful during tutorial
    /// </summary>
    public void InvokeClick() => BackgroundShopItemClickNotifier.NotifyObserver(sender: this);
}
