using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private BackgroundMusic bgmManager;

    void Awake()
    {
        bgmManager = BackgroundMusic.Instance;
    }

    void Start()
    {
        if (bgmManager != null)
            bgmManager.PlayMainBgm();
    }

    public void Ev_Quit()
    {
        Application.Quit();
    }
}