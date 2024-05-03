using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] 
    [Tooltip("When true, give the player some seconds before starting the game")] 
    private bool preparePlayer = true;

    [SerializeField] private int prepTimeSec = 4;
    [SerializeField] private GameObject prepWrapper;
    [SerializeField] private TextMeshProUGUI prepText;

    public SliderTimer gameTimer;

    private GameResults gameResult;

    /// <summary>
    /// Partially finished means the level is accomplished,
    /// but the entire level is not yet done with processing
    /// other stuff such as game over animations.
    /// </summary>
    public bool IsLevelPartiallyFinished { private set; get; }

    [Space(10)]
    [Header("Game Mechanics")]

    private int secondsFinished;    // The seconds it takes to finish the level
    private int currentStarsEarned;
    private int currentRewardAmountEarned;
    private RewardType currentRewardTypeEarned;
    private StageInfo currentStageInfo;
    private StageInfo nextStageInfo;
    
    private LevelStructuresHelper lvlStructuresHelper;
    private LevelSessionData sessionData;
    private int currentStageIndex;
    private bool isFinalStage = false;

    private void Awake()
    {
        sessionData          = LevelSessionStateTracker.Instance.GetSessionData();
        lvlStructuresHelper  = LevelStructuresHelper.Instance;
        currentStageInfo     = sessionData.SelectedStage;
    }

    private void Start()
    {
        LoadStageInfo();

        if (preparePlayer)
        {
            Time.timeScale = 0;
            StartCoroutine(PrepareGame());
        }
    }

    /// <summary>
    /// Load properties and other information tied to the current stage.
    /// </summary>
    private void LoadStageInfo()
    {
        // Identify the indeces of the current and next stage
        switch (sessionData.SelectedLevel)
        {
            case GameLevels.Easy:

                var easy = lvlStructuresHelper.EasyStages;

                currentStageIndex = easy.IndexOf(currentStageInfo);
                nextStageInfo     = easy[currentStageIndex + 1];

                if (sessionData.SelectedStage.StageNumber == easy.Count)
                    isFinalStage = true;

                break;

            case GameLevels.Normal:

                var normal = lvlStructuresHelper.NormalStages;

                currentStageIndex = normal.IndexOf(currentStageInfo);
                nextStageInfo     = normal[currentStageIndex + 1];

                if (sessionData.SelectedStage.StageNumber == normal.Count)
                    isFinalStage = true;

                break;

            case GameLevels.Hard:

                var hard = lvlStructuresHelper.HardStages;

                currentStageIndex = hard.IndexOf(currentStageInfo);
                nextStageInfo     = hard[currentStageIndex + 1];

                if (sessionData.SelectedStage.StageNumber == hard.Count)
                    isFinalStage = true;
                break;
        }
    }

    public void SetGamePassed()
    {
        gameResult = GameResults.Passed;

        CalculateStarsAttained();
        var claim = ClaimReward(out var rewardType, out var rewardAmount);

        var animatableValues = new GameOverAnimatableValues
        {
            GameResult           = GameResults.Passed,
            RewardAlreadyClaimed = claim,
            RewardType           = rewardType,
            RewardAmount         = rewardAmount,
            TotalStars           = currentStarsEarned,
            TotalPlayTime        = secondsFinished
        };

        // If the original reward has not been claimed yet,
        // then flag it as Claimed
        if (!claim)
            currentStageInfo.RewardClaimed = true;
        
        StartCoroutine(SaveProgress());

        // Begin Animation
        OnGameOverNotifier.Publish(animatableValues);
    }

    public void SetGameFailed()
    {
        gameResult = GameResults.Failed;

        // Begin Animation
        OnGameOverNotifier.Publish(new GameOverAnimatableValues
        {
            GameResult = GameResults.Failed,
        });
    }

    public void NotifySequencesCompleted()
    {
        IsLevelPartiallyFinished = true;

        // Stop the timer then get the total play time
        gameTimer.Stop();
        secondsFinished = gameTimer.ElapsedSeconds;

        BgmManager.Instance.StopBgm();
    }

    public bool IsGameOver() => gameResult == GameResults.Failed;

    public int GetStageNumber() => currentStageInfo.StageNumber;

    private IEnumerator PrepareGame()
    {
        // Display countdown for 3 seconds
        int countdown = prepTimeSec;

        while (countdown > 0)
        {
            Debug.Log("Game starts in " + countdown + " seconds...");

            if (countdown < prepTimeSec)
            {
                switch (countdown)
                {
                    case 3:
                        prepText.text = "Ready";
                        break;

                    case 2:
                        prepText.text = "Set";
                        break;

                    case 1:
                        prepText.text = "Fill !";
                        break;
                }
            }

            yield return new WaitForSecondsRealtime(1);
            countdown--;
        }

        // Set timescale back to normal after countdown
        Time.timeScale = 1;
        Debug.Log("Game Start!");

        prepWrapper.SetActive(false);

        // Game is now ready...
        BgmManager.Instance.PlayStageTheme(currentStageInfo.BgmIndex);

        InitializeGameTimer();
    }

    private void InitializeGameTimer()
    {
        //
        // This event is fired when the timer reached zero but 
        // before ending the timer.
        //
        gameTimer.OnTimerBeforeFlash = () => {

            gameResult = GameResults.Failed;
            BgmManager.Instance.StopBgm();
        };
        gameTimer.OnTimerFinished = () =>
        {                
            if (!IsLevelPartiallyFinished)
                SetGameFailed();
        };

        //
        // Setup the timer, give it a duration in seconds,
        // then start the countdown by calling begin()
        //
        //gameTimer.SetDuration( currentStageInfo.StageTime );
        gameTimer.SetSeconds(currentStageInfo.StageTime);
        gameTimer.Begin();
    }

    //
    // This will calculate the total stars earned after finishing a stage.
    // "Finishing a stage" means reaching "Success".
    //
    private void CalculateStarsAttained()
    {
        // To get a full-three star, finish the game in the given minimum time
        if (secondsFinished <= currentStageInfo.MinStageTime)
            currentStarsEarned = 3;

        // To get 2 - stars, finish within the given minimum time
        else if (secondsFinished > currentStageInfo.MinStageTime && secondsFinished <= currentStageInfo.MaxStageTime)
            currentStarsEarned = 2;

        else
            currentStarsEarned = 1;
    }

    /// <summary>
    /// This will calculate the total rewards earned after finishing a stage.
    /// "Finishing a stage" means reaching "Success".
    ///
    /// The rules for giving the reward are as follows:
    /// 1. The actual reward will only be given once and if hasn't been claimed yet.
    /// 2. Actual rewards must only be given when player earned 3 stars.
    /// 3. If the player reaches "Success" but has already claimed the reward,
    ///    he will be award 1x Silver coin as a consolation price.
    /// </summary>
    /// <param name="rewardType"></param>
    /// <param name="rewardAmount"></param>
    /// <returns></returns>
    private bool ClaimReward
    (
        out RewardType  rewardType, 
        out int  rewardAmount
    )
    {
        // The actual reward
        if (!currentStageInfo.RewardClaimed && currentStarsEarned == 3)
        {
            rewardType   = currentStageInfo.RewardType;
            rewardAmount = currentStageInfo.RewardAmount;

            currentRewardAmountEarned = currentStageInfo.RewardAmount;
            currentRewardTypeEarned   = currentStageInfo.RewardType;

            return false;    // Original reward NOT YET claimed
        }

        // Consolation Price
        rewardType   = RewardType.Coin;
        rewardAmount = 1;
        
        currentRewardAmountEarned = 1;
        currentRewardTypeEarned   = RewardType.Coin;

        return true;       // Original reward CLAIMED
    }

    /// <summary>
    /// We only perform the saving when the player reaches "Success"
    /// </summary>
    /// <returns>CoRoutine</returns>
    private IEnumerator SaveProgress()
    {
        // Update the stars earned from currently loaded stage info.
        // We will only save the total stars only if it is higher than the
        // last stars from the last stars we earned.
        if (currentStarsEarned >= currentStageInfo.StarsAttained)
            currentStageInfo.StarsAttained = currentStarsEarned;

        yield return StartCoroutine
        (
            PlayerProgressHelper.Instance.UnlockNextStage
            (
                sessionData.SelectedLevel,
                currentStageInfo.StageNumber
            )
        );

        yield return StartCoroutine
        (
            PlayerProgressHelper.Instance.IncreasePlayerBank
            (
                currentRewardTypeEarned,
                currentRewardAmountEarned,
                true
            )
        );
       
        yield return StartCoroutine 
        ( 
            lvlStructuresHelper.UpdateLevelProgress(currentStageInfo, sessionData.SelectedLevel) 
        );
    }

    //=========================================================
    // These are triggered by UI event, such as button click
    //=========================================================

    public void Retry()
    {
        BgmManager.Instance.StopBgm(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextStage()
    {
        if (isFinalStage)
            return;

        var loadScene = lvlStructuresHelper.OpenStage(nextStageInfo, sessionData.SelectedLevel);

        if (!loadScene)
            lvlStructuresHelper.GoToMainMenu();
    }

    public void Pause()
    {
        BgmManager.Instance.PauseBgm();
        Time.timeScale = 0.0F;
    }

    public void Resume()
    {
        BgmManager.Instance.ResumeBgm();
        Time.timeScale = 1.0F;
    }

    public void ExitToMenu() => lvlStructuresHelper.GoToMainMenu();
}
