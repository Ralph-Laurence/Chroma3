using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ShopItemCardBase : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI  itemNameLabel;
    [SerializeField] protected TextMeshProUGUI  priceLabel;
    [SerializeField] protected Image            previewBackground;
    [SerializeField] protected Image            previewImage;
    [SerializeField] protected GameObject       activeIndicator;

    public void SetItemPreviewBackground(Sprite background) => previewBackground.sprite = background;

    private BaseItemData m_itemData;

    protected abstract void HandleClicked();
    public abstract void SetOwned();

    public virtual void SetItemData(BaseItemData itemData)
    {
        m_itemData = itemData;
        ApplyCardViewData(itemData);
    }

    /// <summary>
    /// Get the superclass item data
    /// </summary>
    /// <returns>Base item data</returns>
    public BaseItemData GetItemData() => m_itemData;

    /// <summary>
    /// Present the item's properties into UI elements
    /// </summary>
    /// <param name="itemData"></param>
    protected virtual void ApplyCardViewData(BaseItemData itemData)
    {
        itemNameLabel.text  = itemData.Name;
        previewImage.sprite = itemData.PreviewImage;

        var cost = itemData.Cost == CurrencyType.Coin
                 ? $"<style=\"Coin\">{itemData.Price}"
                 : $"<style=\"Gem\">{itemData.Price}";

        priceLabel.text = cost;
    }
}