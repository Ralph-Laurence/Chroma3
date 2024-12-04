using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayTutorialStep9 : MonoBehaviour
{

    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private AudioClip  pauseSound;
    [SerializeField] private AudioClip  resumeSound;

    [SerializeField] private HandsOnStage handsOnStage;
    [SerializeField] private GameOverScreenSuccess successDialog;
    [SerializeField] private GameOverScreenFailed failedDialog;
    [SerializeField] private TutorialDriver tutorialDriver;
    [SerializeField] private GameObject fancySceneLoader;

    [Space(5)]
    [Header("Content Jumping")]
    [SerializeField] private int tutorialContentIndexFail;
    [SerializeField] private int tutorialContentIndexSuccess;

    private GameSessionManager  gsm;
    private BackgroundMusic     bgm;
    private SoundEffects        sfx;

    private readonly int rewardCoins = 500;
    private readonly int rewardGems = 6;
    private void Awake()
    {
        gsm = GameSessionManager.Instance;
        bgm = BackgroundMusic.Instance;
        sfx = SoundEffects.Instance;

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

        Time.timeScale = 1.0F;
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0F;

        bgm.Pause();
        sfx.PlayOnce(pauseSound);

        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        sfx.PlayOnce(resumeSound);

        ClosePauseMenu();
        bgm.Resume();

        Time.timeScale = 1.0F;
    }

    public void Begin() => handsOnStage.BeginLevel();
    public void RetryLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void GiveReward()
    {
        Instantiate(fancySceneLoader).TryGetComponent(out FancySceneLoader loader);
        Time.timeScale = 1.0F;
        loader.AddTask(SaveProgress);
        loader.LoadScene(Constants.Scenes.TutorialFinished, false);
    }
    //
    // Used during tutorial
    //
    public void ClosePauseMenu() => pauseMenu.SetActive(false);
    private IEnumerator SaveProgress()
    {
        var userData = gsm.UserSessionData;

        // Give Continuity Reward
        userData.Inventory.OwnedPowerups.Add(new PowerupInventory
        { 
            PowerupID     = 110,
            CurrentAmount = 2
        });

        // Give Idea Reward
        userData.Inventory.OwnedPowerups.Add(new PowerupInventory
        {
            PowerupID = 100,
            CurrentAmount = 2
        });

        userData.Inventory.EquippedPowerupIds.Add(100);
        userData.Inventory.EquippedPowerupIds.Add(110);
        userData.TotalCoins += rewardCoins;
        userData.TotalGems  += rewardGems;

        userData.IsTutorialCompleted = true;

        userData.CurrentTutorialStep = TutorialSteps.TUTORIALS_COMPLETE;
        userData.CurrentTutorialStage++;

        userData.MainMenuTheme = MainMenuThemeIdentifier.Auto;

        // Write the changes to file
        yield return StartCoroutine(UserDataHelper.Instance.SaveUserData(userData));
    }
    //
    //
    //
}
