using Revamp;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageTester : MonoBehaviour
{
    private GameSessionManager gsm;
    private BackgroundMusic bgm;

    [Space(10)]
    [Header("Test Materials")]
    [SerializeField] private Material Blue;
    [SerializeField] private Material Green;
    [SerializeField] private Material Magenta;
    [SerializeField] private Material Orange;
    [SerializeField] private Material Purple;
    [SerializeField] private Material Yellow;

    [Space(10)]
    [Header("Behaviour")]
    [SerializeField] private GameObject testStage;
    [SerializeField] private Image previewer;
    [SerializeField] private TextMeshProUGUI stageName;
    [SerializeField] private Image completionIcon;
    [SerializeField] private Sprite completionSuccess;
    [SerializeField] private Sprite completionFail;

    private StageVariant stageVariant;

    void Awake()
    {
        gsm = GameSessionManager.Instance;
        bgm = BackgroundMusic.Instance;
        
        var mats = new BlockSkinsSession
        {
            BlueSkinMat = Blue,
            GreenSkinMat = Green,
            MagentaSkinMat = Magenta,
            OrangeSkinMat = Orange,
            PurpleSkinMat = Purple,
            YellowSkinMat = Yellow
        };

        gsm.BlockSkinMaterialsInUse = mats;
    }

    void Start()
    {
        Instantiate(testStage).TryGetComponent(out stageVariant);

        previewer.sprite = stageVariant.PatternObjective;
        bgm.SetClip(stageVariant.BgmClip);

        stageName.text = stageVariant.name.Replace("(Clone)", string.Empty);

        stageVariant.enabled = true;
        bgm.Play();
    }

    void OnEnable() => OnStageCompleted.BindEvent(HandleSequenceCompleted);

    void OnDisable() => OnStageCompleted.UnbindEvent(HandleSequenceCompleted);

    private void HandleSequenceCompleted(StageCompletionType completionType)
    {
        completionIcon.sprite = completionType == StageCompletionType.Success
                              ? completionSuccess
                              : completionFail;
    }

    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.R))
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        #endif
    }
}
