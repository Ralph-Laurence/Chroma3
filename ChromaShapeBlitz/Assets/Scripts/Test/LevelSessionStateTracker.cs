public class LevelSessionData
{
    public StageInfo SelectedStage;
    public GameLevels SelectedLevel;
}

public class LevelSessionStateTracker
{
    /* #region SINGLETON */
    //========================================================
    //
    private static LevelSessionStateTracker instance;

    private  LevelSessionStateTracker() { }

    public static  LevelSessionStateTracker Instance
    {
        get
        {
            if (instance == null)
                instance = new LevelSessionStateTracker();

            return instance;
        }
    }
    //
    //========================================================
    /* #endregion */
    //

    private LevelSessionData levelSessionData;
    public void SetSessionData(LevelSessionData sessionData) => levelSessionData = sessionData;
    public LevelSessionData GetSessionData() => levelSessionData;

    public void ClearSession() => levelSessionData = null;
}