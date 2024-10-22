using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class StageSelectButton : MonoBehaviour
{
    public LevelDifficulties DifficultyLevel { get; set; }
    public int StageNumber { get; set; }

    [FormerlySerializedAs("stageNumberText")]
    public TextMeshProUGUI stageNumberLabel;
    public GameObject LockIndicator;
    
    [Header("Stars in array must be in order of display")]
    public Image[] StarImage;

    private Image buttonBackground;
    private Button button;

    private bool isInitialized;

    void Awake() => Initialize();
    void OnEnable() => Initialize();

    public void Initialize()
    {
        if (isInitialized)
            return;

        TryGetComponent(out buttonBackground);
        TryGetComponent(out button);

        isInitialized = true;
    }

    public void Ev_HandleClicked()
    {
        var eventArgs = new StageSelectedEventArgs
        {
            StageNumber = StageNumber,
            Difficulty  = DifficultyLevel
        };
        
        Debug.Log($"Select Args -> {eventArgs.Difficulty} -- {eventArgs.StageNumber}");

        StageSelectedNotifier.NotifyObserver(eventArgs);
    }

    public void SetTargetStage(LevelDifficulties difficulty, int stageNumber)
    {
        StageNumber     = stageNumber;
        DifficultyLevel = difficulty;

        stageNumberLabel.text = stageNumber.ToString();
    }

    public void SetAppearance(Sprite color) => buttonBackground.sprite = color;

    public void SetStars(int starCount, Sprite starFilled, Sprite starUnfilled)
    {
       for (var i = 0; i < StarImage.Length; i++)
       {
            if (i < starCount)
                StarImage[i].sprite = starFilled;
            else
                StarImage[i].sprite = starUnfilled;
       }
    }

    public void ResetStars(Sprite star)
    {
       for (var i = 0; i < StarImage.Length; i++)
       {
            StarImage[i].sprite = star;
       }
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    public void SetUnlocked()
    {
        stageNumberLabel.gameObject.SetActive(true);
        LockIndicator.SetActive(false);
        button.interactable = true;
    }
    
    public void SetLocked()
    {
        stageNumberLabel.gameObject.SetActive(false);
        LockIndicator.SetActive(true);
        button.interactable = false;
    }
}
