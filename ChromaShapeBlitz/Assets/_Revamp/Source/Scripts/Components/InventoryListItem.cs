using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryListItem : MonoBehaviour
{
    private InventoryItemData itemData;

    [SerializeField] private Image thumbnail;
    [SerializeField] private Image countIndicator;
    [SerializeField] private TextMeshProUGUI itemNameLabel;

    private Button m_button;

    private void Awake()
    {
        TryGetComponent(out m_button);

        if (m_button != null)
        {
            // Broadcast an event that this item was selected and pass the event sender.
            m_button.onClick.AddListener(() =>
            {
                InventoryListItemSelectedNotifier.NotifyObserver(this);
            });
        }
    }

    public void RenderItemDetails()
    {
        thumbnail.sprite      = itemData.Thumbnail;
        itemNameLabel.text    = itemData.Name;
        countIndicator.sprite = itemData.AmountIcon;
    }

    public void SetData(InventoryItemData itemData)
    {
        this.itemData = itemData;
        RenderItemDetails();

        // For non-consumable items, prevent the button from processing clicks
        if (itemData.ItemType != InventoryItemType.Consumable && m_button != null)
        {
            m_button.interactable = false;
            return;
        }
    }

    //
    // Useful mostly during tutorial session
    //
    public InventoryItemData GetItemData() => itemData;
    public void InvokeClick() => InventoryListItemSelectedNotifier.NotifyObserver(this);
}