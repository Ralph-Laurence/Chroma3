using System.Collections;
using UnityEngine;

public class PlayerProgressHelper : MonoBehaviour
{
    /* #region SINGLETON */
    //========================================================
    //
    private static PlayerProgressHelper instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public static PlayerProgressHelper Instance => instance;
    //
    //========================================================
    /* #endregion */

    private PlayerProgressIO progressIO;
    private PlayerProgress   deserializedData;

    public IEnumerator LoadPlayerData()
    {
        // Get a reference to the Singleton instance of the PlayerProgressIO.
        // This handles the saving and loading of binary theme data.
        progressIO = PlayerProgressIO.Instance;

        if (!progressIO.FileExists)
        {
            deserializedData = CreateDefaultData();

            yield return StartCoroutine(progressIO.Write(deserializedData));
        }
        else
        {
            yield return StartCoroutine(progressIO.Read((d) => deserializedData = d));
        }

        yield return null;
    }

    /// <summary>
    /// Create a fresh theme data with everything set to default
    /// </summary>
    /// <returns>New theme data</returns>
    private PlayerProgress CreateDefaultData() => new PlayerProgress
    {
        HighestStageEasy    = 1,
        HighestStageNormal  = 1,
        HighestStageHard    = 1,
        CurrentCoins        = 320,
        CurrentGems         = 280
    };

    // public void SetProgressData(PlayerProgress progressData) => deserializedData = progressData;
    public PlayerProgress GetProgressData() => deserializedData;

    public IEnumerator UnlockNextStage(GameLevels difficulty, int currentStage, bool commit = false)
    {
        switch (difficulty)
        {
            case GameLevels.Easy:
                if (deserializedData.HighestStageEasy == currentStage)
                    deserializedData.HighestStageEasy ++;
                break;

            case GameLevels.Normal:
                if (deserializedData.HighestStageNormal == currentStage)
                    deserializedData.HighestStageNormal ++;
                break;

            case GameLevels.Hard:
                if (deserializedData.HighestStageHard == currentStage)
                    deserializedData.HighestStageHard ++;
                break;
        }

        if (commit)
            yield return StartCoroutine(Commit());
    }

    public IEnumerator IncreasePlayerBank(RewardType rewardType, int amount, bool commit = false)
    {
        switch (rewardType)
        {
            case RewardType.Coin:
                deserializedData.CurrentCoins += amount;
                break;

            case RewardType.Gem:
                deserializedData.CurrentGems += amount;
                break;
        }

        if (commit)
            yield return StartCoroutine(Commit());
    }

    public IEnumerator DecreasePlayerBank(CurrencyType type, int amount, bool commit = false)
    {
        switch (type)
        {
            case CurrencyType.Coin:
                if (deserializedData.CurrentCoins > 0)
                    deserializedData.CurrentCoins -= amount;
                break;

            case CurrencyType.Gem:
                if (deserializedData.CurrentGems > 0)
                    deserializedData.CurrentGems -= amount;
                break;
        }

        if (commit)
            yield return StartCoroutine(Commit());
    }

    public IEnumerator Commit()
    {
        // Save the updated values into the binary data
        yield return StartCoroutine(progressIO.Write(deserializedData));
    }
}
