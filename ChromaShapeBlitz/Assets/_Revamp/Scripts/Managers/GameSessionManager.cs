using UnityEngine;

public struct CustomizationsSession
{
    public Material BlueBlockSkinMat;
    public Material GreenBlockSkinMat;
    public Material MagentaBlockSkinMat;
    public Material OrangeBlockSkinMat;
    public Material PurpleBlockSkinMat;
    public Material YellowBlockSkinMat;
}

public class GameSessionManager
{
    #region SINGLETON

    public static GameSessionManager instance;

    public static GameSessionManager Instance
    {
        get
        {
            if (instance == null)
                instance = new GameSessionManager();

            return instance;
        }
    }
    #endregion SINGLETON

    public CustomizationsSession Customizations;
    public LevelDifficulties SelectedDifficulty;
    public int SelectedStageNumber;
}