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

    private Button button;
    private Slider levelProgressBar;
    private Text levelProgressText;

    private bool initialized;

    private void Initialize()
    {
        if (initialized)
            return;

        TryGetComponent(out button);

        progressMeterObj.TryGetComponent(out levelProgressBar);
        progressTextObj.TryGetComponent(out levelProgressText);

        initialized = true;
    }

    public void SetLocked(bool locked)
    {
        Initialize();

        if (locked)
        {
            lockIndicator.SetActive(true);
            button.interactable = false;
            return;
        }

        lockIndicator.SetActive(false);
        button.interactable = true;
    }

    public void UpdateLevelProgressMeter(int currentValue, int maxValue)
    {
        Initialize();

        levelProgressBar.maxValue = maxValue;
        levelProgressBar.value    = currentValue > 1 ? currentValue : 0;

        levelProgressText.text = $"{levelProgressBar.value}/{levelProgressBar.maxValue}";
    }

    public void MarkCompleted()
    {
        levelCompleteIndicator.SetActive(true);
        trophyIndicator.sprite = trophyIcon;
    }
}
