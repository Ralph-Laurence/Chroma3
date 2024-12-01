using System;
using System.Collections;
using System.Drawing;
using TMPro;
using UnityEngine;

public class SlotMachineGameController : MonoBehaviour
{
    [Space(10)]
    [Header("Player Banks")]
    [SerializeField] private PlayerBankIndicator coinText;
    [SerializeField] private PlayerBankIndicator gemText;

    [Space(10)]
    [Header("Behaviour")]
    [SerializeField] private SlotMachine slotMachine;
    [SerializeField] private AudioClip   bgmSlotMachine;
    [SerializeField] private GameObject  fancySceneLoader;
    [SerializeField] private GameObject outOfOrderBlocker;
    [SerializeField] private TextMeshProUGUI outOfOrderTimer;

    private BackgroundMusic bgm;
    private GameSessionManager gsm;
    private readonly int MaxAllowedSpinsPerDay = 3;
    private int spinsLeft;

    private const int MaxAllowedMoney = 9000000;
    private int currentPlayerCoins;
    private int currentPlayerGems;

    void Awake()
    {
        gsm = GameSessionManager.Instance;
        bgm = BackgroundMusic.Instance;

        if (bgm != null)
        {
            bgm.SetClip(bgmSlotMachine);
            bgm.Play();
        }

        spinsLeft = gsm.UserSessionData.SlotMachineSpinsLeft;

        currentPlayerCoins = gsm.UserSessionData.TotalCoins;
        currentPlayerGems  = gsm.UserSessionData.TotalGems;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //coinText.text = gsm.UserSessionData.TotalCoins.ToString();
        //gemText.text = gsm.UserSessionData.TotalGems.ToString();

        coinText.RenderValue(gsm.UserSessionData.TotalCoins);
        gemText.RenderValue(gsm.UserSessionData.TotalGems);

        StartCoroutine(IECheckIfAllowedToSpinOnLoad(gsm.UserSessionData));
    }
 
    public void Ev_ExitToMenu()
    {
        Instantiate(fancySceneLoader).TryGetComponent(out FancySceneLoader loader);
        loader.LoadScene(Constants.Scenes.MainMenu);
    }

    public int GetPlayerCurrentCoins()       => gsm.UserSessionData.TotalCoins;
    public int GetPlayerCurrentGems()        => gsm.UserSessionData.TotalGems;

    public void HandleSpinBegan(SlotMachineBetTypes betType)
    {
        var userData  = gsm.UserSessionData;
        var betAmount = betType == SlotMachineBetTypes.Coin
                      ? SlotMachine.BetCoinAmount
                      : SlotMachine.BetGemAmount;

        // Subtract the slot machine bet amount from player's current bank amount
        switch (betType)
        {
            case SlotMachineBetTypes.Coin:
                currentPlayerCoins -= betAmount;
                currentPlayerCoins  = Mathf.Clamp(currentPlayerCoins, 0, MaxAllowedMoney);
                userData.TotalCoins = currentPlayerCoins;
                break;

            case SlotMachineBetTypes.Gem:
                currentPlayerGems -= betAmount;
                currentPlayerGems  = Mathf.Clamp(currentPlayerGems, 0, MaxAllowedMoney);
                userData.TotalGems = currentPlayerGems;
                break;
        }

        spinsLeft--;
        userData.SlotMachineSpinsLeft = spinsLeft;

        // Decrease the player's bank balance and the decrease remaining spins.
        StartCoroutine(UserDataHelper.Instance.SaveUserData(userData, (updatedValue) =>
        {
            gsm.UserSessionData = updatedValue;
        }));
    }

    public void GivePrize(SlotMachineBetTypes betType, SlotMachineItemData prizeWon)
    {
        var userData       = gsm.UserSessionData;
        var prizeAmount    = 0;
        var obtainableCoin = SlotMachineBetTypes.Coin;
        var obtainableGem  = SlotMachineBetTypes.Gem;
        var obtainableBoth = SlotMachineBetTypes.Either;

        // Determine the prize amount based on bet type and "ObtainableBy" property
        switch (betType)
        {
            case SlotMachineBetTypes.Coin:
                if (prizeWon.ObtainableBy == obtainableCoin || prizeWon.ObtainableBy == obtainableBoth) // Either or Coin
                {
                    prizeAmount = prizeWon.PrizeAmountOnBetCoin;
                }
                break;

            case SlotMachineBetTypes.Gem:
                if (prizeWon.ObtainableBy == obtainableGem || prizeWon.ObtainableBy == obtainableBoth) // Either or Gem
                {
                    prizeAmount = prizeWon.PrizeAmountOnBetGem;
                }
                break;
        }

        // Award the prize based on the prize type
        switch (prizeWon.PrizeType)
        {
            case SlotMachineItemPrizeTypes.CoinIncrease:
                GiveGoldCoins(prizeAmount, userData);
                break;
            case SlotMachineItemPrizeTypes.GemIncrease:
                GiveGems(prizeAmount, userData);
                break;
            case SlotMachineItemPrizeTypes.PowerupIncrease:
                GivePowerups(userData, prizeWon, prizeAmount);
                break;
        }

        // Save user data
        StartCoroutine(UserDataHelper.Instance.SaveUserData(userData, (updatedValue) =>
        {
            gsm.UserSessionData = updatedValue;
        }));
    }


    private void AnimatePlayerBank(CurrencyType currency, int from, int to)
    {
        var prizeType = currency == CurrencyType.Gem
                      ? SlotMachineItemPrizeTypes.GemIncrease
                      : SlotMachineItemPrizeTypes.CoinIncrease;

        // If we won a prize such as Coin or Gems, update the currently displayed amount in UI
        if (prizeType == SlotMachineItemPrizeTypes.CoinIncrease)
        {
            PlayerBankAnimationNotifier.NotifyObserver(new PlayerBankAnimationParams
            {
                AnimationType   = PlayerBankAnimationTypes.Increase,
                Currency        = CurrencyType.Coin,
                CurrentValue    = from,
                Amount          = to
            });
        }

        else if (prizeType == SlotMachineItemPrizeTypes.GemIncrease)
        {
            PlayerBankAnimationNotifier.NotifyObserver(new PlayerBankAnimationParams
            {
                AnimationType   = PlayerBankAnimationTypes.Increase,
                Currency        = CurrencyType.Gem,
                CurrentValue    = from,
                Amount          = to
            });
        }
    }

    private void GiveGoldCoins(int amount, UserData userData)
    {
        AnimatePlayerBank(CurrencyType.Coin, currentPlayerCoins, amount);
        currentPlayerCoins  += amount;
        userData.TotalCoins = currentPlayerCoins;
    }

    private void GiveGems(int amount, UserData userData)
    {
        AnimatePlayerBank(CurrencyType.Gem, currentPlayerGems, amount);
        currentPlayerGems  += amount;
        userData.TotalGems = currentPlayerGems;
    }

    private void GivePowerups(UserData userData, SlotMachineItemData prizeData, int powerupAmount)
    {
        var powerupOwned = false;

        // Check if the powerup was already owned or not
        foreach (var item in userData.Inventory.OwnedPowerups)
        {
            if (item.PowerupID == prizeData.ItemID)
            {
                powerupOwned = true;
                break;
            }
        }

        // If the powerup wasnt owned yet, own it
        if (!powerupOwned)
        {
            userData.Inventory.OwnedPowerups.Add(new PowerupInventory
            {
                PowerupID = prizeData.ItemID,
                CurrentAmount = powerupAmount
            });
        }
    }

    public void EndSlotMachineSession()
    {
        var userData = gsm.UserSessionData;

        // Reset remaining spins
        userData.SlotMachineSpinsLeft = MaxAllowedSpinsPerDay;

        // Set the next allowed spin to tomorrow
        userData.NextAllowedSpinTime = DateTime.Now.AddHours(24).ToString("o");

        gsm.UserSessionData = userData;

        StartCoroutine(UserDataHelper.Instance.SaveUserData(userData));
    }

    public bool CanMakeSpins()
    {
        var nextSpinTime = DateTime.Parse(gsm.UserSessionData.NextAllowedSpinTime);

        return (spinsLeft > 0) && (DateTime.Now >= nextSpinTime);
    }

    public int GetCurrentCoinsInBank() => currentPlayerCoins;
    public int GetCurrentGemsInBank() => currentPlayerGems;

    private IEnumerator IECheckIfAllowedToSpinOnLoad(UserData userData)
    {
        var nextAllowedTime = DateTime.Parse(userData.NextAllowedSpinTime);
        var countdownTick   = new WaitForSeconds(1.0F);

        // Yes, we are allowed
        if (DateTime.Now > nextAllowedTime)
        {
            outOfOrderBlocker.SetActive(false);
            yield break;
        }

        // No, we arent.
        outOfOrderBlocker.SetActive(true);

        while (true)
        {
            TimeSpan timeRemaining = nextAllowedTime - DateTime.Now;

            if (timeRemaining <= TimeSpan.Zero)
            {
                // Hide the "out of order" overlay
                outOfOrderBlocker.SetActive(false);

                yield break; // Exit the coroutine
            }

            // Update the UI
            outOfOrderTimer.text = FormatTime(timeRemaining);
            
            // Wait for a second before updating again
            yield return countdownTick;
        }
    }

    private string FormatTime(TimeSpan time)
    {
        return string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds);
    }
}
