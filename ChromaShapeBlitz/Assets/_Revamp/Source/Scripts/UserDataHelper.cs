﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserDataHelper : MonoBehaviour
{
    #region SINGLETON
    public static UserDataHelper Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
            Destroy(gameObject);
    }
    #endregion SINGLETON

    // This IO class handles the reading and saving of binary data
    private UserDataIO userDataIO;
    private UserData deserializedUserData;

    public UserData GetLoadedData() => deserializedUserData;

    public IEnumerator LoadUserData(Action<UserData> callback = null)
    {
        userDataIO = UserDataIO.Instance;

        if (!userDataIO.FileExists)
        {
            deserializedUserData = SeedDefault();
            yield return StartCoroutine(userDataIO.Write
            (
                deserializedUserData
            ));
            
            File.WriteAllText(Application.persistentDataPath + "/user.sav.json", JsonUtility.ToJson(deserializedUserData));
        }
        else
        {
            yield return StartCoroutine(userDataIO.Read
            (
                u => deserializedUserData = u
            ));
        }

        callback?.Invoke(deserializedUserData);

        yield return null;
    }

    public IEnumerator SaveUserData(UserData input, Action<UserData> callback = null)
    {
        userDataIO = UserDataIO.Instance;

        yield return StartCoroutine(userDataIO.Write(input));

        deserializedUserData = input;
        callback?.Invoke(deserializedUserData);
        File.WriteAllText(Application.persistentDataPath + "/user.sav.json", JsonUtility.ToJson(input));
        yield return null;
    }

    /// <summary>
    /// Seed the user data with starting values
    /// </summary>
    /// <returns>Default UserData</returns>
    public UserData SeedDefault()
    {
        int[] defaultBlockSkinIds = { 1,2,3,4,5,6 };
        var initialDate = DateTime.MinValue.ToString("o");

        var userData = new UserData
        {
            CurrentTutorialStep  = TutorialSteps.STEP1_BASICS,
            CurrentTutorialStage = 0,
            IsTutorialCompleted  = false,

            HighestEasyStage    = 0,
            HighestNormalStage  = 0,
            HighestHardStage    = 0,

            StageProgressEasy   = new List<StageProgress>(),
            StageProgressNormal = new List<StageProgress>(),
            StageProgressHard   = new List<StageProgress>(),

            EasyStageUnlocked   = false,
            NormalStageUnlocked = false,
            HardStageUnlocked   = false,

            TotalCoins          = 0,
            TotalGems           = 0,

            OwnedBlockSkinIDs   = new List<int>(defaultBlockSkinIds),
            ActiveBlockSkins    = new() 
            {
                Blue    = 1,
                Green   = 2,
                Magenta = 3,
                Orange  = 4,
                Purple  = 5,
                Yellow  = 6
            },

            ActiveBackgroundID = 0,
            OwnedBackgroundIds = new List<int>{ 0 },

            SequenceFillRate = 1,
            CoinMultiplier   = 1,
            GemMultiplier    = 1,

            RemainingEMPUsage    = 0,
            SlotMachineSpinsLeft = 3,
            NextAllowedSpinTime  = initialDate, //DateTime.MinValue.ToString("o"),

            NextDailyGiftTime     = initialDate, //DateTime.Now.AddHours(24).ToString("o"),//initialDate,
            DailyGiftClaimed      = default,
            DailyGiftDayNumber    = 1,
            DailyGiftClaimHistory = new(),

            // Use this as the default theme.
            // We can only use other themes after
            // completing (or skipping) the tutorials
            MainMenuTheme = MainMenuThemeIdentifier.Stock
        };

        for (var i = 1; i <= Revamp.GameManager.TotalEasyStages; i++)
        {
            // Easy Stages are upto 50 max
            var stageData = new StageProgress { StageNumber = i };
            userData.StageProgressEasy.Add(stageData);

            // Normal Stages are upto 30 max
            if (i <= Revamp.GameManager.TotalNormalStages)
                userData.StageProgressNormal.Add(stageData);

            // Hard Stages are upto 25 max
            if (i <= Revamp.GameManager.TotalHardStages)
                userData.StageProgressHard.Add(stageData);
        }

        var settings = SettingsManager.Instance;

        if (settings != null)
        {
            settings.EnableSfx();
            settings.EnableBgm();
        }

        return userData;
    }
}
