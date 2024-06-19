using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class LevelMenuController : MonoBehaviour
{
    #region SINGLETON
    public static LevelMenuController Instance {get; private set;}

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        else if (Instance != this)
            Destroy(gameObject);
    }
    #endregion SINGLETON

    [Space(10)]
    [SerializeField] private GameObject mainCanvas;
    [SerializeField] private GameObject mainCanvasContent;
    [SerializeField] private GameObject topRibbon;

    [Space(10)] [Header("Level Select Behaviour")]
    [SerializeField] private HorizontalScrollSnap levelSelectScrollView;
    [SerializeField] private GameObject pageIndicator;
    [SerializeField] private GameObject scrollControlButtons;
    [SerializeField] private Button stageMenuBackButton;


    [Space(10)] [Header("Stage Select Behaviour")]
    [SerializeField] private GameObject fancySceneLoader;
    [SerializeField] private GameObject stageSelectButtonPrefab;
    [SerializeField] private Sprite buttonAppearanceInitial;
    [SerializeField] private Sprite buttonAppearanceEasy;
    [SerializeField] private Sprite buttonAppearanceNormal;
    [SerializeField] private Sprite buttonAppearanceHard;

    [SerializeField] GameObject stageSelectionMenu;
    [SerializeField] private ScrollRect scrollView;

    [Header("RectTransform of scrollview content")]
    [SerializeField] private RectTransform scrollViewContent;

    private RectTransform mainCanvasContentRect;
    private RectTransform stageSelectionMenuRect;
    private List<StageSelectButton> stageButtons;
    private Dictionary<LevelDifficulties, Sprite> buttonAppearances;
    private LevelSelectPage eventSender;
    private UserDataHelper userDataHelper;
    private UserData userData;
    private UISound uiSound;
    private LTDescr tween;

    private bool isLevelSelectScrollViewTweening;
    private bool isScrollViewParentTweening;
    private bool isInitialized;

    public IEnumerator Initialize()
    {
        if (isInitialized)
            yield break;

        ActionStatusNotifier.NotifyObserver(StatusCodes.BEGIN_BUILD_LEVEL_MENU);

        uiSound           = UISound.Instance;
        userDataHelper    = UserDataHelper.Instance;
        userData          = userDataHelper.GetLoadedData();

        stageButtons      = new List<StageSelectButton>();
        buttonAppearances = new Dictionary<LevelDifficulties, Sprite>
        {
            { LevelDifficulties.Easy,   buttonAppearanceEasy    },
            { LevelDifficulties.Normal, buttonAppearanceNormal  },
            { LevelDifficulties.Hard,   buttonAppearanceHard    },
        };

        mainCanvasContent.TryGetComponent(out mainCanvasContentRect);

        stageSelectionMenu.TryGetComponent(out stageSelectionMenuRect);
        stageMenuBackButton.onClick.AddListener(delegate {
            CloseStageSelectMenu(eventSender);
        });

        for (var i = 0; i < Revamp.GameManager.TotalEasyStages; i++)
        {
            var btn = Instantiate(stageSelectButtonPrefab, scrollViewContent);
            btn.TryGetComponent(out StageSelectButton button);

            stageButtons.Add(button);
        }

        isInitialized = true;

        ActionStatusNotifier.NotifyObserver(StatusCodes.DONE_BUILD_LEVEL_MENU);
        yield return null;
    }

    public void SetDifficulty(LevelDifficulties difficulty, int visibleButtons)
    {
        for (int i = 0; i < stageButtons.Count; i++)
        {
            var button = stageButtons[i];

            if (i < visibleButtons)
            {   
                var stageNumber = i + 1;
                
                button.SetTargetLevel(difficulty, stageNumber);

                if (stageNumber <= userData.HighestEasyStage)
                {
                    button.SetAppearance(buttonAppearances[difficulty]);
                    button.SetUnlocked();
                }
                else
                {
                    button.SetAppearance(buttonAppearanceInitial);
                    button.SetLocked();
                }
                button.Show();
            }
            else
            {
                button.Hide();
            }
        }
    }

    private IEnumerator MaintainScrollPositionDuringTween()
    {
        while (isScrollViewParentTweening)
        {
            scrollView.verticalNormalizedPosition = 1.0F;
            yield return null; // Wait for the next frame
        }
    }

    private void CloseStageSelectMenu(LevelSelectPage sender)
    {
        tween?.reset();
        tween = LeanTween.scale(stageSelectionMenuRect, Vector3.zero, 0.25F).setOnComplete(() => {
            stageSelectionMenu.SetActive(false);
            isScrollViewParentTweening = false;

            pageIndicator.SetActive(true);
            scrollControlButtons.SetActive(true);
            topRibbon.SetActive(true);
            
            if (sender != null)
                OnStageSelectMenuCloseNotifier.NotifyObserver(sender);
        });
    }

    private IEnumerator MaintainLevelSelectScrollPositionDuringTween()
    {
        while (isLevelSelectScrollViewTweening)
        {
            levelSelectScrollView.NormalizeScrollPosition();
            yield return null; // Wait for the next frame
        }
    }

    public void Show()
    {
        mainCanvas.SetActive(true);
        
        isLevelSelectScrollViewTweening = true;
        StartCoroutine(MaintainLevelSelectScrollPositionDuringTween());

        tween?.reset();
        tween = LeanTween.scale(mainCanvasContentRect, Vector3.one, 0.25F).setOnComplete(() => {
            StopCoroutine(MaintainLevelSelectScrollPositionDuringTween());
            isLevelSelectScrollViewTweening = false;
        });
    }

    public void Hide()
    {
        CloseStageSelectMenu(eventSender);

        tween = LeanTween.scale(mainCanvasContentRect, Vector3.zero, 0.25F).setOnComplete(() => {
            mainCanvas.SetActive(false);
            levelSelectScrollView.ResetScrollPosition();
            tween?.reset();
        });
    }

    #region EVENT_OBSERVERS

    void OnEnable()
    {
        LevelMenuNotifier.BindLevelPageClickEvent(ObserveLevelMenuPageClicked);
        StageSelectedNotifier.BindEvent(ObserveStageSelected);
    }

    void OnDisable()
    {
        LevelMenuNotifier.UnbindLevelPageClickEvent(ObserveLevelMenuPageClicked);
        StageSelectedNotifier.UnbindEvent(ObserveStageSelected);
    }

    private void ObserveLevelMenuPageClicked(LevelSelectPageData data)
    {
        topRibbon.SetActive(false);
        pageIndicator.SetActive(false);
        scrollControlButtons.SetActive(false);
        stageSelectionMenu.SetActive(true);

        SetDifficulty(data.Difficulty, data.TotalStages);

        // Start coroutine to maintain scroll position at the top during the tween
        isScrollViewParentTweening = true;
        StartCoroutine(MaintainScrollPositionDuringTween());

        tween?.reset();
        tween = LeanTween.scale(stageSelectionMenuRect, Vector3.one, 0.25F).setOnComplete(() => {
            // Stop the coroutine after the scaling animation
            StopCoroutine(MaintainScrollPositionDuringTween());
            // Reset the scroll position to ensure it remains at the top
            scrollView.verticalNormalizedPosition = 1.0F;
            isScrollViewParentTweening = false;

            eventSender = data.EventSender;
        });
    }

    private void ObserveStageSelected(StageSelectedEventArgs eventArgs)
    {   
        // We will handle the click sound in this controller
        // instead of attaching UXButton component to each
        // stage select button to reduce memory usage.
        uiSound.PlayUxPositiveClick();
        var sceneLoader = Instantiate(fancySceneLoader);

        Hide();
        var gsm = GameSessionManager.Instance;

        gsm.SelectedDifficulty  = eventArgs.Difficulty;
        gsm.SelectedStageNumber = eventArgs.StageNumber;

        sceneLoader.TryGetComponent(out FancySceneLoader fancyLoader);
        fancyLoader.LoadScene("GamePlay");
    }
    #endregion EVENT_OBSERVERS
}