using System;
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
                deserializedUserData,
                StatusCodes.BEGIN_WRITE_USER_DATA,
                StatusCodes.DONE_WRITE_USER_DATA
            ));
        }
        else
        {
            yield return StartCoroutine(userDataIO.Read
            (
                u => deserializedUserData = u,
                StatusCodes.BEGIN_READ_USER_DATA,
                StatusCodes.DONE_READ_USER_DATA
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
        //File.WriteAllText(Application.persistentDataPath + "/test.json", JsonUtility.ToJson(input));
        yield return null;
    }

    private UserData SeedDefault()
    {
        int[] defaultBlockSkinIds = { 1,2,3,4,5,6 };

        var userData = new UserData
        {
            HighestEasyStage    = 1,
            HighestNormalStage  = 1,
            HighestHardStage    = 1,

            StageProgressEasy   = new List<StageProgress>(),
            StageProgressNormal = new List<StageProgress>(),
            StageProgressHard   = new List<StageProgress>(),

            EasyStageUnlocked   = true,
            NormalStageUnlocked = true,
            HardStageUnlocked   = true,

            TotalCoins          = 512,
            TotalGems           = 256,

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

            ActiveBackground = 0,
            OwnedBackgroundIds = new List<int>{ 0 }
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
