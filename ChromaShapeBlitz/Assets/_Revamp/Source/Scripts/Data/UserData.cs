using System;
using System.Collections.Generic;

[Serializable]
public struct StageProgress
{
    public int StageNumber;
    public int StarsAttained;
    public bool FullRewardEarned;
}

[Serializable]
public struct ActiveBlockSkinIDs
{
    public int Blue;
    public int Green;
    public int Magenta;
    public int Orange;
    public int Purple;
    public int Yellow;
}

[Serializable]
public struct PowerupInventory
{
    public int PowerupID;
    public int CurrentAmount;
}

[Serializable]
public struct PlayerInventory
{
    public List<PowerupInventory> OwnedPowerups;    // Stored Powerups
    public List<int> EquippedPowerupIds;            // Inventory 3-bar slots
}

[Serializable]
public class UserData
{
    //===========================
    // PLAYER BALANCE
    //===========================
    public int TotalCoins;
    public int TotalGems;

    //===========================
    // TUTORIAL
    //===========================
    public bool IsTutorialCompleted;
    public int CurrentTutorialStep;

    //===========================
    // HIGHEST UNLOCKED STAGES
    //===========================
    public int HighestEasyStage;
    public int HighestNormalStage;
    public int HighestHardStage;

    //===========================
    // PROGRESS PER STAGE
    //===========================
    public List<StageProgress> StageProgressEasy;
    public List<StageProgress> StageProgressNormal;
    public List<StageProgress> StageProgressHard;

    public bool EasyStagesCompleted;
    public bool NormalStagesCompleted;
    public bool HardStagesCompleted;

    public bool EasyStageUnlocked;
    public bool NormalStageUnlocked;
    public bool HardStageUnlocked;

    //===========================
    // CUSTOMIZATIONS
    //===========================
    public List<int>            OwnedBlockSkinIDs;
    public ActiveBlockSkinIDs   ActiveBlockSkins;
    public List<int>            OwnedBackgroundIds;
    public int                  ActiveBackgroundID;

    //===========================
    // POWERUP EFFECTS
    //===========================
    public PlayerInventory Inventory;

    public int SequenceFillRate;
    public int CoinMultiplier;
    public int GemMultiplier;

    public int RemainingEMPUsage;
}