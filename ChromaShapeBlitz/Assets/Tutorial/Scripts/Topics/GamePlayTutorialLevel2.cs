using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayTutorialLevel2 : MonoBehaviour
{
    [SerializeField] private TutorialPointerHand pointerHand;

    [Space(10)]
    [SerializeField] private SkinShopController skinsShop;
    [SerializeField] private int orangeSkinID;
    [SerializeField] private RectTransform orangeTrashcanShopItemCard_softClone;

    [SerializeField] private ColorSelect colorSelectOrig;
    private ColorSelect colorSelectSoftClone;
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
        ColorSelectEventNotifier.BindObserver(ObserveColorFilterEvents);
        TutorialEventNotifier.BindObserver(ObserveTutorialEventObservers);
        BlockSkinShopItemClickNotifier.BindObserver(ObserveBlockSkinItemClicked);
        DialogEventNotifier.BindObserver(ObservePurchaseDialogResult);
        BlockSkinItemPurchaseNotifier.BindEvent(ObserveBlockSkinPurchase);
    }

    private void OnDisable()
    {
        ShopMenuPageCommonNotifier.UnbindObserver(ObserveShopMenuEvents);
        ColorSelectEventNotifier.UnbindObserver(ObserveColorFilterEvents);
        TutorialEventNotifier.UnbindObserver(ObserveTutorialEventObservers);
        BlockSkinShopItemClickNotifier.UnbindObserver(ObserveBlockSkinItemClicked);
        DialogEventNotifier.UnbindObserver(ObservePurchaseDialogResult);
        BlockSkinItemPurchaseNotifier.UnbindEvent(ObserveBlockSkinPurchase);
    }

    private void ObserveBlockSkinPurchase(SkinShopItemCard sender)
    {
        // After a block skin has been purchased, we save the tutorial progress.
        var userData = gsm.UserSessionData;
        userData.CurrentTutorialStep = TutorialSteps.STEP3_BLOCK_SKIN_USAGE;

        StartCoroutine(UserDataHelper.Instance.SaveUserData(userData));
    }

    private void ObserveShopMenuEvents(ShopMenuPageEventNames eventName)
    {
        if (eventName.Equals(ShopMenuPageEventNames.SkinsPageFullyShown))
        {
            TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);
        }
    }

    private void ObserveColorFilterEvents(ColorSelectEventNames eventName, object data)
    {
        pointerHand.Hide();

        switch (eventName)
        {
            case ColorSelectEventNames.OptionsShown:
                HandleColorFilterShown(data);
                break;

            case ColorSelectEventNames.ColorSelected:

                // Disable the temporary (soft cloned) color select
                if (colorSelectSoftClone != null)
                    colorSelectSoftClone.gameObject.SetActive(false);

                // Update the original color select's current value
                // without triggering the original's value changed event.
                if (data != null)
                {
                    var selectData = (ColorSelectValueCallback)data;
                    colorSelectOrig.SetValueSilent( selectData.ColorIndex );
                }

                // Proceed to the next step
                TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);
                break;
        }
    }

    private void ObserveTutorialEventObservers(string eventName, object data)
    {
        // We will use this observer to watch for notifications when an element
        // has been cloned and cache it as long as it is of type "Color Select"
        if (eventName.Equals(TutorialEventNames.ElementCloned))
        {
            if (data == null)
                return;

            var cloneObj = (GameObject) data;

            // We only need to cache it once.
            if (cloneObj.TryGetComponent(out ColorSelect colorSelect) && colorSelectSoftClone == null)
            {
                colorSelectSoftClone = colorSelect;
            }
        }
    }

    private void ObserveBlockSkinItemClicked(SkinShopItemCard sender)
    {
        // Proceed to the next step
        TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);
    }

    private void ObservePurchaseDialogResult(DialogEventNames eventName)
    {
        if (eventName == DialogEventNames.BuySkinsDialogResultShown || 
            eventName == DialogEventNames.BuySkinsDialogResultHidden )
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
    //
    //
    private void HandleColorFilterShown(object data)
    {
        if (data == null)
            return;

        var colorSelectUIData = (ColorSelectUIData)data;

        // Extract the data thrown by the color filter
        var colorToggles = colorSelectUIData.ColorOptionToggles;

        // Disable clicking outside the color filter's dropdowns
        if (colorSelectUIData.UIBlocker != null && colorSelectUIData.UIBlocker.TryGetComponent(out Button button))
        {
            button.interactable = false;
        }

        // Find the UI Toggle Dropdown Item that holds the target color
        RectTransform target = default;

        foreach (KeyValuePair<string, Toggle> kvp in colorToggles)
        {
            var toggle = kvp.Value;
            var color = kvp.Key;

            if (color.Equals(nameof(ColorSwatches.Orange)))
            {
                toggle.interactable = true;
                toggle.TryGetComponent(out target);
            }
            else
            {
                toggle.interactable = false;
            }
        }
        //
        // Add a blinking highlighter into the target color filter item
        //
        if (target != null)
        {
            var highlighter = target.gameObject.AddComponent<ColorSelectItemHighlighter>();
            highlighter.HighlightWithColor(Constants.ColorSwatches.ORANGE);
        }
    }

    /// <summary>
    /// REFERENCED IN SCENE
    /// </summary>
    public void CloneTrashcanShopItem()
    {
        SkinShopItemCard origItemCard = default;

        // Find the original trashcan shop item
        for (var i = 0; i < skinsShop.SkinItems.Count; i++)
        {
            var card = skinsShop.SkinItems[i];

            // if (card.GetItemData().ID == targetItemID)
            if (card.GetItemData().ID == orangeSkinID)
            {
                // Grab a reference to it
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
        
        orangeTrashcanShopItemCard_softClone.gameObject.SetActive(true);

        origItemCard.TryGetComponent(out RectTransform original);
        tutorialUtils.MatchCloneToOriginalPosition(orangeTrashcanShopItemCard_softClone, original);

        // Bind a custom click event to trigger the original item's click event
        if (orangeTrashcanShopItemCard_softClone.TryGetComponent(out Button btn))
        {
            btn.onClick.AddListener(() =>
            {
                origItemCard.InvokeClick();
                orangeTrashcanShopItemCard_softClone.gameObject.SetActive(false);
            });
        }
    }

    /// <summary>
    /// Instead of "going back", we need to reload the Main Menu instead.
    /// This will effectively switch to next tutorial level.
    /// </summary>
    public void GoBackToMainMenu() => SceneManager.LoadScene(Constants.Scenes.MainMenu);
}