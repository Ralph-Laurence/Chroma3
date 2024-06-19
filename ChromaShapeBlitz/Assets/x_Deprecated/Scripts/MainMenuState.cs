[System.Obsolete]
public class MainMenuState
{
    /* #region SINGLETON */
    //========================================================
    //
    private static MainMenuState instance;

    private MainMenuState() { }

    public static MainMenuState Instance
    {
        get
        {
            if (instance == null)
                instance = new MainMenuState();

            return instance;
        }
    }
    //
    //========================================================
    /* #endregion */
    //

    public bool IsSplashAlreadyShown { get; set; }
}