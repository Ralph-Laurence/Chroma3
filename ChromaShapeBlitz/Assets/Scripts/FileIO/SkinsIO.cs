public class BlockSkinsIO : BinarySerializer<CustomBlockSkins>
{
    //============================================
    // Begin Singleton
    //============================================
    private static BlockSkinsIO instance;

    private BlockSkinsIO() { }

    public static BlockSkinsIO Instance
    {
        get
        {
            if (instance == null)
                instance = new BlockSkinsIO();

            return instance;
        }
    }

    //...........................................
    // End Singleton
    //...........................................
    protected override string BinaryPath => Constants.DataPaths.CustomSkinsData;
}