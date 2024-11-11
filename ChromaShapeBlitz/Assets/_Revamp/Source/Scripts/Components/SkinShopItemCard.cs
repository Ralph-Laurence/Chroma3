using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SkinShopItemCard : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector]
    public void OnPointerClick(PointerEventData eventData) => HandleClicked();
    
    [SerializeField] private TextMeshProUGUI itemNameLabel;
    [SerializeField] private TextMeshProUGUI priceLabel;
    [SerializeField] private Image cardBackground;
    [SerializeField] private Image previewImage;
    [SerializeField] private GameObject activeIndicator;
    private SkinItemData _itemData;

    private Toggle toggle;

    private bool isInitialized;
    public bool IsOwned;

    //public string CardIdentifier { get; private set;}

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
    public void SetItemData(SkinItemData data)
    {
        _itemData = data;
        ApplyVisualData(_itemData);
    }

    public SkinItemData GetItemData() => _itemData;

    /// <summary>
    /// Apply the data into the UI
    /// </summary>
    /// <param name="data">The data</param>
    private void ApplyVisualData(SkinItemData data)
    {
        if (data == null)
            return;

        previewImage.sprite = data.PreviewImage;
        itemNameLabel.text = data.Name;

        var cost = data.CostCurrency == CurrencyType.Coin
                 ? $"<style=\"Coin\">{data.Price}"
                 : $"<style=\"Gem\">{data.Price}";

        priceLabel.text = cost;

        // The format of card identifier is "Color-(dash)-ItemID
        // CardIdentifier = $"{data.ColorCategory}-{data.ID}";
    }

    public void SetBackground(Sprite background) => cardBackground.sprite = background;
    public void SetOwned(bool toggleActive = false)
    {
        IsOwned = true;
        toggle.isOn = toggleActive;
        priceLabel.text = "Owned";
    }

    private void HandleClicked()
    {
        // If this item is currently active, ignore clicks
        if (toggle.isOn)
            return;

        BlockSkinShopItemClickNotifier.NotifyObserver(sender: this);
    }

    public SkinItemData GetSkinItemData() => _itemData;

    /// <summary>
    /// Invoke the click event manually. This is useful during tutorial
    /// </summary>
    public void InvokeClick() => BlockSkinShopItemClickNotifier.NotifyObserver(sender: this);
}
