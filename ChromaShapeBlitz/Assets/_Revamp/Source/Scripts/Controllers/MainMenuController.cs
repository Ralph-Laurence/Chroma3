using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private BackgroundMusic bgmManager;
    private GameSessionManager gsm;

    [Header("Used when user visits shop after game over")]
    [SerializeField] private GameObject shopMenu;

    [Space(10)]
    [SerializeField] private TextMeshProUGUI versionLabel;

    [Space(5)]
    [SerializeField] private GameObject fancySceneLoader;

    void Awake()
    {
        gsm = GameSessionManager.Instance;

        bgmManager = BackgroundMusic.Instance;
        LeanTween.init();
    }

    void Start()
    {
        if (gsm != null && gsm.IsVisitShopOnGameOver)
        {
            gsm.IsVisitShopOnGameOver = false;
            shopMenu.SetActive(true);
        }

        if (bgmManager != null)
            bgmManager.PlayMainBgm();

        versionLabel.text = $"v{Application.version}";
        versionLabel.enabled = true;
    }

    #region UI_EVENT_ACTIONS

    public void Ev_Quit()
    {
        Application.Quit();
    }

    public void Ev_LaunchSlotMachine()
    {
        Instantiate(fancySceneLoader).TryGetComponent(out FancySceneLoader loader);
        loader.LoadScene(Constants.Scenes.SlotMachine);
    }

    #endregion
}