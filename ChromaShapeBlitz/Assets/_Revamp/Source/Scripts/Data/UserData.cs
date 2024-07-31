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
public class UserData
{
    //===========================
    // PLAYER BALANCE
    //===========================
    public int TotalCoins;
    public int TotalGems;

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
    // OTHER DATA
    //===========================
    public List<int> OwnedBlockSkinIDs;
    public ActiveBlockSkinIDs ActiveBlockSkins;
    public List<int> OwnedBackgroundIds;
    public int ActiveBackgroundID;
}