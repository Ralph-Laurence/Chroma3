using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayTutorialLevel4 : MonoBehaviour
{
    [SerializeField] private BackgroundShopController shopController;
    [SerializeField] private ScrollRect shopScrollRect;
    [SerializeField] private RectTransform scrollRectContent;

    [SerializeField] private int fishPondBackgroundID = 1;
    [SerializeField] private RectTransform purchaseBackgroundSoftClone;

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
    #region EVENT_OBSERVER
    //
    //
    void OnEnable()
    {
        ShopMenuPageCommonNotifier.BindObserver(ObserveShopMenuEvents);
        DialogEventNotifier.BindObserver(ObservePurchaseDialogResult);
        BackgroundPurchaseNotifier.BindObserver(ObservePurchase);
        BackgroundShopItemClickNotifier.BindObserver(ObserveBackgroundItemClicked);
    }

    void OnDisable()
    {
        ShopMenuPageCommonNotifier.UnbindObserver(ObserveShopMenuEvents);
        DialogEventNotifier.UnbindObserver(ObservePurchaseDialogResult);
        BackgroundPurchaseNotifier.UnbindObserver(ObservePurchase);
        BackgroundShopItemClickNotifier.UnbindObserver(ObserveBackgroundItemClicked);
    }
    
    /// <summary>
    /// Usually, the skins page is the landing page... it gets shown first.
    /// We add a second event case to handle the backgrounds page to capture
    /// its state if it has been fully shown or not
    /// </summary>
    /// <param name="eventName">Event Key that triggered the event</param>
    private void ObserveShopMenuEvents(ShopMenuPageEventNames eventName)
    {
        switch (eventName)
        {
            case ShopMenuPageEventNames.SkinsPageFullyShown:
                TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);
                break;

            case ShopMenuPageEventNames.BackgroundsPageFullyShown:
                ScrollToTargetBackgroundItem();
                break;
        }
    }

    private void ObservePurchaseDialogResult(DialogEventNames eventName)
    {
        if (eventName == DialogEventNames.BuyBackgroundDialogResultShown || 
            eventName == DialogEventNames.BuyBackgroundDialogResultHidden )
        {
            // Proceed to the next step
            TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);
        }
    }

    private void ObservePurchase(BackgroundShopItemCard sender)
    {
        // After a background skin has been purchased, we save the tutorial progress.
        var userData = gsm.UserSessionData;
        userData.CurrentTutorialStep = TutorialSteps.STEP5_BACKGROUND_USAGE;

        StartCoroutine(UserDataHelper.Instance.SaveUserData(userData));
    }

    
    private void ObserveBackgroundItemClicked(BackgroundShopItemCard sender)
    {
        // Proceed to the next step
        TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);
    }
    //
    //
    //
    #endregion EVENT_OBSERVER
    /// <summary>
    /// Scroll vertically until the target background is at the middle of screen
    /// </summary>
    public void ScrollToTargetBackgroundItem()
    {
        var items = shopController.BackgroundItemCards;

        if (items == null || items?.Count <= 0)
            return;

        BackgroundShopItemCard origItemCard = default;

        // Find the background shop item with matching id
        for (var i = 0; i < items.Count; i++)
        {
            var card = items[i];

            // Grab a reference to the target card
            if (card.GetItemData().ID == fishPondBackgroundID)
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
        StartCoroutine(IEScrollToTarget(scrollTarget, () => {
            HandleSoftCloneAfterScrolled(origItemCard);
        }));
    }

    private IEnumerator IEScrollToTarget(RectTransform target, Action onScrolled)
    {
        yield return new WaitForEndOfFrame();

        // Calculate the position of the target item within the ScrollRect
        float targetPosition = (float)target.GetSiblingIndex() / (scrollRectContent.childCount - 1);

        // Adjust the position to center the target item
        float centeredPosition = Mathf.Clamp01(targetPosition - 0.5f / (scrollRectContent.childCount - 1));

        // Set the ScrollRect's vertical normalized position
        shopScrollRect.verticalNormalizedPosition = 1 - centeredPosition;
        
        // When scroll is finished
        onScrolled?.Invoke();
    }

    private void HandleSoftCloneAfterScrolled(BackgroundShopItemCard origItemCard)
    {
        purchaseBackgroundSoftClone.gameObject.SetActive(true);

        origItemCard.TryGetComponent(out RectTransform original);
        tutorialUtils.MatchCloneToOriginalPosition(purchaseBackgroundSoftClone, original);

        // Bind a custom click event to trigger the original item's click event
        if (purchaseBackgroundSoftClone.TryGetComponent(out Button btn))
        {
            btn.onClick.AddListener(() =>
            {
                origItemCard.InvokeClick();
                purchaseBackgroundSoftClone.gameObject.SetActive(false);
            });
        }

        // Move to the next tutorial step
        TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);
    }
    
    /// <summary>
    /// Instead of "going back", we need to reload the Main Menu instead.
    /// This will effectively switch to next tutorial level.
    /// </summary>
    public void GoBackToMainMenu() => SceneManager.LoadScene(Constants.Scenes.MainMenu);
}
