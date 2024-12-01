using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyGiftController : MonoBehaviour
{
    [Space(10)]
    [SerializeField] private PlayerBankIndicator coinsIndicator;
    [SerializeField] private PlayerBankIndicator gemsIndicator;
    
    [Space(10)]
    [SerializeField] private ScrollRect scrollViewRect;
    [SerializeField] private List<PowerupsAsset> powerupGifts;
    [SerializeField] private List<DailyGiftItemCard> giftItemCards;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private RectTransform claimTimer;
    [SerializeField] private TextMeshProUGUI claimTimerText;

    //...................................................
    //  CLAIM BASIC GIFTS
    //...................................................
    [Space(10)]
    [SerializeField] private GameObject claimSuccessPopup;
    [SerializeField] private Image claimSuccessIcon;
    [SerializeField] private TextMeshProUGUI claimedSuccessAmount;
    [SerializeField] private TextMeshProUGUI claimedItemName;

    //...................................................
    //  CLAIM SPECIAL MEGA GIFTS
    //...................................................
    [Space(10)]
    [SerializeField] private GameObject claimMegapackPopup;
    [SerializeField] private Image claimMegapackPowerupIcon;
    [SerializeField] private TextMeshProUGUI claimMegapackItemName;
    [SerializeField] private int megapackCoins = 200;
    [SerializeField] private int megpackGems = 20;

    [Space(5)]
    [SerializeField] private AudioClip sfxClaimPowerup;
    [SerializeField] private AudioClip sfxClaimMegapack;

    [Space(10)]
    [SerializeField] private GameObject FancyLoader;

    private GameSessionManager gsm;
    private RectTransform claimSuccessPopupRect;
    private RectTransform claimMegapackPopupRect;
    private SoundEffects sfx;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        gsm = GameSessionManager.Instance;
        sfx = SoundEffects.Instance;

        claimSuccessPopup.TryGetComponent(out claimSuccessPopupRect);
        claimMegapackPopup.TryGetComponent(out claimMegapackPopupRect);
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        StartCoroutine(Initialize());
    }

    // This function is called when the object becomes enabled and active.
    void OnEnable()
    {
        GiftItemCardClickNotifier.BindObserver(HandleGiftCardClicked);
    }

    // This function is called when the behaviour becomes disabled or inactive.
    void OnDisable()
    {
        GiftItemCardClickNotifier.UnbindObserver(HandleGiftCardClicked);
    }

    // This function is called whenever the observer detects a gift card click.
    private void HandleGiftCardClicked(DailyGiftItemCard sender)
    {
        // Claim the gift(s)
        sender.MarkAsClaimed();

        switch (sender.GiftType)
        {
            case DailyGiftTypes.RandomPowerup:
                ClaimPowerup(sender);
                break;

            case DailyGiftTypes.MegaPack:
                ClaimMegapack(sender);
                break;

            default:
                ClaimTreasure(sender);
                break;
        }
    }
    //
    // Show the "You Got" popup screen
    //
    private void ShowYouGot(string itemName, Sprite itemIcon, int amount)
    {
        claimedItemName.text      = itemName;
        claimSuccessIcon.sprite   = itemIcon;
        claimedSuccessAmount.text = $"\u00D7{amount}";

        claimSuccessPopupRect.localScale = Vector3.forward * 0.1F;

        claimSuccessPopup.SetActive(true);

        LeanTween.scale(claimSuccessPopup, Vector3.one, 0.25F)
                 .setDelay(0.3F);
    }
    //
    // Show the "You Got" popup screen for Mega Pack special gift
    //
    private void ShowYouGotMegapack(string powerupName, Sprite powerupIcon)
    {
        claimMegapackItemName.text        = powerupName;
        claimMegapackPowerupIcon.sprite   = powerupIcon;
        claimMegapackPopupRect.localScale = Vector3.forward * 0.1F;

        claimMegapackPopup.SetActive(true);

        LeanTween.scale(claimMegapackPopup, Vector3.one, 0.25F)
                 .setDelay(0.3F);
    }
    // This function handles the claiming of coins or gems
    private void ClaimTreasure(DailyGiftItemCard sender)
    {
        // Read the user data from the session
        var userData = gsm.UserSessionData;
        //
        //.........................................
        // Prepare the animation parameters
        //.........................................
        //
        var animationParams = new PlayerBankAnimationParams
        {
            Amount = sender.GiftAmount,
            AnimationType = PlayerBankAnimationTypes.Increase
        };

        // Decide which bank amount to increase.
        // We do this along with preparing the animation parameters.
        switch (sender.GiftType)
        {
            case DailyGiftTypes.Coin:
            
                animationParams.Currency       = CurrencyType.Coin;
                animationParams.CurrentValue   = userData.TotalCoins;
                userData.TotalCoins            += sender.GiftAmount;
                break;

            case DailyGiftTypes.Gem:

                animationParams.Currency       = CurrencyType.Gem;
                animationParams.CurrentValue   = userData.TotalGems;
                userData.TotalGems             += sender.GiftAmount;
                break;
        };
        //
        //.........................................
        // Save the changes into the player's data
        //.........................................
        //
        userData.DailyGiftClaimHistory.Add(sender.DayNumber);
        
        SaveUserData(userData, onSuccess: () => {
            
            PlayerBankAnimationNotifier.NotifyObserver(animationParams);
            UpdateBannerText(giftAvailable: false);

            // Show the timer on the next gift card to claim,
            // except if the current card's day is the 12th day
            var nextToClaim = sender.DayNumber + 1;

            if (nextToClaim < 12)
            {
                giftItemCards.ForEach(card =>
                {
                    if (card.DayNumber == nextToClaim)
                    {
                        StartCoroutine(IECountdownOnNextClaimableGift(userData, card));
                    }
                });
            }

            ShowYouGot(sender.GiftName, sender.CardIcon.sprite, sender.GiftAmount);
        });
    }

    // This function handles the claiming of random powerups
    private void ClaimPowerup(DailyGiftItemCard sender)
    {
        // Read the user data from the session
        var userData = gsm.UserSessionData;

        // Randomize which powerup to claim
        var powerupItem = GiveRandomPowerup(userData, 1);

        userData.DailyGiftClaimHistory.Add(sender.DayNumber);

        // Save the changes into disk permanently
        SaveUserData(userData, onSuccess: () => {
            sfx.PlayOnce(sfxClaimPowerup);

            var itemName = $"\"{powerupItem.Name}\" Powerup";

            // Show the "You got" popup screen
            ShowYouGot(itemName, powerupItem.PreviewImage, sender.GiftAmount);
        });
    }

    private void ClaimMegapack(DailyGiftItemCard sender)
    {
        // Read the user data from the session
        var userData = gsm.UserSessionData;

        // Randomize which powerup to claim
        var powerupItem = GiveRandomPowerup(userData, 3);

        // Give gems and coins
        userData.TotalCoins += megapackCoins;
        userData.TotalGems  += megpackGems;

        userData.DailyGiftClaimHistory.Add(sender.DayNumber);

        // Save the changes into disk permanently
        SaveUserData(userData, onSuccess: () => {
            
            sfx.PlayOnce(sfxClaimMegapack);

            // Show the "You got" popup screen
            ShowYouGotMegapack(powerupItem.Name, powerupItem.PreviewImage);
        });

    }

    private PowerupsAsset GiveRandomPowerup(UserData userData, int amount)
    {
        // Randomize which powerup to claim
        var powerupIndex = UnityEngine.Random.Range(0, powerupGifts.Count);
        var powerupItem  = powerupGifts[powerupIndex];

        // Save the powerup into user's data
        var ownedPowerups = userData.Inventory.OwnedPowerups;

        // Check if the powerup isnt owned yet, then own it
        var isOwned = ownedPowerups.Any(powerup => powerup.PowerupID == powerupItem.Id);

        // If we already own a powerup, just update its current amount.
        // In this case, we will update the amount stored into the disk
        if (isOwned)
        {
            for (var i = 0; i < ownedPowerups.Count; i++)
            {
                if (ownedPowerups[i].PowerupID == powerupItem.Id)
                {
                    var ownedPowerup = ownedPowerups[i];

                    ownedPowerup.CurrentAmount += amount;
                    ownedPowerups[i] = ownedPowerup;
                    
                    // Exit the loop once the powerup is found and updated.
                    break;
                }
            }
        }
        // Otherwise add it
        else
        {
            ownedPowerups.Add(new PowerupInventory
            {
                PowerupID = powerupItem.Id,
                CurrentAmount = amount
            });
        }

        return powerupItem;
    }

    private void SaveUserData(UserData userData, Action onSuccess = null)
    {
        userData.DailyGiftDayNumber++;
        userData.NextDailyGiftTime = DateTime.Now.AddHours(24).ToString("o");

        // Reset the day back to day 1 after claiming 12th day gift
        // then clear the claim history
        if (userData.DailyGiftDayNumber > 12)
        {
            userData.DailyGiftDayNumber = 1;
            userData.DailyGiftClaimHistory.Clear();
        }

        // Write the changes into the disk
        StartCoroutine(UserDataHelper.Instance.SaveUserData(userData, (u) => {
            onSuccess?.Invoke();
        }));
    }

    private IEnumerator Initialize()
    {
        yield return StartCoroutine(UserDataHelper.Instance.LoadUserData((userData) =>
        {
            gsm.UserSessionData = userData;
        }));

        // Load the current coins and gems
        coinsIndicator.RenderValue(gsm.UserSessionData.TotalCoins);
        gemsIndicator.RenderValue(gsm.UserSessionData.TotalGems);

        InitializeGiftCards();
    }

    private void InitializeGiftCards()
    {
        var userData       = gsm.UserSessionData;
        var claimHistory   = userData.DailyGiftClaimHistory;
        var currentGiftDay = userData.DailyGiftDayNumber;
        
        giftItemCards.ForEach(giftCard =>
        {
            // Find the gift card that has its "Day Number" equal to the
            // next day number saved in user data. Remeber that
            // the next day number is always incremented by 1, after claiming.
            if (IsGiftAvailable && giftCard.DayNumber == currentGiftDay)
            {
                giftCard.UnLockGiftCard();
            }
            else
            {
                // Tell if a gift card had already been claimed, by looking at the claim history.
                if (claimHistory.Contains(giftCard.DayNumber))
                    giftCard.MarkAsClaimed();

                // Lock the gift cards that are yet to be claimed on their given day.
                else
                    giftCard.LockGiftCard();

                // To Do:
                //
                // Show the timer on the gift card next to the previously claimed.
                // We can only show it when the gift card's day number is not
                // equal or greater than day 12. Also, we will only show the timer
                // when the next claim date has been set to the next 24hrs.
                // The next claim time is initially "minimum time" on first run.
                // We can only set the next 24hrs after claiming any gift.
                // 

                // The default standard ISO date time
                var minDateISO = DateTime.Parse( DateTime.MinValue.ToString("o") );

                // The next claim time saved in user data
                var nextClaimTime = DateTime.Parse(userData.NextDailyGiftTime);

                // If the next claim time is newer than the default initial time,
                // show the countdown timer
                if (nextClaimTime > minDateISO 
                    //&& currentGiftDay > 1
                    && currentGiftDay < 12
                    && giftCard.DayNumber == currentGiftDay
                )
                {
                    StartCoroutine(IECountdownOnNextClaimableGift(userData, giftCard));
                }
            }
        });

        // If the giftcard (to claim) is at the bottom, we scroll down the view.
        // These are usually the upper half gift cards' day numbers (ie 7-12).
        if (currentGiftDay > (giftItemCards.Count / 2))
        {
            scrollViewRect.verticalNormalizedPosition = 0.0F;
        }

        // Update the banner text depending on the availability of gifts
        UpdateBannerText(IsGiftAvailable);
    }

    private void UpdateBannerText(bool giftAvailable)
    {
        // If a gift is available to claim, set the banner text color to green
        if (!giftAvailable)
        {
            headerText.text = "<color=#FFFFFF>Come back tomorrow</color>";
            return;
        }

        // If a gift is NOT available, set the banner text color to white
        headerText.text = "<color=#7DFF00>Claim your gift now!</color>";
    }
    
    public bool IsGiftAvailable
    {
        get
        {
            // We can tell if a gift is available by comparing the current datetime
            // vs the next gift time. A gift is "available" when the current date time
            // is greater than next gift time
            var nextGiftTime = DateTime.Parse(gsm.UserSessionData.NextDailyGiftTime);

            return DateTime.Now >= nextGiftTime;
        }
    }

    public void Ev_ReturnToMainMenu()
    {
        Instantiate(FancyLoader).TryGetComponent(out FancySceneLoader loader);
        loader.LoadScene(Constants.Scenes.MainMenu);
    }

    private IEnumerator IECountdownOnNextClaimableGift(UserData userData, DailyGiftItemCard giftCard)
    {
        // Place the timer countdown inside the card
        AddGiftCardTimer(giftCard);

        var nextDailyGiftTime = DateTime.Parse(userData.NextDailyGiftTime);
        var countdownTick = new WaitForSeconds(1.0F);

        while (true)
        {
            TimeSpan timeRemaining = nextDailyGiftTime - DateTime.Now;

            if (timeRemaining <= TimeSpan.Zero)
            {
                // Unlock the gift
                giftCard.UnLockGiftCard();
                
                // Hide the countdown timer
                claimTimer.SetParent(null);
                claimTimer.gameObject.SetActive(false);

                // Update the banner text
                UpdateBannerText(giftAvailable: true);

                yield break; // Exit the coroutine
            }

            // Update the UI
            claimTimerText.text = FormatTime(timeRemaining);
            
            // Wait for a second before updating again
            yield return countdownTick;
        }
    }

    private void AddGiftCardTimer(DailyGiftItemCard giftCard)
    {
        // Lock the gift card but turn off the button.
        // This is to make sure that the dark background isn't
        // visible but still wont be able to recieve clicks.
        giftCard.LockGiftCard(disableButton: true);

        // Make the claim timer UI a child of the card
        claimTimer.SetParent(giftCard.transform);

        claimTimer.anchorMin  = new Vector2(0.0F ,0.5F);
        claimTimer.anchorMax  = new Vector2(1.0F ,0.5F);
        claimTimer.pivot      = new Vector2(0.5F, 0.5F);
        claimTimer.localScale = Vector3.one;

        // Set the Left position of stretched rect transform
        var offsetMin = claimTimer.offsetMin;
        offsetMin.x = 4.0F;
        
        claimTimer.offsetMin = offsetMin;

        // Set the Left position of stretched rect transform
        var offsetMax = claimTimer.offsetMax;
        offsetMax.x = -3.0F;
        
        claimTimer.offsetMax = offsetMax;

        // Set the Y Pos of the stretched rect transform
        var posY = claimTimer.anchoredPosition;
        posY.y = 0.0F;

        claimTimer.anchoredPosition = posY;

        // Show the timer UI then Run the countdown
        claimTimer.gameObject.SetActive(true);
    }

    private string FormatTime(TimeSpan time)
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
    }
}