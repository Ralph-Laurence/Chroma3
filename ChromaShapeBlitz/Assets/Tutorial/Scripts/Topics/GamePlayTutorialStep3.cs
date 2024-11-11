using System.Collections;
using UnityEngine;

public class GamePlayTutorialStep3 : MonoBehaviour
{
    [SerializeField] private GameOverScreenSuccess gameOverDialogSuccess;
    [SerializeField] private HandsOnStage handsOnStage;
    [SerializeField] private TutorialDriver tutorialDriver;

    [SerializeField] private GameObject fancySceneLoader;

    private readonly int rewardCoin = 120;
    private GameSessionManager gsm;

    private void Awake()
    {
        gsm = GameSessionManager.Instance;
        gameOverDialogSuccess.onDialogShown.AddListener(HandleSuccessDialogShown);
    }

    public void BeginLevel() => handsOnStage.BeginLevel();

    private void HandleSuccessDialogShown()
    {
        if (tutorialDriver == null)
            return;

        tutorialDriver.enabled = true;
        TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);
    }

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
        userData.CurrentTutorialStep = TutorialSteps.STEP4_BACKGROUND_PURCHASE;
        userData.TotalCoins += rewardCoin;
        
        userData.CurrentTutorialStage ++;

        var helper = UserDataHelper.Instance;

        yield return StartCoroutine(helper.SaveUserData(userData, (updatedData) =>
        {
            gsm.UserSessionData = updatedData;
        }));
    }
}
