using InspectorUtils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
public class DailyGiftItemCard : MonoBehaviour
{
    public int DayNumber;

    [Space(10, order=1)]
    [Help("Powerups (only chosen) are randomized by ID which gives fixed amount to 1. MegaPack give a randomized value of all types.", order=2)]
    public DailyGiftTypes GiftType;
    public int GiftAmount = 1;
    public string GiftName;

    [SerializeField] private Button button;
    [SerializeField] private GameObject claimOverlay;

    public Image CardIcon;

    void Awake()
    {
        // Find the button then attach a click listener
        transform.Find("Button").TryGetComponent(out button);
        button.onClick.AddListener(HandleClicked);

        // Find the "Claimed" overlay indicator
        claimOverlay = transform.Find("Claim Indicator").gameObject;
        transform.Find("Icon").TryGetComponent(out CardIcon);
        
        // Get the sibling index of this card then add (1) to be its day number
        DayNumber = transform.GetSiblingIndex() + 1;
    }
    //===========================================
    //              PRIVATE METHODS
    //===========================================
    private void HandleClicked()
    {
        GiftItemCardClickNotifier.NotifyObserver(this);
    }

    //===========================================
    //              PUBLIC METHODS
    //===========================================
    public void LockGiftCard(bool disableButton = false)
    {
        if (disableButton)
            button.gameObject.SetActive(false);

        button.interactable = false;
    }

    public void UnLockGiftCard()
    {
        if (!button.gameObject.activeInHierarchy)
            button.gameObject.SetActive(true);

        button.interactable = true;
    }

    public void MarkAsClaimed()
    {
        button.interactable = false;
        claimOverlay.SetActive(true);
    }
}
