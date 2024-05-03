using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameLevels
{
    Easy,
    Normal,
    Hard
}

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject loaderUI;
    [SerializeField] private GameObject logoStatic;
    [SerializeField] private GameObject splashScreenUI;
    [SerializeField] private GameObject controlButtonsWrapper;
    [SerializeField] private GameObject splashScreenAnimationObj;

    [Space(5)] 
    [Header("Player Bank")]
    [SerializeField] private TextMeshProUGUI txtTotalCoins;
    [SerializeField] private TextMeshProUGUI txtTotalGems;

    [Space(5)]
    [Header("Selection Pads Container")]
    [SerializeField] private GameObject easySelectionContainer;
    [SerializeField] private GameObject normalSelectionContainer;
    [SerializeField] private GameObject hardSelectionContainer;

    [Space(5)]
    [Header("Selection Pads Appearance")]
    [SerializeField] private GameObject padPrefab;
    [SerializeField] private Sprite easyPad;
    [SerializeField] private Sprite normalPad;
    [SerializeField] private Sprite hardPad;
    [SerializeField] private Sprite lockedPad;

    [Space(5)]
    [Header("Dialogs")]
    [SerializeField] private GameObject dialogOverlay;
    [SerializeField] private GameObject comingSoonDialog;

    [Space(5)]
    [Header("Backdrop")]
    [SerializeField] private Image backdropImage;
    [SerializeField] private float backdropFadeDuration = 2.0F;
    [SerializeField] private float backdropTargetFadeAmount = 0.8F;

    private ControlButtonAnimation controlButtonsAnimation;
    private SplashAnimation splashAnimation;
    
    private PlayerProgress playerProgress;

    private LevelStructuresHelper lvlStructsHelper;
    private MainMenuState mainMenuState;

    void Awake()
    {
        playerProgress   = PlayerProgressHelper.Instance.GetProgressData();
        lvlStructsHelper = LevelStructuresHelper.Instance;
        mainMenuState    = MainMenuState.Instance;

        controlButtonsWrapper   .TryGetComponent(out controlButtonsAnimation);
        splashScreenAnimationObj.TryGetComponent(out splashAnimation);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Time.timeScale < 1.0F)
            Time.timeScale = 1.0F;

        splashAnimation.OnAnimationEndCallback = () =>
        {
            mainMenuState.IsSplashAlreadyShown = true;

            BgmManager.Instance.PlayMainMenu();

            HideSplashScreen();
            ShowControlButtons();
        };

        StartCoroutine(LoadGameData());
    }

    private IEnumerator LoadGameData()
    {
        loaderUI.SetActive(true);

        yield return StartCoroutine(BuildLevelSelectionMenu(GameLevels.Easy,   lvlStructsHelper.EasyStages));
        yield return StartCoroutine(BuildLevelSelectionMenu(GameLevels.Normal, lvlStructsHelper.NormalStages));
        yield return StartCoroutine(BuildLevelSelectionMenu(GameLevels.Hard,   lvlStructsHelper.HardStages));
        
        loaderUI.SetActive(false);

        yield return null;

        if (!mainMenuState.IsSplashAlreadyShown)
            yield return StartCoroutine(AnimateSplash());
        
        else
            yield return StartCoroutine(SkipSplash());

        yield return null;
    }

    private IEnumerator AnimateSplash()
    {
        splashAnimation.BeginAnimation();
        yield return null;
    }

    private IEnumerator SkipSplash()
    {
        HideSplashScreen();
        ShowControlButtons();

        yield return null;
    }

    public IEnumerator BuildLevelSelectionMenu(GameLevels level, List<StageInfo> stageInfo)
    {
        var padColorToUse     = lockedPad;
        var lastUnlockedStage = Constants.StartingStage;

        GameObject padsContainer = null;

        switch (level)
        {
            case GameLevels.Easy:
                lastUnlockedStage = playerProgress.HighestStageEasy;
                padsContainer     = easySelectionContainer;
                padColorToUse     = easyPad;
                break;

            case GameLevels.Normal:
                lastUnlockedStage = playerProgress.HighestStageNormal;
                padsContainer     = normalSelectionContainer;
                padColorToUse     = normalPad;
                break;

            case GameLevels.Hard:
                lastUnlockedStage = playerProgress.HighestStageHard;
                padsContainer     = hardSelectionContainer;
                padColorToUse     = hardPad;
                break;
        }

        foreach(var info in stageInfo)
        {
            // Instantiate (create) each pad, and make it as a child of the container
            var padObj = Instantiate(padPrefab, padsContainer.transform.position, Quaternion.identity);

            padObj.transform.SetParent(padsContainer.transform, false);

            // For unlocked stages, change their appearance
            // into colored pads and they must be clickable
            padObj.TryGetComponent(out Image padImage);
            padObj.TryGetComponent(out Button padButton);

            if (info.StarsAttained > 0)
            {
                padObj.TryGetComponent(out LevelSelectPad levelSelectPad);
                levelSelectPad.FillStars(info.StarsAttained);
            }

            // Get the TextMeshPro component
            var padText = padObj.GetComponentInChildren<TextMeshProUGUI>();

            // Change the button text to appear as stage number
            padText.text = info.StageNumber.ToString();

            if (info.StageNumber <= lastUnlockedStage)
            {
                padImage.sprite = padColorToUse;
                padButton.interactable = true;

                // When a pad button was clicked, launch the stage...
                padButton.onClick.AddListener(() =>
                {
                    var loadScene = lvlStructsHelper.OpenStage(info, level);

                    if (!loadScene)
                    {
                        dialogOverlay.SetActive(true);
                        comingSoonDialog.SetActive(true);
                    }
                });
            }
            else
            {
                padImage.sprite = lockedPad;
                padButton.interactable = false;
            }

            yield return null;
        }
    }

    private void ShowControlButtons()
    {
        // Set the Player Bank text contents before they are shown
        txtTotalCoins.text = playerProgress.CurrentCoins.ToString();
        txtTotalGems.text  = playerProgress.CurrentGems.ToString();

        // Begin showing the control buttons
        BgmManager.Instance.PlayMainMenu();
        controlButtonsAnimation.BeginAnimation();

        // Then show the backdrop prop
        StartCoroutine(FadeBackdrop());
    }

    private void HideSplashScreen()
    {
        splashScreenUI.SetActive(false);
        logoStatic.SetActive(true);
    }

    private IEnumerator FadeBackdrop()
    {
        Color imageColor = backdropImage.color;
        float fadeSpeed = 1f / backdropFadeDuration;

        while (backdropImage.color.a > backdropTargetFadeAmount)
        {
            imageColor.a -= fadeSpeed * Time.deltaTime;
            backdropImage.color = imageColor;
            yield return null;
        }
    }

    // Reset the scroll view back to top.
    // 0 -> Bottom; 1 -> Top.
    // This method must be assigned from UI Click Event
    public void ResetScrollView(ScrollRect scrollRect) => scrollRect.verticalNormalizedPosition = 1.0F;

    public void ExitGame()
    {
        BgmManager.Instance.Shutdown();
        SfxManager.Instance.Shutdown();

        Application.Quit();
    }
}
