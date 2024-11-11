using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SlotMachineState
{
    Idle,
    Spinning,
    Stopped,
    Win,
    TryAgain,
    InsufficientCoin,
    InsufficientGem
}

public class SlotMachineAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private SlotMachineLightBulb[] smallBulbs;
    [SerializeField] private Sprite smallLightOn;
    [SerializeField] private Sprite smallLightOff;
    [SerializeField] private Sprite smallLightJackpot;
    [SerializeField] private Sprite smallLightTryAgain;

    [SerializeField] private RawImage itemStripLeft;
    [SerializeField] private RawImage itemStripMiddle;
    [SerializeField] private RawImage itemStripRight;

    [Space(5)]
    [SerializeField] private float itemStripScrollRate = 10.0F;

    [Space(10)]
    [SerializeField] private AudioClip sfxSpin;
    [SerializeField] private AudioClip sfxFailed;
    [SerializeField] private AudioClip sfxJackpot;
    [SerializeField] private AudioClip sfxDenied;

    private readonly string slotMachineDefaultTitle     = "Slot Machine";
    private readonly string slotMachineTitleSpinToWin   = "Spin To Win";
    private readonly string slotMachineTitleJackpot     = "Jackpot!";
    private readonly string slotMachineTitleTryAgain    = "Try Again";
    private readonly string slotMachineInsuffCoins      = "Not Enough Coins";
    private readonly string slotMachineInsuffGems       = "Not Enough Gems";

    private SoundEffects sfx;
    private bool spinBegan;
    
    private readonly int spinDuration = 5;

    public Action SpinAnimationEnded {get; set; }
    public Action SpinAnimationStarting {get; set; }
    public Action OnSpinEndWithResult { get; set; }

    private SlotMachineState slotMachineState;

    private readonly Color defaultTitleColor = new(0.87F, 0.48F, 0.15F, 1.00F);
    private readonly Color failTitleColor    = new(1.00F, 0.14F, 0.23F, 1.00F);

    private int spinCooldown;

    void Awake()
    {
        sfx = SoundEffects.Instance;      
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slotMachineState = SlotMachineState.Idle;
        AnimateLights();
    }

    // Update is called once per frame
    void Update()
    {
        if (spinBegan)
        {
            var uvRectLeft   = itemStripLeft.uvRect;
            var uvRectMiddle = itemStripMiddle.uvRect;
            var uvRectRight  = itemStripRight.uvRect;

            uvRectLeft.y    += Time.deltaTime * itemStripScrollRate;
            uvRectMiddle.y  += Time.deltaTime * itemStripScrollRate;
            uvRectRight.y   += Time.deltaTime * itemStripScrollRate;

            itemStripLeft.uvRect   = uvRectLeft;
            itemStripMiddle.uvRect = uvRectMiddle;
            itemStripRight.uvRect  = uvRectRight;
        }
    }
    //
    //
    //
    public void AnimateJackpot()
    {
        slotMachineState = SlotMachineState.Win;
        AnimateLights();
    }
    //
    //
    //
    public void AnimateNotEnoughCoins()
    {
        slotMachineState = SlotMachineState.InsufficientCoin;
        AnimateLights();
    }
    //
    //
    //
    public void AnimateNotEnoughGems()
    {
        slotMachineState = SlotMachineState.InsufficientGem;
        AnimateLights();
    }
    //
    //
    //
    public void AnimateTryAgain()
    {
        slotMachineState = SlotMachineState.TryAgain;
        AnimateLights();
    }
    //
    //
    //
    public void Begin(SlotMachineBetTypes bet)
    {
        if (spinBegan)
            return;

        spinBegan = true;
        spinCooldown = spinDuration;
        
        SpinAnimationStarting?.Invoke();
        ShowItemStrips();

        // Begin spinning the item strip when knob was pulled down
        StartCoroutine(BeginSpinAsync());
    }
    //
    // Perform the spin for 5 seconds
    //
    private IEnumerator BeginSpinAsync()
    {
        var wait = new WaitForSeconds(1.0F);
        
        sfx.PlayOnce(sfxSpin);

        slotMachineState = SlotMachineState.Spinning;
        AnimateLights();

        while (spinCooldown > 0)
        {
            yield return wait;
            spinCooldown--;
        }

        if (spinCooldown == 0)
            Stop();
    }
    //
    // Stop the spin and partially reset slot machine
    //
    private void Stop()
    {
        spinBegan = false;

        slotMachineState = SlotMachineState.Stopped;
        AnimateLights();
        ShowItemStrips(false);

        var defaultRect = new Rect(0.0F, 0.0F, 1.0F, 1.0F);

        itemStripLeft.uvRect   = defaultRect;
        itemStripMiddle.uvRect = defaultRect;
        itemStripRight.uvRect  = defaultRect;

        SpinAnimationEnded?.Invoke();
    }
    //
    //
    //
    private void ShowItemStrips(bool show = true)
    {
        itemStripLeft.enabled   = show;
        itemStripMiddle.enabled = show;
        itemStripRight.enabled  = show;
    }
    #region LIGHTS_ANIMATION
    //
    // Store Coroutine references in a dictionary
    //
    private readonly Dictionary<SlotMachineState, Coroutine> stateCoroutines = new();

    private void AnimateLights()
    {
        // Stop the current coroutine if it exists
        if (stateCoroutines.ContainsKey(slotMachineState) && stateCoroutines[slotMachineState] != null)
        {
            StopCoroutine(stateCoroutines[slotMachineState]);
            stateCoroutines[slotMachineState] = null;
        }

        // Start the new coroutine based on the slot machine state
        switch (slotMachineState)
        {
            case SlotMachineState.Idle:
                stateCoroutines[SlotMachineState.Idle] = StartCoroutine(AnimateIdleStateAsync());
                break;
            case SlotMachineState.Spinning:
                stateCoroutines[SlotMachineState.Spinning] = StartCoroutine(AnimateSpinningStateAsync());
                break;
            case SlotMachineState.Win:
                stateCoroutines[SlotMachineState.Win] = StartCoroutine(AnimateWinStateAsync());
                break;
            case SlotMachineState.Stopped:
                stateCoroutines[SlotMachineState.Stopped] = StartCoroutine(AnimateStoppedStateAsync());
                break;
            case SlotMachineState.TryAgain:
                stateCoroutines[SlotMachineState.TryAgain] = StartCoroutine(AnimateTryAgainStateAsync());
                break;
            case SlotMachineState.InsufficientCoin:
                stateCoroutines[SlotMachineState.InsufficientCoin] = StartCoroutine(AnimateNotEnoughFundsAsync(CurrencyType.Coin));
                break;
            case SlotMachineState.InsufficientGem:
                stateCoroutines[SlotMachineState.InsufficientGem] = StartCoroutine(AnimateNotEnoughFundsAsync(CurrencyType.Gem));
                break;
        }
    }


    private IEnumerator AnimateIdleStateAsync()
    {
        var sequenceRate = new WaitForSeconds(0.4F);
        var group = 0; // 0 -> Even; 1 -> Odd
        var titleChange = 0;
        var titleIndex = 0;

        while (slotMachineState == SlotMachineState.Idle)
        {
            // Turn on all Even lights and shutdown Odd lights
            if (group % 2 == 0)
            {
                for (var i = 0; i < smallBulbs.Length; i++)
                {
                    var light = (i % 2 == 0) ? smallLightOn : smallLightOff;
                    smallBulbs[i].Toggle(light);
                }
            }
            // Turn on all Odd lights and shutdown Even lights
            else
            {
                for (var i = 0; i < smallBulbs.Length; i++)
                {
                    var light = (i % 2 == 0) ? smallLightOff : smallLightOn;
                    smallBulbs[i].Toggle(light);
                }
            }

            group++;

            if (group > 1)
                group = 0;

            titleChange++;

            if (titleChange == 6)
            {
                var title = titleIndex % 2 == 0 ? slotMachineTitleSpinToWin : slotMachineDefaultTitle;
                titleText.text = title;
            }

            // Change title text only every five steps
            if (titleChange > 6)
            {
                titleIndex++;
                titleChange = 0;
            }

            yield return sequenceRate;
        }
    }
    
    private IEnumerator AnimateSpinningStateAsync()
    {
        int lightCount = 3; // Number of bulbs to light at a time
        int currentIndex = 0; // Starting index for lighting

        while (slotMachineState == SlotMachineState.Spinning)
        {
            // Turn off all bulbs first
            foreach (var bulb in smallBulbs)
            {
                bulb.Toggle(smallLightOff);
            }
            // Calculate and turn on the current set of bulbs
            for (int i = 0; i < lightCount; i++)
            {
                int bulbIndex = (currentIndex + i) % smallBulbs.Length;
                smallBulbs[bulbIndex].Toggle(smallLightOn);
            }
            // Move the starting index for the next frame
            currentIndex = (currentIndex + 1) % smallBulbs.Length;

            yield return new WaitForSeconds(0.1F); // Adjust the speed of the animation here
        }
    }

    private IEnumerator AnimateStoppedStateAsync()
    {
        // Turn on all bulbs
        foreach (var bulb in smallBulbs)
        {
            bulb.Toggle(smallLightOn);
        }

        yield return new WaitForSeconds(3.0F);

        slotMachineState = SlotMachineState.Idle;
        AnimateLights();
    }

    private IEnumerator AnimateWinStateAsync()
    {
        sfx.PlayOnce(sfxJackpot); 
        titleText.text = slotMachineTitleJackpot;

        for (int i = 0; i < 3; i++)
        {
            // Turn on all bulbs
            foreach (var bulb in smallBulbs)
            {
                bulb.Toggle(smallLightJackpot);
            }

            yield return new WaitForSeconds(0.2F);

            // Turn off all bulbs
            foreach (var bulb in smallBulbs)
            {
                bulb.Toggle(smallLightOff);
            }

            yield return new WaitForSeconds(0.2F);
        }

        OnSpinEndWithResult?.Invoke();
    }

    private IEnumerator AnimateTryAgainStateAsync()
    {
        sfx.PlayOnce(sfxFailed);
        titleText.text = slotMachineTitleTryAgain;

        for (int i = 0; i < 3; i++)
        {
            // Turn on all bulbs
            foreach (var bulb in smallBulbs)
            {
                bulb.Toggle(smallLightTryAgain);
            }

            yield return new WaitForSeconds(0.5F);
        }

        ResetTitleText();
        slotMachineState = SlotMachineState.Idle;
        AnimateLights();

        OnSpinEndWithResult?.Invoke();
    }

    private IEnumerator AnimateNotEnoughFundsAsync(CurrencyType currency)
    {
        sfx.PlayOnce(sfxDenied);

        titleText.text = currency == CurrencyType.Coin 
                       ? slotMachineInsuffCoins
                       : slotMachineInsuffGems;

        titleText.color = failTitleColor;

        for (int i = 0; i < 6; i++)
        {
            // Turn on all bulbs
            foreach (var bulb in smallBulbs)
            {
                bulb.Toggle(smallLightTryAgain);
            }

            yield return new WaitForSeconds(0.5F);
        }

        yield return new WaitForSeconds(0.3F);
        
        ResetTitleText();
        slotMachineState = SlotMachineState.Idle;
        AnimateLights();
    }
    //
    //
    //
    #endregion LIGHTS_ANIMATION
    //
    //
    //
    private void ResetTitleText()
    {
        titleText.text = slotMachineDefaultTitle;
        titleText.color = defaultTitleColor;
    }
}
