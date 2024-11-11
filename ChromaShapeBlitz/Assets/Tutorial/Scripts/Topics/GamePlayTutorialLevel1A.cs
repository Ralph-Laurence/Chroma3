using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayTutorialLevel1A : MonoBehaviour
{
    public GameOverScreenSuccess gameOverDialogSuccess;
    public GameOverScreenFailed gameOverDialogFailed;

    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private TMP_StyleSheet styleSheet;
    [SerializeField] private TMP_SpriteAsset spriteAsset;

    [SerializeField] private TutorialDialog tutorialDialog;
    [SerializeField] private TutorialContent onStageFailed;

    [SerializeField] private HandsOnStage handsOnStage;
    [SerializeField] private Transform poofSmoke;
    [SerializeField] private AudioClip poofSmokeSfx;

    [SerializeField] private TutorialDriver tutorialDriver;
    [SerializeField] private GameObject fancySceneLoader;

    private SoundEffects sfx;
    private GameSessionManager gsm;

    private bool handsonStageReady;

    private readonly int rewardCoin = 1000;
    private readonly int rewardGems = 20;

    private void Awake()
    {
        sfx = SoundEffects.Instance;
        gsm = GameSessionManager.Instance;
    }

    void OnEnable()
    {
        gameOverDialogFailed.onDialogShown.AddListener(HandleFailDialogShown);
        gameOverDialogSuccess.onDialogShown.AddListener(HandleSuccessDialogShown);

        TutorialEventNotifier.BindObserver(ObserveSequenceFilled);
    }

    void OnDisable()
    {
        TutorialEventNotifier.UnbindObserver(ObserveSequenceFilled);
    }

    void Update()
    {
        if (handsonStageReady)
            return;

        handsonStageReady  = true;
        poofSmoke.gameObject.SetActive(true);

        sfx.PlayOnce(poofSmokeSfx);

        poofSmoke.position = new Vector3
            (
                handsOnStage.transform.position.x,
                handsOnStage.transform.position.y + 0.5F,
                handsOnStage.transform.position.z
            );

        var stageBegan = false;

        LeanTween.scale(handsOnStage.gameObject, Vector3.one, 0.3F)
                 .setOnUpdate((float time) =>
                 {
                     if (time >= 0.75F && !stageBegan)
                     {
                         stageBegan = true;
                         handsOnStage.BeginLevel();
                     }
                 });
    }

    private void ObserveSequenceFilled(string key, object data)
    {
        if (string.IsNullOrEmpty(key))
            return;

        //
        // Effective only during tutorial stage phase
        //
        //if (key.Equals(TutorialEventNames.SequenceFilled))
        //    NotifyMoveNextStep();
    }

    private void HandleSuccessDialogShown()
    {
        if (tutorialDriver == null)
            return;

        TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);
    }

    private void HandleFailDialogShown()
    {
        if (tutorialDriver == null)
            return;

        tutorialDriver.ShowTutorialContent();
    }

    public void AddIconsToDialogTextMesh()
    {
        dialogText.spriteAsset = spriteAsset;
        dialogText.styleSheet = styleSheet;
    }

    public void DetachSpritesAndStyles()
    {
        dialogText.spriteAsset = null;
        dialogText.styleSheet = null;
    }

    public void RetryLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void ExitToMenu()
    {
        Instantiate(fancySceneLoader).TryGetComponent(out FancySceneLoader loader);

        loader.AddTask(SaveProgress);
        loader.LoadScene(Constants.Scenes.MainMenu, false);
    }

    private IEnumerator SaveProgress()
    {
        var userData = gsm.UserSessionData;

        userData.CurrentTutorialStep = TutorialSteps.STEP2_BLOCK_SKIN_PURCHASE;
        userData.TotalCoins += rewardCoin;
        userData.TotalGems  += rewardGems;
        
        userData.CurrentTutorialStage ++;

        var helper = UserDataHelper.Instance;

        yield return StartCoroutine(helper.SaveUserData(userData, (updatedData) =>
        {
            gsm.UserSessionData = updatedData;

            Debug.Log(gsm.UserSessionData.TotalCoins);
        }));
    }
}