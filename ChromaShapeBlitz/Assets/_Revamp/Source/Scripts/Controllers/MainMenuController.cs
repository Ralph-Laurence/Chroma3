using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private BackgroundMusic bgmManager;

    void Awake()
    {
        bgmManager = BackgroundMusic.Instance;
        LeanTween.init();
    }

    void Start()
    {
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