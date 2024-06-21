using System.Collections.Generic;
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

// public struct GsmPlayerData
// {
//     public List<int> OwnedBlockSkinIds;

//     public int HighestEasyStage;
//     public int HighestNormalStage;
//     public int HighestHardStage;

//     public bool EasyStageUnlocked;
//     public bool NormalStageUnlocked;
//     public bool HardStageUnlocked;

//     public int CurrentCoins;
//     public int CurrentGems;
// }

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

    #region GAME_LOGIC
    public CustomizationsSession Customizations = new CustomizationsSession();
    public LevelDifficulties SelectedDifficulty;
    public int SelectedStageNumber;
    public int LastRandomSpawnIndex;
    public int SelectedStageMaxTime;
    public int SelectedStageMinTime;
    
    //public UserData PlayerData;

    public void ClearSession()
    {
        SelectedDifficulty   = default;
        SelectedStageNumber  = default;
        LastRandomSpawnIndex = default;
        SelectedStageMaxTime = default;
        SelectedStageMinTime = default;
        //PlayerData           = default;
    }
    #endregion GAME_LOGIC
}