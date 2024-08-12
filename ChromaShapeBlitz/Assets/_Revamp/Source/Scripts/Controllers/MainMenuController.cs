using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private BackgroundMusic bgmManager;
    private GameSessionManager gsm;

    [Header("Used when user visits shop after game over")]
    [SerializeField] private GameObject shopMenu;

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
    }

    #region UI_EVENT_ACTIONS

    public void Ev_Quit()
    {
        Application.Quit();
    }

    #endregion
}