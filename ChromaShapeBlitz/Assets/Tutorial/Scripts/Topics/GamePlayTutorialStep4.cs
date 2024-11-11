using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayTutorialStep4 : MonoBehaviour
{
    [SerializeField] private HandsOnStage stage;
    [SerializeField] private GameObject fancySceneLoader;
    [SerializeField] private StageCamera stageCamera;

    [SerializeField] private GameOverScreenSuccess successDialog;
    [SerializeField] private GameOverScreenFailed failedDialog;

    [SerializeField] private TutorialDriver tutorialDriver;

    [Space(5)]
    [Header("Content Jumping")]
    [SerializeField] private int tutorialContentIndexFail;
    [SerializeField] private int tutorialContentIndexSuccess;

    private GameSessionManager gsm;

    private Vector3 stageCameraDefaultRot;
    private int rewardCoin = 100;

    //
    //
    #region MONOBEHAVIOUR_METHODS
    //
    //
    void Awake()
    {
        gsm = GameSessionManager.Instance;

        stageCameraDefaultRot = stageCamera.transform.localEulerAngles;
    }
    //
    // EVENT OBSERVERS
    //
    void OnEnable()
    {
        TutorialEventNotifier.BindObserver(ObserveTutorialEvents);

        failedDialog.onDialogShown.AddListener(() =>
        {
            tutorialDriver.JumpToContentIndex(tutorialContentIndexFail);
            tutorialDriver.ShowTutorialContent();
        });

        successDialog.onDialogShown.AddListener(() =>
        {
            tutorialDriver.JumpToContentIndex(tutorialContentIndexSuccess);
            tutorialDriver.ShowTutorialContent();
        });
    }

    void OnDisable() => TutorialEventNotifier.UnbindObserver(ObserveTutorialEvents);
    //
    //
    #endregion MONOBEHAVIOUR_METHODS
    //
    //
    public void ObserveTutorialEvents(string eventName, object data)
    {
        var failed = eventName.Equals(TutorialEventNames.GameOverFailed);
        var passed = eventName.Equals(TutorialEventNames.GameOverSuccess);

        if (failed || passed)
            stageCamera.Freeze();
    }

    public void BeginGameplay()
    {
        LeanTween.rotateY(stageCamera.gameObject, stageCameraDefaultRot.y, 0.25F)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() =>
                 {
                     stageCamera.UnFreeze();
                     stage.BeginLevel();
                 });
    }
    public void RetryLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void MoveNextStep() => TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);
    public void ExitToMenu()
    {
        Instantiate(fancySceneLoader).TryGetComponent(out FancySceneLoader loader);

        loader.AddTask(SaveProgress);
        loader.LoadScene(Constants.Scenes.MainMenu, false);
    }

    private IEnumerator SaveProgress()
    {
        var userData = gsm.UserSessionData;

        // Unlock Next step:
        userData.CurrentTutorialStep = TutorialSteps.STEP6_POWERUP_PURCHASE;
        userData.TotalCoins += rewardCoin;
        
        userData.CurrentTutorialStage ++;

        var helper = UserDataHelper.Instance;

        yield return StartCoroutine(helper.SaveUserData(userData, (updatedData) =>
        {
            gsm.UserSessionData = updatedData;
        }));
    }
}
