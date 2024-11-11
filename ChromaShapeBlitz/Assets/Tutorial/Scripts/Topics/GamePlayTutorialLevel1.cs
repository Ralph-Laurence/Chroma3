using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayTutorialLevel1 : MonoBehaviour
{
    public GameOverScreenSuccess gameOverDialogSuccess;
    public GameOverScreenFailed gameOverDialogFailed;

    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private TMP_StyleSheet styleSheet;
    [SerializeField] private TMP_SpriteAsset spriteAsset;

    [Space(5)]
    [Header("Replay when failed")]
    [SerializeField] private int retryPoint;

    [Header("The content that will trigger the replay")]
    [SerializeField] private TutorialContent contentOnReplay;

    void Start()
    {
        gameOverDialogSuccess.onDialogShown.AddListener(() => NotifyMoveNextStep());
    }

    void OnEnable() => TutorialEventNotifier.BindObserver(ObserveSequenceFilled);

    void OnDisable() => TutorialEventNotifier.UnbindObserver(ObserveSequenceFilled);

    private void ObserveSequenceFilled(string key, object data)
    {
        if (string.IsNullOrEmpty(key))
            return;

        //
        // Effective only during tutorial stage phase
        //
        if (key.Equals(TutorialEventNames.SequenceFilled))
            NotifyMoveNextStep();
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

    private void NotifyMoveNextStep()
    {
        TutorialEventNotifier.NotifyObserver(TutorialEventNames.MoveNextStep, null);
    }

    public void LaunchSkillTest() => SceneManager.LoadScene("TutorialStage_1A");
}