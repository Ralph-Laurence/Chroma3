[System.Obsolete]
public class PlayerProgressIO : BinarySerializer<PlayerProgress>
{
    //
    // Begin Singleton
    //
    private static PlayerProgressIO instance;

    private PlayerProgressIO() { }

    public static PlayerProgressIO Instance
    {
        get
        {
            if (instance == null)
                instance = new PlayerProgressIO();

            return instance;
        }
    }
    //
    // End Singleton
    //
    
    protected override string BinaryPath => Constants.DataPaths.PlayerProgress;
}