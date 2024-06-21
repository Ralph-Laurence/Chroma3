using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SkinShopItemCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameLabel;
    [SerializeField] private TextMeshProUGUI priceLabel;
    [SerializeField] private Image previewImage;
    [SerializeField] private GameObject activeIndicator;
    private SkinItemData _itemData;

    /// <summary>
    /// The root button must be disabled first before showing the
    /// deactivate button to prevent conflicting events.
    /// The deactivate button should only appear when a block skin item
    /// is currently active.
    /// </summary>
    [SerializeField] private Button deactivateButton;

    private Button rootButton;

    private bool isInitialized;

    void OnEnable() => Initialize();

    void Start() => Initialize();

    private void Initialize()
    {
        if (isInitialized)
            return;

        TryGetComponent(out rootButton);

        rootButton.onClick.AddListener(HandleRootButtonClicked);
        deactivateButton.onClick.AddListener(UnequipSkin);

        ApplyVisualData(_itemData);
        ToggleActive(false);
        
        isInitialized = true;
    }

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
    }

    public void ToggleActive(bool active)
    {
        activeIndicator.SetActive(active);
        rootButton.enabled = !active;
    }

    private void HandleRootButtonClicked()
    {
        Debug.Log("Item was clicked");
    }

    private void UnequipSkin()
    {
        Debug.Log("Unequip item");
    }
}
