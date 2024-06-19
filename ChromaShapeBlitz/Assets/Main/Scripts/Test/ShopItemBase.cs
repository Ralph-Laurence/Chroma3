using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ShopItemBase : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// Called when the "Buy" button (from the dialog) was clicked
    /// </summary>
    public abstract void HandlePurchase();

    [HideInInspector]
    public void OnPointerClick(PointerEventData eventData) => HandleItemClicked();

    //protected CustomizationsHelper themeHelper;

    [SerializeField] private AudioClip clickedSfx;
    public TextMeshProUGUI ItemNameLabel;
    public TextMeshProUGUI ItemPriceLabel;
    public Image           ItemPreviewImage;

    private Toggle equipToggle;

    //private BaseItemInfo itemInfo;
    private SfxManager sfxManager;

    void Awake() => OnAwake();

    protected virtual void OnAwake()
    {
        sfxManager = SfxManager.Instance;

        TryGetComponent(out equipToggle);

        if (equipToggle == null)
            Debug.LogWarning("Equip toggle is null");

        //themeHelper = CustomizationsHelper.Instance;
    }

    /// <summary>
    /// Called when this item was selected
    /// </summary>
    public virtual void HandleItemClicked()
    {
        if (sfxManager != null)
            sfxManager.PlaySfx(clickedSfx);
    }

    /// <summary>
    /// Draw or Re-Draw the card's ui elements with visual values
    /// </summary>
    public void DisplayDescriptors(BaseItemInfo itemInfo)
    {
        ItemPreviewImage.sprite = itemInfo.PreviewImage;
        ItemNameLabel.text      = itemInfo.Name;
        ItemPriceLabel.text     = itemInfo.Cost.PrefixWithCurrencyIcon(itemInfo.Price);
    }

    /// <summary>
    /// Set the toggle group that this toggle item should belong to
    /// </summary>
    public void SetToggleGroup(ToggleGroup toggleGroup) => equipToggle.group = toggleGroup;

    /// <summary>
    /// Show the check icon when this item was equipped.
    /// </summary>
    /// <param name="silent">Set this to false if you want to trigger the ValueChanged event</param>
    public void ToggleEquipped(bool silent = true)
    {
        if (silent)
        {
            equipToggle.SetIsOnWithoutNotify(true);
            return;
        }

        equipToggle.isOn = true;
    }

    /// <summary>
    /// Hide the check icon when this item was unequipped.
    /// </summary>
    /// <param name="silent">Set this to false if you want to trigger the ValueChanged event</param>
    public void ToggleUnequipped(bool silent = true)
    {
        if (silent)
        {
            equipToggle.SetIsOnWithoutNotify(false);
            return;
        }

        equipToggle.isOn = false;
    }

    /// <summary>
    /// Get the toggled status of this item.
    /// </summary>
    public bool IsToggled => equipToggle.isOn;

    public void HidePriceLabel() => ItemPriceLabel.gameObject.SetActive(false);

    public void Hide() => gameObject.SetActive(false);
    public void Show() => gameObject.SetActive(true);
}