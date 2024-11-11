using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayTutorialStep8 : MonoBehaviour
{
    [SerializeField] private Image  hotbarSlotImage;
    [SerializeField] private Image  hotbarSlotCount;

    [SerializeField] private HintMarker     hintMarker;
    [SerializeField] private HandsOnStage   handsonStage;
    [SerializeField] private StageCamera    stageCamera;

    [SerializeField] private CanvasGroup     canvasGroup;

    [SerializeField] private GameObject      hintCalloutDialog;
    [SerializeField] private TextMeshProUGUI hintCalloutText;

    [SerializeField] private GameOverScreenSuccess  successDialog;
    [SerializeField] private GameOverScreenFailed   failedDialog;
    [SerializeField] private TutorialDriver         tutorialDriver;
    [SerializeField] private GameObject             fancySceneLoader;

    [Space(5)]
    [Header("Content Jumping")]
    [SerializeField] private int tutorialContentIndexFail;
    [SerializeField] private int tutorialContentIndexSuccess;
    private GameSessionManager gsm;

    private void Awake()
    {
        gsm = GameSessionManager.Instance;

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
    //
    // Assign these in the inspector
    //
    public void RetryLevel()    => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void Begin()         => handsonStage.BeginLevel();
    public void UsePowerup()    => StartCoroutine(IEUsePowerup());
    public void MoveNextLevel()
    {
        var targetScene = $"{Constants.Scenes.TutorialStagePrefix}5";

        Instantiate(fancySceneLoader).TryGetComponent(out FancySceneLoader loader);

        loader.AddTask(SaveProgress);
        loader.LoadScene(targetScene, false);
    }
    public IEnumerator IEUsePowerup()
    {
        // Allow pass thru
        canvasGroup.blocksRaycasts = false;

        var hintText = "Hint starts in\n<size=150%><color=#0B52FC>*time*</color></size>";

        hintCalloutText.text = "Watch the hint carefully.";
        hintCalloutDialog.SetActive(true);

        LeanTween.scale(hintCalloutDialog, Vector3.one, 0.25F);
        yield return new WaitForSeconds(1.3F);

        hintCalloutText.text = hintText.Replace("*time*", "3");
        yield return new WaitForSeconds(1.0F);

        hintCalloutText.text = hintText.Replace("*time*", "2");

        yield return new WaitForSeconds(1.0F);
        hintCalloutText.text = hintText.Replace("*time*", "1");

        yield return new WaitForSeconds(1.0F);
        LeanTween.scale(hintCalloutDialog, Vector3.forward * 1.0F, 0.25F);

        hotbarSlotCount.enabled = false;
        hotbarSlotImage.enabled = false;

        // Prevent the stage variant from being shown at the bottom screen 
        handsonStage.SetStickToBottom(false);

        var stageRotationY = handsonStage.transform.eulerAngles.y;

        // Rotate and move the camera at top-down position
        yield return StartCoroutine(stageCamera.IEViewFromAbove
        (
            followStageYRotation: stageRotationY
        ));

        // Rotate the pointer hand to be the same rotation as the stage variant
        // hintMarker.transform.localEulerAngles = new Vector3(90.0F, stageRotationY);
        hintMarker.MatchStageRotation(new Vector3(90.0F, stageRotationY));

        yield return hintMarker.ShowHints(LevelDifficulties.Easy);
        yield return new WaitForSeconds(0.5F);

        // Bring back to initial view angle before hints were shown.
        yield return StartCoroutine(stageCamera.IEUnviewFromAbove());

        hintMarker.gameObject.SetActive(false);
        handsonStage.SetStickToBottom(true);

        // Prevent pass thru
        canvasGroup.blocksRaycasts = true;

        TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);
    }

    private IEnumerator SaveProgress()
    {
        var userData = gsm.UserSessionData;
        
        userData.Inventory.EquippedPowerupIds.Clear();
        userData.Inventory.OwnedPowerups.Clear();

        userData.CurrentTutorialStep  = TutorialSteps.STEP9_GAMEPLAY_MECHANICS;
        userData.CurrentTutorialStage ++;

        // Write the changes to file
        yield return StartCoroutine(UserDataHelper.Instance.SaveUserData(userData));
    }
    //
    //
    //
}
