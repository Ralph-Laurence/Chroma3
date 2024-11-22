using System;
using TMPro;
using UnityEngine;

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

    [Header("Side Fabs")]
    [SerializeField] private MainMenuSideFab slotMachineFab;

    private UserData userData;

    void Awake()
    {
        gsm = GameSessionManager.Instance;
        userData = gsm.UserSessionData;

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

        // Side fabs
        var nextSpinTime = DateTime.Parse(userData.NextAllowedSpinTime);

        var minCoins = SlotMachine.BetCoinAmount;
        var minGems = SlotMachine.BetGemAmount;

        if (userData.SlotMachineSpinsLeft > 0 && DateTime.Now >= nextSpinTime &&
           (userData.TotalCoins >= minCoins || userData.TotalGems >= minGems))

            slotMachineFab.MakeActive();
        else
            slotMachineFab.MakeInactive();

        // Versioning
        versionLabel.text = $"v{Application.version}";
        versionLabel.enabled = true;
    }

    #region UI_EVENT_ACTIONS

    public void Ev_Quit() => Application.Quit();
    public void Ev_LaunchSlotMachine() => Launch(Constants.Scenes.SlotMachine);
    public void Ev_LaunchAbout() => Launch(Constants.Scenes.About);
    public void Ev_LaunchCredits() => Launch(Constants.Scenes.Credits);

    private void Launch(string sceneName)
    {
        Instantiate(fancySceneLoader).TryGetComponent(out FancySceneLoader loader);
        loader.LoadScene(sceneName);
    }

    #endregion
}