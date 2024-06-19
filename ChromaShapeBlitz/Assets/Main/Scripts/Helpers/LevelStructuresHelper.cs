using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is responsible for how a stage or level behaves.
/// This also wraps the functionality for saving and loading 
/// level progress such as how many stars were earned from
/// each stage or which stage number to unlock and so on.
/// </summary>
public class LevelStructuresHelper : MonoBehaviour
{
    /* #region SINGLETON */
    //========================================================
    //
    private static LevelStructuresHelper instance;

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

    public static LevelStructuresHelper Instance => instance;
    //
    //========================================================
    /* #endregion */
    //
    //
    //
    /* #region ENTRY POINT COMPONENT INITIALIZATION */
    //========================================================
    //
    [SerializeField] private TextAsset levelStructuresTemplate;

    private LevelStructuresIO levelStructsDataIO;
    private LevelStructures levelStructures;

    /// <summary>
    /// Call this in the EntryPoint component
    /// </summary>
    /// <returns>Coroutine</returns>
    public IEnumerator LoadLevelStructures()
    {
        levelStructsDataIO = LevelStructuresIO.Instance;

        // Check if there is an existing binary game data, then load it
        if (levelStructsDataIO.FileExists)
        {
            yield return StartCoroutine(levelStructsDataIO.Read((data) => levelStructures = data));
        }
        // Otherwise create a new binary which was loaded from json.
        // We can assume that this is a fresh run.
        else
        {
            yield return StartCoroutine(levelStructsDataIO.ReadFromTemplate
            (
                templateAsset: levelStructuresTemplate,
                callback: (data) => levelStructures = data
            ));

            yield return StartCoroutine(levelStructsDataIO.Write(levelStructures));
        }

        // Non File IO initializaitons
        yield return StartCoroutine(PostInitialize());
        yield return null;
    }
    //
    //========================================================
    /* #endregion */


    /* #region BUSINESS LOGIC */
    //========================================================
    //
    public List<StageInfo> EasyStages { get; private set; }
    public List<StageInfo> NormalStages { get; private set; }
    public List<StageInfo> HardStages { get; private set; }

    private LevelSessionStateTracker lvlSessionStateTracker;

    private IEnumerator PostInitialize()
    {
        // Get a reference to the stages by difficulty
        EasyStages = levelStructures.Easy;
        NormalStages = levelStructures.Normal;
        HardStages = levelStructures.Hard;

        lvlSessionStateTracker = LevelSessionStateTracker.Instance;

        yield return null;
    }

    public IEnumerator UpdateLevelProgress(StageInfo stageInfo, GameLevels gameLevel)
    {
        var targetStageIndex = stageInfo.StageNumber - 1;

        if (targetStageIndex < 0)
            yield break;

        switch (gameLevel)
        {
            case GameLevels.Easy:
                levelStructures.Easy[targetStageIndex] = stageInfo;
                break;

            case GameLevels.Normal:
                levelStructures.Normal[targetStageIndex] = stageInfo;
                break;

            case GameLevels.Hard:
                levelStructures.Hard[targetStageIndex] = stageInfo;
                break;
        }

        yield return StartCoroutine(levelStructsDataIO.Write(levelStructures));
    }

    /// <summary>
    /// Begin playing the stage
    /// </summary>
    /// <param name="info">The properties for loading the stage</param>
    /// <param name="level">The difficulty</param>
    /// <returns>bool</returns>
    public bool OpenStage(StageInfo info, GameLevels level)
    {
        var targetScene = $"{GetStageNamePrefix(level)}{info.StageNumber}";

        // Check if the target scene exists
        if (!SceneExists(targetScene))
        {
            return false;
        }

        // We assume that the scene exists, and then we can store
        // the tracking data
        lvlSessionStateTracker.SetSessionData(new LevelSessionData
        {
            SelectedStage = info,
            SelectedLevel = level
        });

        // Otherwise load the target scene
        BgmManager.Instance.StopBgm(true);
        SceneManager.LoadScene(targetScene);

        // Call UnloadUnusedAssets to free up memory
        Resources.UnloadUnusedAssets();
        return true;
    }

    public string GetStageNamePrefix(GameLevels level)
    {
        var l = string.Empty;

        switch (level)
        {
            case GameLevels.Easy:   l = Constants.StageNameEasy; break;
            case GameLevels.Normal: l = Constants.StageNameNormal; break;
            case GameLevels.Hard:   l = Constants.StageNameHard; break;
        }

        return l;
    }

    /// <summary>
    /// Checks if a scene with the given name or path exists and 
    /// is loaded, or if it can be streamed and loaded.
    /// </summary>
    /// <param name="sceneName">The target scene name</param>
    /// <returns>bool</returns>
    public bool SceneExists(string sceneName)
    {
        return SceneManager.GetSceneByName(sceneName).isLoaded || 
               SceneManager.GetSceneByPath(sceneName).isLoaded || 
               Application.CanStreamedLevelBeLoaded(sceneName);
    }

    /// <summary>
    /// Opens the main menu screen
    /// </summary>
    public void GoToMainMenu()
    {
        BgmManager.Instance.StopBgm(true);
        SceneManager.LoadScene(Constants.Scenes.MainMenu);
    }
    //========================================================
    /* #endregion */
}