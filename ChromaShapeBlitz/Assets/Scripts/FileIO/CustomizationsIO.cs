public class CustomizationsIO : BinarySerializer<Customizations>
{
    //============================================
    // Begin Singleton
    //============================================
    private static CustomizationsIO instance;

    private CustomizationsIO() { }

    public static CustomizationsIO Instance
    {
        get
        {
            if (instance == null)
                instance = new CustomizationsIO();

            return instance;
        }
    }

    //...........................................
    // End Singleton
    //...........................................
    protected override string BinaryPath => Constants.DataPaths.CustomizationsData;
}