using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public struct StageProgress
{
    public int StageNumber;
    public int StarsAttained;
    public bool FullRewardEarned;
    public bool PartialRewardEarned;
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

    public bool EasyStageUnlocked;
    public bool NormalStageUnlocked;
    public bool HardStageUnlocked;

    //===========================
    // OTHER DATA
    //===========================
    // public SettingsData UserSettings;
}