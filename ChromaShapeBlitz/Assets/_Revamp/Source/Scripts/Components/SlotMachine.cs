using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SlotMachineBetTypes
{
    Either,
    Coin,
    Gem
}

public enum SlotMachineItemPrizeTypes
{
    None,
    CoinIncrease,
    GemIncrease,
    PowerupIncrease
}

public struct SlotMachinePrizeQueueItemCardData
{
    public string ItemName;
    public int PrizeAmount;
}

[RequireComponent(typeof(SlotMachineAnimation))]
public class SlotMachine : MonoBehaviour
{
    //
    //
    //
    #region FIELDS
    //
    //
    [Space(10)] [Header("Item Frame")]
    [SerializeField] private Image itemFrameResultLeft;
    [SerializeField] private Image itemFrameResultMiddle;
    [SerializeField] private Image itemFrameResultRight;

    [Space(10)]
    [SerializeField] private SlotMachineItemSpriteMapping   itemSpriteMapping;
    [SerializeField] private SlotMachineGameController      controller;
    [SerializeField] private SlotMachinePrizeCard           prizeCardPrefab;
    [SerializeField] private GameObject                     noPrizeCardPrefab;

    [Space(10)]
    [SerializeField] private TextAsset slotmachineItemDataJson;
    [SerializeField] private GameObject prizesResultOverlay;
    [SerializeField] private Transform prizesResultContainer;
    //
    //
    #endregion FIELDS
    //
    //

    // Slot machine items and their rarity weights 
    private Dictionary<string, SlotMachineItemData> slotMachineItems;

    private SlotMachineAnimation slotAnimation;
    private SlotMachineBetTypes selectedBetType;

    private string shuffleResult;
    private List<string> junks;

    [SerializeField] private bool readyForNextSpin;
    public const int BetCoinAmount = 250;
    public const int BetGemAmount = 5;

    private List<SlotMachinePrizeQueueItemCardData> prizeQueue;

    public SlotMachineKnob coinKnob;
    public SlotMachineKnob gemKnob;

    void Awake()
    {
        itemSpriteMapping.Initialize();

        slotMachineItems = new();
        prizeQueue       = new();
        junks            = new() { "None1", "None2", "Random", "Random1", "Shit" };
        var itemsList    = JsonUtility.FromJson<SlotMachineItems>(slotmachineItemDataJson.text);

        itemsList.Items.ForEach(item =>
        {
            slotMachineItems[item.ItemName] = item;
        });

        itemsList.Items.Clear();
        itemsList = default;

        TryGetComponent(out slotAnimation);

        slotAnimation.SpinAnimationStarting += OnSpinAnimationStarting;
        slotAnimation.SpinAnimationEnded    += OnSpinAnimationEnded;
        slotAnimation.OnSpinEndWithResult   += OnSlotMachineSpinResult;
    }

    void Start()
    {
        readyForNextSpin = true;

        // Show the "come again tomorrow" dialog
        if (!controller.CanMakeSpins())
        {
            prizesResultOverlay.SetActive(true);
            LeanTween.scale(prizesResultOverlay, Vector3.one, 0.25F);
            return;
        }
    }

    public void BetWithCoin()
    {
        if (!readyForNextSpin)
            return;

        var currentCoins = controller.GetPlayerCurrentCoins();

        if (currentCoins < BetCoinAmount)
        {
            slotAnimation.AnimateNotEnoughCoins();
            coinKnob.Retract();
            return;
        }

        readyForNextSpin = false;
        selectedBetType = SlotMachineBetTypes.Coin;

        shuffleResult = Shuffle(selectedBetType);
        slotAnimation.Begin(selectedBetType);

        // Decrease the player coins in UI Side.
        // The actual decreasing of coins in the save data
        // is handled via event callbacks
        PlayerBankAnimationNotifier.NotifyObserver(new PlayerBankAnimationParams
        {
            Amount          = BetCoinAmount,
            AnimationType   = PlayerBankAnimationTypes.Decrease,
            Currency        = CurrencyType.Coin,
            CurrentValue    = currentCoins
        });
    }

    public void BetWithGem()
    {
        if (!readyForNextSpin)
            return;

        var currentGems = controller.GetPlayerCurrentGems();

        if (currentGems < BetGemAmount)
        {
            slotAnimation.AnimateNotEnoughGems();
            gemKnob.Retract();
            return;
        }

        readyForNextSpin = false;
        selectedBetType = SlotMachineBetTypes.Gem;

        shuffleResult = Shuffle(selectedBetType);
        slotAnimation.Begin(selectedBetType);

        // Decrease the player Gems in UI Side.
        // The actual decreasing of gems in the save data
        // is handled via event callbacks
        PlayerBankAnimationNotifier.NotifyObserver(new PlayerBankAnimationParams
        {
            Amount          = BetGemAmount,
            AnimationType   = PlayerBankAnimationTypes.Decrease,
            Currency        = CurrencyType.Gem,
            CurrentValue    = currentGems
        });
    }

    //private int IncreaseFrequency(int rarity, float frequency)
    //{
    //    int result = Mathf.RoundToInt(rarity * frequency);
    //    return result;
    //}
    //
    //
    //
    #region EVENTS
    //
    //
    //
    private void OnSpinAnimationStarting()
    {
        ToggleItemFrameResult(false);

        // Just decrease the player's bank, don't apply rewards yet
        controller.HandleSpinBegan(selectedBetType);
    }

    private void OnSpinAnimationEnded()
    {
        Debug.Log(shuffleResult);

        Sprite[] resultSprites = new Sprite[3];

        var hasSpritesSelected = false;
        var isJackpot = GotJackpot(shuffleResult);

        if (isJackpot || shuffleResult.StartsWith("None") || shuffleResult.Equals("Shit"))
        {
            // Assign the same sprite three times for jackpot or 'None' results
            var commonSprite = itemSpriteMapping.Select(shuffleResult);
            resultSprites = new[] { commonSprite, commonSprite, commonSprite };

            hasSpritesSelected = true;
        }

        // Whenever we got a jackpot, we store the prize
        if (isJackpot)
        {
            // Show "Jackpot" in light bulbs and title text
            slotAnimation.AnimateJackpot();

            // Apply the prize
            var prize = slotMachineItems[shuffleResult];

            controller.GivePrize(selectedBetType, prize);

            // We'll display the slot items later on
            prizeQueue.Add(new SlotMachinePrizeQueueItemCardData
            {
                // Prize Amounts vary depending on bet (i.e. gems bet doubles the prize)
                PrizeAmount = selectedBetType == SlotMachineBetTypes.Coin
                            ? prize.PrizeAmountOnBetCoin
                            : prize.PrizeAmountOnBetGem,

                ItemName = shuffleResult
            });
        }
        else
        {
            slotAnimation.AnimateTryAgain();
        }

        if (shuffleResult.StartsWith("Random") || !hasSpritesSelected)
        {
            // Generate three random sprites
            resultSprites = itemSpriteMapping.SelectRandom();
        }

        SetItemFramesSprite(resultSprites);
        ToggleItemFrameResult(true);
    }

    private void OnSlotMachineSpinResult()
    {
        // Show the "come again tomorrow" dialog
        if (!controller.CanMakeSpins())
        {
            controller.EndSlotMachineSession();

            prizesResultOverlay.SetActive(true);

            if (prizeQueue.Count > 0)
            {
                prizeQueue.ForEach(prize =>
                {
                    var prizeCard = Instantiate(prizeCardPrefab, prizesResultContainer);
                    //var cardIcon  = shuffleResult.StartsWith("Random") || shuffleResult.StartsWith("None")
                    //              ? itemSpriteMapping.Select("None1")
                    //              : itemSpriteMapping.Select(prize.ItemName);
                    var cardIcon = itemSpriteMapping.Select(prize.ItemName);

                    if (prizeCard.TryGetComponent(out SlotMachinePrizeCard card))
                    {
                        card.Icon = cardIcon;
                        card.Amount = prize.PrizeAmount;
                    }
                });
            }
            else
            {
                Instantiate(noPrizeCardPrefab, prizesResultContainer);
            }

            LeanTween.scale(prizesResultOverlay, Vector3.one, 0.25F);
            return;
        }

        switch (selectedBetType)
        {
            case SlotMachineBetTypes.Coin:
                coinKnob.Retract();
                break;

            case SlotMachineBetTypes.Gem:
                gemKnob.Retract();
                break;
        }
    }
    //
    //
    //
    #endregion EVENTS
    //
    //
    //
    private string Shuffle(SlotMachineBetTypes betType)
    {
        List<KeyValuePair<string, SlotMachineItemData>> filteredItems = new();

        // Filter items based on the bet type
        foreach (var item in slotMachineItems)
        {
            if (item.Value.ObtainableBy == betType || item.Value.ObtainableBy == SlotMachineBetTypes.Either)
            {
                filteredItems.Add(item);
            }
        }
        
        int totalWeight = 0;
        
        foreach (var item in filteredItems)
        {
            totalWeight += item.Value.Rarity;
        }
        
        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;
        
        foreach (var item in filteredItems)
        {
            cumulativeWeight += item.Value.Rarity;
        
            if (randomValue < cumulativeWeight)
            {
                return item.Key; 
            }
        }

        return "Random"; // Fallback in case something goes wrong
    }
    //
    // We can tell if we got jackpot if item is a "junk item"
    //
    private bool GotJackpot(string result)
    {
        // Tell if we got the jackpot
        if (junks.Contains(result))
            return false;

        return true;
    }

    private void ToggleItemFrameResult(bool show = true)
    {
        itemFrameResultLeft.enabled     = show;
        itemFrameResultMiddle.enabled   = show;
        itemFrameResultRight.enabled    = show;
    }
    //
    // Give each frame a different sprite
    //
    private void SetItemFramesSprite(Sprite[] sprites)
    {
        itemFrameResultLeft.sprite   = sprites[0];
        itemFrameResultMiddle.sprite = sprites[1];
        itemFrameResultRight.sprite  = sprites[2];
    }

    public void SetReadyNextSpin() => readyForNextSpin = true;
}