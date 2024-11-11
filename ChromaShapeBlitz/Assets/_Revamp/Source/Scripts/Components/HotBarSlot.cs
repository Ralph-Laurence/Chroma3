using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HotbarSlotBehaviours have three modes which are:
/// <list type="bullet">
///     <item>
///         <term>ReadOnly</term>
///         <description>The slot is disabled for interactions</description>
///     </item>
///     <item>
///         <term>InventorySlot</term>
///         <description>The slot is configured for inventory page</description>
///     </item>
///     <item>
///         <term>InGameSlot</term>
///         <description>The slot should execute its powerup</description>
///     </item>
/// </list>
/// </summary>
public enum HotbarSlotBehaviours
{
    ReadOnly,
    InventorySlot,
    InGameSlot,
    Disabled
}

public struct HotbarSlotDataSource
{                 
    public int    ItemId;
    public Sprite ItemCountSprite;
    public Sprite ItemThumbnail;
}

public class HotBarSlot : MonoBehaviour
{
    [SerializeField] private Image thumbnail;
    [SerializeField] private Image countIndicator;
    
    private Button m_button;
    private bool slotLocked;
    private int  itemId;
    public bool  IsSlotFilled;
    public int   SlotIndex;

    void Awake()
    {
        TryGetComponent(out m_button);

        if (m_button != null)
            m_button.onClick.AddListener(ObserveHotbarSlotSelected);
    }

    public void FillItem(HotbarSlotDataSource data)
    {
        thumbnail.enabled       = true;
        countIndicator.enabled  = true;

        thumbnail.sprite        = data.ItemThumbnail;
        countIndicator.sprite   = data.ItemCountSprite;
        itemId                  = data.ItemId;

        IsSlotFilled            = true;
    }

    public void Reset()
    {
        thumbnail.sprite        = default;
        countIndicator.sprite   = default;
        itemId                  = default;

        thumbnail.enabled       = false;
        countIndicator.enabled  = false;
        IsSlotFilled            = false;
    }

    private void ObserveHotbarSlotSelected()
    {
        HotbarSlotSelectedNotifier.NotifyObserver(SlotIndex);
    }

    /// <summary>
    /// Get the ID of the item data attached to this slot
    /// </summary>
    public int GetItemID() => itemId;

    /// <summary>
    /// Set the count of the current item that this slot is holding,
    /// which is shown on the indicator.
    /// </summary>
    public void SetCountIndicator(Sprite countIndex) => countIndicator.sprite = countIndex;

    /// <summary>
    /// Make the slot NOT interactable for a given duration,
    /// the make it interactable again
    /// </summary>
    public void BeginLockSlot(int duration)
    {
        if (slotLocked) return;

        StartCoroutine(CooldownHotbarSlot(duration));
    }

    private IEnumerator CooldownHotbarSlot(int duration)
    {
        slotLocked = true;
        m_button.interactable = false;

        var timeRemaining = duration;
        var wait = new WaitForSeconds(1.0F);

        while (timeRemaining > 0.0F)
        {
            if (Time.timeScale > 0.0F)
            {
                yield return wait;
                timeRemaining--;
            }
            else
            {
                // Wait for the next frame if the game is paused
                yield return null;
            }
        }

        slotLocked = false;
        m_button.interactable = true;
    }

    public void SetInteractable() => m_button.interactable = true;
    public void DisableInteraction() => m_button.interactable = false;
}