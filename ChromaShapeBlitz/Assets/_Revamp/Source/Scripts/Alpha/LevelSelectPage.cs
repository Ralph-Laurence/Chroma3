using UnityEngine;
using UnityEngine.UI;

public class LevelSelectPage : MonoBehaviour
{
    [SerializeField] private GameObject progressMeterObj;
    [SerializeField] private GameObject progressTextObj;
    [SerializeField] private GameObject lockIndicator;
    [SerializeField] private GameObject levelCompleteIndicator;
    [SerializeField] private Image trophyIndicator;
    [SerializeField] private Sprite trophyIcon;

    [SerializeField] protected Button button;
    [SerializeField] private Slider levelProgressBar;
    [SerializeField] private Text levelProgressText;

    public void SetLocked(bool locked)
    {
        if (locked)
        {
            lockIndicator.SetActive(true);
            button.interactable = false;
            return;
        }

        lockIndicator.SetActive(false);
        button.interactable = true;
    }
    /// <summary>
    /// Set the value of the progress meter according to the value of "currentValue".
    /// By default, the lower bound isnt inclusive.
    /// If "useExact" is omitted, we set the current value into the actual value without subtracting 1
    /// </summary>
    public void UpdateLevelProgressMeter(int currentValue, int maxValue, bool useExact = false)
    {
        levelProgressBar.maxValue = maxValue;

        if (currentValue >= maxValue)
            levelProgressBar.value = maxValue;

        else
            levelProgressBar.value = useExact ? currentValue : currentValue - 1;

        levelProgressText.text = $"{levelProgressBar.value}/{levelProgressBar.maxValue}";
    }
    public void UpdateLevelProgressMeter_old(int currentValue, int maxValue)
    {
        levelProgressBar.maxValue = maxValue;

        // if (currentValue <= 1)
        //     levelProgressBar.value = currentValue;
        
        //else if
        if (currentValue >= maxValue)
            levelProgressBar.value = maxValue;
            
        else
            levelProgressBar.value = currentValue;
        //if (includeLowerBound && currentValue == 1)
        // if (currentValue <= 1)
        //     levelProgressBar.value = 1;
        // else
        //     levelProgressBar.value = currentValue > 1 ? currentValue : 0;

        levelProgressText.text = $"{levelProgressBar.value}/{levelProgressBar.maxValue}";
    }

    public virtual void MarkCompleted()
    {
        levelCompleteIndicator.SetActive(true);
        trophyIndicator.sprite = trophyIcon;
    }
}
