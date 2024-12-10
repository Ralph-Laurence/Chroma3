using UnityEngine;

public class UserDataIO : BinarySerializer<UserData>
{
    //
    // Begin Singleton
    //
    private static UserDataIO instance;

    private UserDataIO() { }

    public static UserDataIO Instance
    {
        get
        {
            if (instance == null)
                instance = new UserDataIO();

            return instance;
        }
    }
    //
    // End Singleton
    //
    
    // protected override string BinaryPath => $"{Application.persistentDataPath}/user.sav";

}