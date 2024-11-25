using System.Collections;
using UnityEngine;

public class TutorialSkipper : MonoBehaviour
{
    private GameSessionManager gsm;

    [SerializeField] private bool goBackToMainMenu = true;
    [SerializeField] private GameObject fancyLoader;

    void Awake()
    {
        gsm = GameSessionManager.Instance;
    }

    public void Ev_SkipTutorial() => StartCoroutine(IESkipTutorial());

    private IEnumerator IESkipTutorial()
    {
        var nextStep = TutorialSteps.TUTORIALS_COMPLETE;
        var userData = BuildSaveData(nextStep);
        var helper = UserDataHelper.Instance;

        yield return StartCoroutine(helper.SaveUserData(userData, (updatedData) =>
        {
            gsm.UserSessionData = updatedData;

            if (!goBackToMainMenu)
                return;

            var loaderObj = Instantiate(fancyLoader);

            if (loaderObj.TryGetComponent(out FancySceneLoader loader))
            {
                var mainMenu = Constants.Scenes.MainMenu;
                loader.LoadScene(mainMenu, false);
            }
        }));
    }

    private UserData BuildSaveData(TutorialSteps nextStep)
    {
        var userData = gsm.UserSessionData;

        userData.IsTutorialCompleted  = true;
        userData.CurrentTutorialStep  = nextStep;
        userData.CurrentTutorialStage = TutorialDriver.MAX_STAGES;

        return userData;
    }
}
