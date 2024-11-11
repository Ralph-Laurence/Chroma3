using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GamePlayTutorialStep5 : MonoBehaviour
{
    [SerializeField] private PowerupShopController  shopController;
    [SerializeField] private InventoryController    inventoryController;
    [SerializeField] private InventoryHotBar        hotbar;
    
    [Space(10)]
    [SerializeField] private ScrollRect             shopScrollRect;
    [FormerlySerializedAs("scrollRectContent")]
    [SerializeField] private RectTransform          shopScrollRectContent;
    [SerializeField] private ScrollRect             inventoryScrollRect;
    [SerializeField] private RectTransform          inventoryScrollRectContent;

    [Space(10)]
    [SerializeField] private RectTransform          purchasePowerupSoftClone;
    [SerializeField] private RectTransform          inventoryPowerupSoftClone;
    [SerializeField] private TutorialDriver         tutorialDriver;

    [SerializeField] private int ideaItemID = 100;
    [SerializeField] private int tutorialContentIndexInformInventory;

    private TutorialUtilities tutorialUtils;
    private GameSessionManager gsm;

    private void Awake()
    {
        gsm = GameSessionManager.Instance;
        tutorialUtils = TutorialUtilities.Instance;
    }
    //
    //
    //
    #region EVENT_OBSERVERS
    //
    //
    //
    private void OnEnable()
    {
        ShopMenuPageCommonNotifier.BindObserver(ObserveShopMenuEvents);
        PowerupShopItemClickNotifier.BindObserver(ObservePowerupItemClicked);
        PowerupPurchaseNotifier.BindEvent(ObservePowerupPurchase);
        //TutorialEventNotifier.BindObserver(ObserveTutorialEventObservers);
        InventoryHotbarNotifier.BindObserver(ObserveInventoryHotbar);

        DialogEventNotifier.BindObserver(ObservePurchaseDialogResult);
        //BlockSkinItemPurchaseNotifier.BindEvent(ObserveBlockSkinPurchase);
    }

    private void OnDisable()
    {
        ShopMenuPageCommonNotifier.UnbindObserver(ObserveShopMenuEvents);
        PowerupShopItemClickNotifier.UnbindObserver(ObservePowerupItemClicked);
        PowerupPurchaseNotifier.UnbindEvent(ObservePowerupPurchase);
        // TutorialEventNotifier.UnbindObserver(ObserveTutorialEventObservers);

        InventoryHotbarNotifier.UnbindObserver(ObserveInventoryHotbar);
        DialogEventNotifier.UnbindObserver(ObservePurchaseDialogResult);
        //BlockSkinItemPurchaseNotifier.UnbindEvent(ObserveBlockSkinPurchase);
    }
    private void ObserveInventoryHotbar(InventoryHotbarEventNames eventName)
    {
        if (eventName == InventoryHotbarEventNames.ItemEnqueued)
        {
            // Proceed to the next step
            TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);
        }
    }
    private void ObservePowerupPurchase(PowerupShopItemCard sender)
    {
        // After a powerup item has been purchased, we save the tutorial progress.
        var userData = gsm.UserSessionData;
        userData.CurrentTutorialStep = TutorialSteps.STEP7_EQUIP_INVENTORY;

        StartCoroutine(UserDataHelper.Instance.SaveUserData(userData));
    }

    private void ObservePowerupItemClicked(PowerupShopItemCard sender)
    {
        // Proceed to the next step
        TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);
    }

    private void ObserveShopMenuEvents(ShopMenuPageEventNames eventName)
    {
        var skinsSectionShown       = eventName.Equals(ShopMenuPageEventNames.SkinsPageFullyShown);
        var powerupsSectionShown    = eventName.Equals(ShopMenuPageEventNames.PowerupsPageFullyShown);
        var inventorySectionShown   = eventName.Equals(ShopMenuPageEventNames.InventoryPageFullyShown);

        if (inventorySectionShown)
        {
            tutorialDriver.JumpToContentIndex(tutorialContentIndexInformInventory);
            tutorialDriver.ShowTutorialContent();
        }
        
        else if (skinsSectionShown)
            TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);

        else if (powerupsSectionShown)
            FindPowerupItem();
    }

    private void ObservePurchaseDialogResult(DialogEventNames eventName)
    {
        Debug.Log("Dialog shown");

        if (eventName == DialogEventNames.BuyPowerupsDialogResultShown ||
            eventName == DialogEventNames.BuyPowerupsDialogResultHidden)
        {
            // Proceed to the next step
            TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);
        }
    }
    //
    //
    //
    #endregion EVENT_OBSERVERS
    //
    // For Powerup Shop
    //
    public void FindPowerupItem()
    {
        var items = shopController.PowerupItemCards;

        if (items == null || items?.Count <= 0)
            return;

        PowerupShopItemCard origItemCard = default;

        // Find the powerup item with matching id
        for (var i = 0; i < items.Count; i++)
        {
            var card = items[i];

            // Grab a reference to the target card
            if (card.GetItemData().Id == ideaItemID)
            {
                origItemCard = card;
                break;
            }
        }

        // Position the soft clone into the original's position
        if (origItemCard == null)
        {
            Debug.LogWarning("Original item card is not found!");
            return;
        }

        origItemCard.TryGetComponent(out RectTransform scrollTarget);

        StartCoroutine(IEMatchCardPositions(shopScrollRect, shopScrollRectContent, scrollTarget, () => {
            HandlePowerupSoftClone(origItemCard);
        }));
    }
    private void HandlePowerupSoftClone(PowerupShopItemCard origItemCard)
    {
        purchasePowerupSoftClone.gameObject.SetActive(true);

        origItemCard.TryGetComponent(out RectTransform original);
        tutorialUtils.MatchCloneToOriginalPosition(purchasePowerupSoftClone, original);

        // Bind a custom click event to trigger the original item's click event
        if (purchasePowerupSoftClone.TryGetComponent(out Button btn))
        {
            btn.onClick.AddListener(() =>
            {
                origItemCard.InvokeClick();
                purchasePowerupSoftClone.gameObject.SetActive(false);
            });
        }

        // Move to the next tutorial step
        TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);
    }
    //
    // For Powerup Inventory <Attach in Inspector>
    //
    public void FindPowerupInventoryItem()
    {
        var items = inventoryController.InventoryItemCards;

        if (items == null || items?.Count <= 0)
            return;

        InventoryListItem origItemCard = default;

        // Find the item with matching id
        for (var i = 0; i < items.Count; i++)
        {
            var card = items[i];

            // Grab a reference to the target card
            if (card.GetItemData().ID == ideaItemID)
            {
                origItemCard = card;
                break;
            }
        }

        // Position the soft clone into the original's position
        if (origItemCard == null)
        {
            Debug.LogWarning("Original item card is not found!");
            return;
        }

        origItemCard.TryGetComponent(out RectTransform origRect);

        tutorialUtils.MatchCloneToOriginalPosition(inventoryPowerupSoftClone, origRect);
        HandleInventoryItemSoftClone(origItemCard);
        //StartCoroutine(IEMatchCardPositions(inventoryScrollRect, inventoryScrollRectContent, origRect, () => {
        //    HandleInventoryItemSoftClone(origItemCard);
        //}));
    }
    private void HandleInventoryItemSoftClone(InventoryListItem origItemCard)
    {
        inventoryPowerupSoftClone.gameObject.SetActive(true);

        origItemCard.TryGetComponent(out RectTransform original);
        tutorialUtils.MatchCloneToOriginalPosition(inventoryPowerupSoftClone, original);

        // Bind a custom click event to trigger the original item's click event
        if (inventoryPowerupSoftClone.TryGetComponent(out Button btn))
        {
            btn.onClick.AddListener(() =>
            {
                origItemCard.InvokeClick();
                inventoryPowerupSoftClone.gameObject.SetActive(false);
            });
        }

        // Move to the next tutorial step
        //TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);
    }
    //
    // For Both
    //

    private IEnumerator IEMatchCardPositions(ScrollRect scrollRect, RectTransform scrollRectContent, RectTransform target, Action onScrolled)
    {
        yield return new WaitForEndOfFrame();

        // Calculate the position of the target item within the ScrollRect
        float targetPosition = (float)target.GetSiblingIndex() / (scrollRectContent.childCount - 1);

        // Adjust the position to center the target item
        float centeredPosition = Mathf.Clamp01(targetPosition - 0.5f / (scrollRectContent.childCount - 1));

        // Set the ScrollRect's vertical normalized position
        scrollRect.verticalNormalizedPosition = 1 - centeredPosition;

        // When scroll is finished
        onScrolled?.Invoke();
    }

    public void ExitToMenu()
    {
        StartCoroutine(IESaveData());
    }

    private IEnumerator IESaveData()
    {
        var equippedPowerupIds = new List<int>();
        var userData           = gsm.UserSessionData;
        var equippedPowerups   = hotbar.GetItemQueue();

        if (equippedPowerups?.Count > 0)
        {
            foreach (var kvp in equippedPowerups)
            {
                equippedPowerupIds.Add(kvp.Value.ID);
                yield return null;
            }
        }
        
        userData.Inventory.EquippedPowerupIds = equippedPowerupIds;
        userData.CurrentTutorialStep = TutorialSteps.STEP8_USE_POWERUP_IN_GAME;

        // Write the changes to file
        yield return StartCoroutine(UserDataHelper.Instance.SaveUserData(userData, (d) =>
        {
            SceneManager.LoadScene(Constants.Scenes.MainMenu);
        }));
    }
}
