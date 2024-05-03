using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour 
{
    private GameManager gameManager;
    [SerializeField] private Image stagePatternIcon;
    [SerializeField] private PatternIconsScriptableObject patternIconsSO;

    [Space(5)] [Header("Success Screen Buttons")]
    [SerializeField] private Button successQuitButton;
    [SerializeField] private Button successReplayButton;
    [SerializeField] private Button nextButton;


    [Space(5)] [Header("Failure Screen Buttons")]
    [SerializeField] private Button failureQuitButton;
    [SerializeField] private Button failureReplayButton;


    [Space(5)] [Header("Pause Menu Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button pauseMenuQuitButton;
    [SerializeField] private Button pauseMenuReplayButton;
    [SerializeField] private Button pauseMenuResumeButton;
    [SerializeField] private Button pauseMenuMuteButton;


    private void Awake()
    {
        GameObject.FindWithTag(Constants.Tags.GameManager).TryGetComponent(out gameManager);
    }

    private void Start()
    {
        if (gameManager != null)
            AssignButtonEvents();

        // Set the stage pattern icon
        stagePatternIcon.sprite = patternIconsSO.GetStagePattern( gameManager.GetStageNumber() );
    }

    private void AssignButtonEvents()
    {
        var actionExit  = new UnityAction(() => gameManager.ExitToMenu());
        var actionRetry = new UnityAction(() => gameManager.Retry());

        successQuitButton.onClick.AddListener     (actionExit);
        successReplayButton.onClick.AddListener   (actionRetry);
        nextButton.onClick.AddListener            (gameManager.NextStage);

        failureQuitButton.onClick.AddListener     (actionExit);
        failureReplayButton.onClick.AddListener   (actionRetry);

        pauseMenuQuitButton.onClick.AddListener   (actionExit);
        pauseMenuReplayButton.onClick.AddListener (actionRetry);
        pauseButton.onClick.AddListener           (gameManager.Pause );
        pauseMenuResumeButton.onClick.AddListener (gameManager.Resume);

        pauseMenuMuteButton.onClick.AddListener   ( () => {
            SfxManager.Instance.Mute();
            BgmManager.Instance.Mute();
        });
    }
}