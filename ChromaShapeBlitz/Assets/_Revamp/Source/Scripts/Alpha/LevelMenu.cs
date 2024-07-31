using System;
using System.Collections;
using System.Collections.Generic;
using Revamp;
using UnityEngine;
using UnityEngine.UI;

public partial class LevelMenu : MonoBehaviour
{
    [SerializeField] private GameObject fancySceneLoader;

    [Space(10)] 
    [Header("..:: Page Transition Behaviour ::..", order = 1)]
    [Header("[A] Content Scrolling", order = 2)]
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform pages; // Assign the <Pages> RectTransform in the inspector
    [SerializeField] private float pageWidth = 300f; // Width of each page
    [SerializeField] private float spacing = 300f; // Spacing between the pages
    [SerializeField] private float duration = 0.5f; // Duration of the tween

    [Space(5)]
    [Header("[B] Background Transition")]
    private Image rootBackground;
    // [SerializeField] private Color[] pageBackgrounds = new[] 
    // {
    //     new Color(0.0F, 0.5F, 1.0F, 1.0F),     // Blue | Easy
    //     new Color(0.98F, 0.49F, 0.0F, 1.0F),    // Orange | Normal
    //     new Color(1.0F, 0.26F, 0.31F, 1.0F)     // Red | Hard
    // };

    [SerializeField] private Sprite[] pageBackgrounds;

    [Space(10)] [Header("Page Location Tracking")]
    [SerializeField] private Image[] pageTrackers;
    [SerializeField] private Sprite trackerActive;
    [SerializeField] private Sprite trackerInactive;

    [Space(10)] [Header("Scrolling Controls")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button nextButton;

    [Space(10)] [Header("Level Selection Behaviour")]
    [SerializeField] private GameObject difficultySelectionMenu;
    [SerializeField] private GameObject stageSelectMenuOverlay;
    [SerializeField] private GameObject stageSelectMenuContent;

    [Space(10)]
    [Header("Level Difficulty Selections")]
    [SerializeField] private LevelSelectPage levelSelectPageEasy;
    [SerializeField] private LevelSelectPage levelSelectPageNormal;
    [SerializeField] private LevelSelectPage levelSelectPageHard;
    
    [Space(10)] [Header("Stage Selection Behaviour")]
    [SerializeField] private GameObject stageSelectButtonPrefab;
    [SerializeField] private Transform stageSelectionScrollContent;
    [SerializeField] private ScrollRect stageSelectionScrollRect;
    [SerializeField] private Sprite starFilled;
    [SerializeField] private Sprite starUnfilled;

    [Space(10)]
    [SerializeField] private Sprite stageSelectButtonStyleInitial;
    [SerializeField] private Sprite stageSelectButtonStyleEasy;
    [SerializeField] private Sprite stageSelectButtonStyleNormal;
    [SerializeField] private Sprite stageSelectButtonStyleHard;

    [Header("Controls that belong to difficulty menu")]
    [SerializeField] private GameObject[] difficultyMenuControls;

    [Space(8)]
    private Button selectedLevelDifficultyButton;
    private RectTransform selectedLevelDifficultyRect;
    private RectTransform stageSelectionMenuRect;
    private Dictionary<LevelDifficulties, Sprite> stageButtonAppearances;

    private int selectedIndex = 0;
    private int totalPages;
    private readonly Vector2 trackerSizeOn = Vector2.one * 24.0F;
    private readonly Vector2 trackerSizeOff = Vector2.one * 16.0F;

    private readonly List<RectTransform> pageItems = new();
    private readonly List<StageSelectButton> stageSelectButtons = new();
    private LevelDifficulties lastSelectedDifficulty;
    private UserDataHelper userDataHelper;
    private UserData userData;
    private UISound uiSound;

    private bool isInitialized;
    private bool stageSelectionScrollRectTweening;

    void Initialize()
    {
        if (isInitialized)
            return;

        uiSound         = UISound.Instance;
        userDataHelper  = UserDataHelper.Instance;
        userData        = userDataHelper.GetLoadedData();
        
        // The Image component attached to this gameobject
        TryGetComponent(out rootBackground);

        stageSelectMenuContent.TryGetComponent(out stageSelectionMenuRect);

        stageButtonAppearances = new()
        {
            { LevelDifficulties.Easy, stageSelectButtonStyleEasy },
            { LevelDifficulties.Normal, stageSelectButtonStyleNormal },
            { LevelDifficulties.Hard, stageSelectButtonStyleHard },
        };

        // Pre-instantiate the stage select buttons
        // 
        for (var i = 0; i < GameManager.TotalEasyStages; i++)
        {
            var button = Instantiate(stageSelectButtonPrefab, stageSelectionScrollContent);
            button.TryGetComponent(out StageSelectButton selectButton);
            selectButton.Initialize();

            stageSelectButtons.Add(selectButton);
        }

        lastSelectedDifficulty = LevelDifficulties.Easy;
        UpdateStageSelectButtons(LevelDifficulties.Easy);

        // Cache references to level difficulty select buttons
        for (var i = 0; i < pages.childCount; i++)
        {
            pages.GetChild(i).TryGetComponent(out RectTransform rect);
            pageItems.Add(rect);
        }
        
        totalPages = pageItems.Count;

        ResetDifficultyPagerScrollPosition();

        // Update the overall progress of the level into a meterbar
        var progressEasy = userData.HighestEasyStage;

        if (userData.HighestEasyStage <= GameManager.TotalEasyStages-1)
           progressEasy -= 1;

        levelSelectPageEasy.UpdateLevelProgressMeter(progressEasy, GameManager.TotalEasyStages);
        levelSelectPageNormal.UpdateLevelProgressMeter(userData.HighestNormalStage, GameManager.TotalNormalStages);
        levelSelectPageHard.UpdateLevelProgressMeter(userData.HighestHardStage, GameManager.TotalHardStages);

        // Unlock the level selection pages
        levelSelectPageNormal.SetLocked( userData.HighestEasyStage != GameManager.TotalEasyStages );
        levelSelectPageHard.SetLocked( userData.HighestNormalStage != GameManager.TotalNormalStages);

        // Mark the level select pages as completed (if they really are)
        if (userData.EasyStagesCompleted)
            levelSelectPageEasy.MarkCompleted();

        if (userData.NormalStagesCompleted)
           levelSelectPageNormal.MarkCompleted();

        if (userData.HardStagesCompleted)
            levelSelectPageHard.MarkCompleted();
            
        isInitialized = true;
    }

    public void ScrollPage(int pageIndex)
    {
        UpdateDifficultyPagerButtons(pageIndex);

        // Calculate the target position
        var targetPosition = CalculateTargetXPosition(pageIndex);

        // Animate to the target position using LeanTween
        LeanTween.moveX(pages, targetPosition, duration).setEase(LeanTweenType.easeInOutQuad);
        
        // Interpolate the background color of the target page
        // TransitionBackgroundColor(pageIndex);
        var nextBg = pageBackgrounds[pageIndex];
        rootBackground.sprite = nextBg;

        AdjustScales(pageIndex);
        UpdateTrackerLocation(pageIndex);
    }

    /// <summary>
    /// Change the level select menu's background color as we scroll through pages.
    /// This utilizes the common tween duration.
    /// </summary>
    /// <param name="to">The target page index</param>
    // private void TransitionBackgroundColor(int to)
    // {
    //     var currentColor = rootBackground.color;
    //     var nextColor = pageBackgrounds[to];
        
    //     LeanTween.value(gameObject, 0f, 1f, duration).setOnUpdate((float t) => {
    //         rootBackground.color = Color.Lerp(currentColor, nextColor, t);
    //     });
    // }

    private void SetColorCallback(Color color) => rootBackground.color = color;

    private float CalculateTargetXPosition(int pageIndex)
    {
        var screenCenter = content.rect.width / 2.0F;
        var pageOffset   = pageIndex * (pageWidth + spacing);
        var targetPos    = screenCenter - (pageWidth / 2f) - pageOffset;

        return targetPos;
    }

    private void AdjustScales(int selectedPageIndex)
    {
        for (int i = 0; i < pageItems.Count; i++)
        {
            if (i == selectedPageIndex)
            {
                // Scale up the selected page
                LeanTween.scale(pageItems[i], Vector3.one, duration).setEase(LeanTweenType.easeInOutQuad);
            }
            else
            {
                // Scale down the other pages
                LeanTween.scale(pageItems[i], Vector3.one * 0.5f, duration).setEase(LeanTweenType.easeInOutQuad);
            }
        }
    }

    private void ResetDifficultyPagerScrollPosition()
    {
        selectedIndex = 0;
        var initialX = CalculateTargetXPosition(selectedIndex);

        pages.anchoredPosition = new Vector2
        (
            initialX,
            pages.anchoredPosition.y
        );
    }

    private void UpdateDifficultyPagerButtons(int pageIndex)
    {
        if (pageIndex == 0)
        {
            backButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(true);
        }
        else if (pageIndex == totalPages - 1)
        {
            backButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(false);
        }
        else
        {
            backButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(true);
        }
    }
    
    #region UI_EVENTS
    public void Ev_ScrollNext()
    {
        if (selectedIndex < totalPages - 1)
        {
            selectedIndex++;
            ScrollPage(selectedIndex);
        }
    }

    public void Ev_ScrollPrevious()
    {
        if (selectedIndex > 0)
        {
            selectedIndex--;
            ScrollPage(selectedIndex);
        }
    }

    public void Ev_SelectEasyLevels(Button button) => HandleLevelSelected(LevelDifficulties.Easy, button);
    public void Ev_SelectNormalLevels(Button button) => HandleLevelSelected(LevelDifficulties.Normal, button);
    public void Ev_SelectHardLevels(Button button)   => HandleLevelSelected(LevelDifficulties.Hard, button);

    public void Ev_CloseStageSelectMenu()
    {
        LeanTween.scale(stageSelectionMenuRect, Vector3.zero, 0.25F).setOnComplete(() => {

            LeanTween.scale(selectedLevelDifficultyRect, Vector3.one, 0.25F)
                     .setOnComplete(() => {
                        ShowDifficultyMenuControls(difficultyMenuControls, true);
                        stageSelectMenuOverlay.SetActive(false);
                });
        });
    }
    #endregion

    private void HandleLevelSelected(LevelDifficulties difficulty, Button sender)
    {
        stageSelectMenuOverlay.SetActive(true);
        ShowDifficultyMenuControls(difficultyMenuControls, false);

        // Prevent unnecessary redraw of each buttons
        if (lastSelectedDifficulty != difficulty)
        {
            UpdateStageSelectButtons(difficulty);
            lastSelectedDifficulty = difficulty;
        }

        // Grab the Rect transform component of the button that was clicked,
        // so that we can shrink [animate] it.
        selectedLevelDifficultyButton = sender;
        selectedLevelDifficultyButton.TryGetComponent(out selectedLevelDifficultyRect);
        
        stageSelectionScrollRectTweening = true;
        StartCoroutine(MaintainScrollTop(stageSelectionScrollRect));

        LeanTween.scale(selectedLevelDifficultyRect, Vector3.zero, 0.25F).setOnComplete(() =>
        {
            LeanTween.scale(stageSelectionMenuRect, Vector3.one, .25F).setOnComplete(() => 
            {
                stageSelectionScrollRectTweening = false;
                StopCoroutine(MaintainScrollTop(stageSelectionScrollRect));
            });
        });
    }

    private IEnumerator MaintainScrollTop(ScrollRect scrollRect)
    {
        while (stageSelectionScrollRectTweening)
        {
            scrollRect.verticalNormalizedPosition = 1.0F;
            yield return null;
        }
    }

    private void UpdateTrackerLocation(int pageIndex)
    {
        RectTransform rect;

        foreach (var tracker in pageTrackers)
        {
            tracker.sprite = trackerInactive;
            tracker.TryGetComponent(out rect);
            rect.sizeDelta = trackerSizeOff;
        }

        pageTrackers[pageIndex].sprite = trackerActive;
        pageTrackers[pageIndex].TryGetComponent(out rect);
        rect.sizeDelta = trackerSizeOn;
    }

    private void ShowDifficultyMenuControls(GameObject[] difficultyMenuControls, bool show)
    {
        foreach (var control in difficultyMenuControls)
        {
            control.SetActive(show);
        }
    }

    private void UpdateStageSelectButtons(LevelDifficulties difficulty)
    {
        // Identify the highest unlocked level by difficulty
        var highestUnlockedLevel = difficulty switch
        {
            LevelDifficulties.Easy   => userData.HighestEasyStage,
            LevelDifficulties.Normal => userData.HighestNormalStage,
            LevelDifficulties.Hard   => userData.HighestHardStage,
            _ => 0
        };

        // Each difficulty has a specific number of stages.
        // We can use this to show the total number of visible buttons.
        var visibleButtons = GameManager.GetTotalStages(difficulty);

        for (var i = 0; i < stageSelectButtons.Count; i++)
        {
            var button = stageSelectButtons[i];
            var stageNumber = i + 1;

            // Show only the visible buttons
            if (i < visibleButtons)
            {
                button.SetTargetStage(difficulty, stageNumber);
                button.Show();
            }
            else
            {
                button.Hide();
            }

            // Unlock a button according to highest unlocked level
            if (stageNumber <= highestUnlockedLevel)
            {
                var starsAttained = difficulty switch
                {
                    LevelDifficulties.Easy   => userData.StageProgressEasy[i].StarsAttained,
                    LevelDifficulties.Normal => userData.StageProgressNormal[i].StarsAttained,
                    LevelDifficulties.Hard   => userData.StageProgressHard[i].StarsAttained,
                    _ => 0
                };

                button.SetUnlocked();
                button.SetAppearance(stageButtonAppearances[difficulty]);
                button.SetStars(starsAttained, starFilled, starUnfilled);
            }
            else
            {
                button.SetLocked();
                button.SetAppearance(stageSelectButtonStyleInitial);
                button.ResetStars(starUnfilled);
            }
        };
    }

    #region EVENT_OBSERVERS
    void Awake() => Initialize();
    void OnDisable() => StageSelectedNotifier.UnbindEvent(ObserveStageSelected);

    void OnEnable()
    {
        StageSelectedNotifier.BindEvent(ObserveStageSelected);
        Initialize();
    }

    private void ObserveStageSelected(StageSelectedEventArgs eventArgs)
    {   
        // We will handle the click sound in this controller
        // instead of attaching UXButton component to each
        // stage select button to reduce memory usage.
        uiSound.PlayUxPositiveClick();
        var sceneLoader = Instantiate(fancySceneLoader);

        // Hide();
        var gsm = GameSessionManager.Instance;

        gsm.SelectedDifficulty  = eventArgs.Difficulty;
        gsm.SelectedStageNumber = eventArgs.StageNumber;

        sceneLoader.TryGetComponent(out FancySceneLoader fancyLoader);
        fancyLoader.LoadScene("GamePlay");
    }
    #endregion
}
