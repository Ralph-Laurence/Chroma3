using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public struct DimenGrid
{
    public int Rows;
    public int Columns;
}

public partial class StageFabricator : MonoBehaviour
{
    //
    //==============================================
    // Data and Behaviour
    //==============================================
    //
    private AudioSource audioPlayer;
    private Dictionary<string, AudioClip> bgmMap;
    private Dictionary<string, DimenGrid> gridDimensions;
    private Dictionary<string, RewardTypes> rewardTypes;

    private readonly string PatternsResPathEasy     = "Assets/_Revamp/Art/UI/Patterns/Easy"; //"UI/Patterns/Easy";
    private readonly string PatternsResPathNormal   = "Assets/_Revamp/Art/UI/Patterns/Normal";
    private readonly string PatternsResPathHard     = "Assets/_Revamp/Art/UI/Patterns/Hard";
    //
    //==============================================
    // Inspector Fields
    //==============================================
    //
    [Space(10)] [Header("Grid Templates")]
    [SerializeField] private GameObject lightBlock;
    [SerializeField] private GameObject darkBlock;
    [SerializeField] private GameObject stageTemplate;
    [SerializeField] private GameObject gridsTransform;

    [Space(10)] [Header("Filler Pads Fabrication")]
    [SerializeField] private GameObject fillTriggerObj_Blue;
    [SerializeField] private GameObject fillTriggerObj_Green;
    [SerializeField] private GameObject fillTriggerObj_Magenta;
    [SerializeField] private GameObject fillTriggerObj_Orange;
    [SerializeField] private GameObject fillTriggerObj_Purple;
    [SerializeField] private GameObject fillTriggerObj_Yellow;

    [Space(10)] [Header("Fabricator Level Design")]
    [SerializeField] private Image patternPreviewer;
    [SerializeField] private Text patternNameText;
    [SerializeField] private Sprite defaultPatternPreview;

    [Space(5)]
    [SerializeField] private Button showOnlyTriggersButton;
    [SerializeField] private TMP_Dropdown gridDimensionsSelect;
    [SerializeField] private TMP_Dropdown levelDifficultySelect;
    [SerializeField] private TMP_Dropdown levelNumberSelect;
    [SerializeField] private TMP_Dropdown levelVariantSelect;
    [SerializeField] private TMP_Dropdown bgmSelect;

    [Space(10)] 
    [SerializeField] private InputField inputStagePosX;
    [SerializeField] private InputField inputStagePosY;
    [SerializeField] private InputField inputMinOrtho;
    [SerializeField] private InputField inputMaxOrtho;
    [SerializeField] private InputField inputIsoCamTiltX;
    [SerializeField] private InputField inputStageTiltY;
    [SerializeField] private InputField inputStageViewLeft;
    [SerializeField] private InputField inputStageViewRight;

    [Space(10)] [Header("Mechanics")]
    [SerializeField] private InputField inputMaxStageTime;
    [SerializeField] private InputField inputMinStageTime;
    [SerializeField] private InputField inputTotalStageTime;

    [Space(10)]
    [SerializeField] private TMP_Dropdown rewardTypeSelect;
    [SerializeField] private InputField inputTotalReward;
    //
    //==============================================
    // :::::::::::: BEGIN MONOBEHAVIOUR ::::::::::::
    //==============================================
    //
    void Awake()
    {
        rewardTypes = new Dictionary<string, RewardTypes>
        {
            {"Coins", RewardTypes.Coins},
            {"Gems", RewardTypes.Gems}
        };

        TryGetComponent(out audioPlayer);

        BuildGridDimensionsSelectMenu();
        BuildStageNumberSelectMenu();
        BuildBGMSelectMenu();
        BuildDifficultySelectMenu();

        InitializeSequenceEditorData();
    }

    //
    //========================================================
    // ::::::::::::::::::: MAIN MECHANICS ::::::::::::::::::::
    //========================================================
    //
    private void BuildGridDimensionsSelectMenu()
    {
        gridDimensions = new Dictionary<string, DimenGrid>
        {
            { "3x3", new DimenGrid { Columns = 3, Rows = 3 } },
            { "3x4", new DimenGrid { Columns = 3, Rows = 4 } },
            { "3x5", new DimenGrid { Columns = 3, Rows = 5 } },
            { "4x4", new DimenGrid { Columns = 4, Rows = 4 } },
            { "5x4", new DimenGrid { Columns = 5, Rows = 4 } },
            { "5x5", new DimenGrid { Rows = 5, Columns = 5 } },
            { "6x6", new DimenGrid { Rows = 6, Columns = 6 } },
            { "8x8", new DimenGrid { Rows = 8, Columns = 8 } },
        };

        gridDimensionsSelect.options.Clear();
        
        foreach(var dimens in gridDimensions)
        {
            gridDimensionsSelect.options.Add(new TMP_Dropdown.OptionData(dimens.Key));
        }

        gridDimensionsSelect.value = 0;
        gridDimensionsSelect.RefreshShownValue();
    }

    private void BuildStageNumberSelectMenu()
    {
        levelNumberSelect.options.Clear();

        for (var i = 0; i < 50; i++)
        {
            var stageNumber = (i + 1).ToString();
            levelNumberSelect.options.Add(new TMP_Dropdown.OptionData(stageNumber));
        }

        levelNumberSelect.value = 0;
        levelNumberSelect.RefreshShownValue();
    }

    private void BuildBGMSelectMenu()
    {
        bgmMap       = new Dictionary<string, AudioClip>();
        var bgmNames = new List<string>();
        var bgms     = ReadAllBgmAssets();
        var textInfo = new CultureInfo("en-US",false).TextInfo;

        bgmSelect.options.Clear();

        foreach (var bgm in bgms)
        {
            var bgmName = bgm.name.Replace("_", " ");
            bgmName = textInfo.ToTitleCase(bgmName);
            
            // Skip Bgm names that do not contain number
            // and does not start with prefix "stage_upbeat"
            if (!bgmName.Any(char.IsDigit) && !bgmName.StartsWith("stage_upbeat_"))
            {
                Debug.Log($"SKip: {bgmName}");
                continue;
            }

            bgmMap.Add(bgmName, bgm);
            bgmNames.Add(bgmName);
        }

        bgmNames.OrderBy(b => {
            var parts  = b.Split(' ');
            var number = int.Parse(parts[^1]); // indexing ^1 = parts.Length-1
            return number;
        })
        .ToList()
        .ForEach(bgm => bgmSelect.options.Add(new TMP_Dropdown.OptionData(bgm)));

        bgmSelect.value = 0;
        bgmSelect.RefreshShownValue();
    }

    private AudioClip[] ReadAllBgmAssets()
    {
        var output = new List<AudioClip>();

        #if UNITY_EDITOR
        // Specify the path to the folder containing the audio clips
        string folderPath = "Assets/_Revamp/Art/Sounds/Bgm";

        // Find all assets in the specified folder
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { folderPath });

        // Loop through each asset and load it
        foreach (var guid in guids)
        {
            // Get the path of the asset
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Load the audio clip
            var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);

            if (audioClip != null)
                output.Add(audioClip);

            // if (audioClip != null)
            // {
            //     // Output the name of the audio clip to the console
            //     Debug.Log("Loaded Audio Clip: " + audioClip.name);
            // }
            // else
            // {
            //     Debug.LogError("Failed to load audio clip at path: " + assetPath);
            // }
        }
        #endif

        return output.ToArray();
    }

    private Sprite GetPattern(LevelDifficulties levelDifficulty, string levelNumber, string variant)
    {
        var patternsFolder = levelDifficulty switch
        {
            LevelDifficulties.Easy   => PatternsResPathEasy,
            LevelDifficulties.Normal => PatternsResPathNormal,
            LevelDifficulties.Hard   => PatternsResPathHard,
            _ => string.Empty,
        };

        Sprite pattern = default;

        #if UNITY_EDITOR
        var patternPath = $"{patternsFolder}/{levelDifficulty}_{levelNumber}/{variant}.png";
        pattern = AssetDatabase.LoadAssetAtPath<Sprite>(patternPath); // Resources.Load<Sprite>(patternPath);
        
        if (pattern == null)
        {
            Debug.LogWarning($"Unable to get pattern at: {patternPath}");
        }
        #endif
        
        return pattern;



        // var output = new List<Sprite>();

        // // Specify the path to the folder containing the audio clips
        // string folderPath = "Assets/_Revamp/Art/Sounds/Bgm";

        // // Find all assets in the specified folder
        // string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { folderPath });

        // // Loop through each asset and load it
        // foreach (var guid in guids)
        // {
        //     // Get the path of the asset
        //     var assetPath = AssetDatabase.GUIDToAssetPath(guid);

        //     // Load the audio clip
        //     var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);

        //     if (audioClip != null)
        //         output.Add(audioClip);
        // }
    }

    private void BuildDifficultySelectMenu()
    {
        levelDifficultySelect.ClearOptions();

        var diffs = Enum.GetNames(typeof(LevelDifficulties)).ToList();
        levelDifficultySelect.AddOptions(diffs);
    }

    private AudioClip GetSelectedBgm() 
    {
        var clipName = bgmSelect.options[bgmSelect.value].text;
        return bgmMap[clipName];
    }

    private void GenerateGrid(DimenGrid dimensions)
    {
        ResetGrids();

        // Populate the grid
        var startingSpawnPosX = -1.0F * (dimensions.Rows / 2.0F) + 0.5F;
        var startingSpawnPosY = -1.0F * (dimensions.Columns / 2.0F)  + 0.5F;
        
        // This is the non-zero indexed row number
        var rowNumberFriendly = dimensions.Rows; // Row number that begins at 1 instead of 0

        for (var row = 0; row < dimensions.Rows; row++)
        {
            // When a row is EVEN, the stripe begins at LIGHT; DARK if odd
            var stripe = (row % 2 == 0) ? 0 : 1;

            // This will be the parent object for each rows, to hold columns
            var rowObj = new GameObject($"Row_{rowNumberFriendly}").transform;
            rowObj.SetParent(gridsTransform.transform);
            rowObj.transform.SetAsFirstSibling();

            for (var col = 0; col < dimensions.Columns; col++)
            {
                GameObject blockObj;

                var spawnPosition = new Vector3
                (
                    startingSpawnPosX + col,
                    0.0F,
                    startingSpawnPosY
                );
                
                if (stripe % 2 == 0)  
                    blockObj = Instantiate(lightBlock, spawnPosition, Quaternion.identity);
                else
                    blockObj = Instantiate(darkBlock, spawnPosition, Quaternion.identity);

                blockObj.name = $"R{rowNumberFriendly}_C{col + 1}";
                blockObj.transform.SetParent(rowObj);

                blockObj.TryGetComponent(out Block blockComponent);

                if (blockComponent != null)
                {
                    blockComponent.RowIndex = rowNumberFriendly - 1;
                    blockComponent.ColumnIndex = col;
                }

                // Temporarily add an event system to each cube for editing purposes
                BindBlockEventTrigger(blockObj);
                
                stripe++;
            }
            rowNumberFriendly--;
            startingSpawnPosY++;
        }

        ApplyStageTemplateParams();
    }

    private bool IsGridEmpty() => gridsTransform.transform.childCount == 0;

    private void ApplyStageTemplateParams()
    {
        SelectedBlockMarkerNotifier.NotifyReset();
        
        // Selected option value
        var selectedDifficulty = levelDifficultySelect.options[levelDifficultySelect.value].text;

        // Actual enum representation
        var difficulty = (LevelDifficulties) Enum.Parse(typeof(LevelDifficulties), selectedDifficulty);

        var levelNum        = levelNumberSelect.options[levelNumberSelect.value].text;
        var levelVariant    = levelVariantSelect.options[levelVariantSelect.value].text;
        
        var levelNumber         = string.IsNullOrEmpty(levelNum) ? 0 : Convert.ToInt32(levelNum);
        var stageVariantName    = $"{selectedDifficulty}_{levelNum}_{levelVariant}";
        var selectedRewardType  = rewardTypeSelect.options[rewardTypeSelect.value].text;

        // Check if there is already a stage component attached to the stage template
        stageTemplate.TryGetComponent(out StageVariant stageVariant);

        // If none, add one
        if (stageVariant == null)
            stageVariant = stageTemplate.AddComponent(typeof(StageVariant)) as StageVariant;

        // ..:: Apply the values then save ::..

        stageVariant.VariantDifficulty  = difficulty;
        stageVariant.VariantName        = levelVariant;
        stageVariant.VariantNumber      = levelNumber;

        // Stage Design
        stageVariant.AutoFindCamera = true;
        stageVariant.BgmClip        = GetSelectedBgm();
        stageVariant.StageOffset    = new Vector2
        (
            inputStagePosX.ReadFloat(),
            inputStagePosY.ReadFloat()
        );

        stageVariant.ViewAngleLeft          = inputStageViewLeft.ReadFloat();
        stageVariant.ViewAngleRight         = inputStageViewRight.ReadFloat();
        stageVariant.MinCameraOrthoSize     = inputMinOrtho.ReadFloat();
        stageVariant.MaxCameraOrthoSize     = inputMaxOrtho.ReadFloat();

        // Game Mechanics
        stageVariant.PatternObjective       = GetPattern(difficulty, levelNum, levelVariant);
        stageVariant.TotalStageTime         = inputTotalStageTime.ReadInt();
        stageVariant.MaxStageTime           = inputMaxStageTime.ReadInt();
        stageVariant.MinStageTime           = inputMinStageTime.ReadInt();
        stageVariant.TotalReward            = inputTotalReward.ReadInt();
        
        stageVariant.RewardType             = rewardTypes[selectedRewardType];
        
        stageTemplate.transform.rotation    = Quaternion.Euler(Vector3.up * inputStageTiltY.ReadFloat());

        // Update previewer
        patternPreviewer.sprite = stageVariant.PatternObjective;
        patternNameText.text = stageVariantName;
    }

    private void BindBlockEventTrigger(GameObject blockObj)
    {
        blockObj.AddComponent(typeof(BoxCollider));

        var eventTrigger = blockObj.AddComponent(typeof(EventTrigger)) as EventTrigger;
        var entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick
        };

        entry.callback.AddListener((evt) =>
        {
            if (evt is PointerEventData pointerEventData)
            {
                if (pointerEventData.button == PointerEventData.InputButton.Left)
                {
                    // On left click
                    if (IsManualPainting)
                        FabricatorBlockClickedNotifier.NotifyPaint(blockObj);
                    else if (IsMultiDelete)
                        FabricatorBlockClickedNotifier.NotifyErase(blockObj);
                    else
                        FabricatorBlockClickedNotifier.Publish(blockObj, 0);
                }
                else if (pointerEventData.button == PointerEventData.InputButton.Right)
                {
                    // On right click
                    FabricatorBlockClickedNotifier.Publish(blockObj, 1);
                }
            }
        });

        eventTrigger.triggers.Add(entry);
    }

    private void ResetGrids()
    {
        // Clear selections
        SelectedBlockMarkerNotifier.NotifyReset();

        // Clear the grid
        while (!IsGridEmpty())
        {
            var child = gridsTransform.transform.GetChild(0);
            child.SetParent(null);
            
            Destroy(child.gameObject);
        }

        stageTemplate.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        
        // Reset previewer
        patternPreviewer.sprite = defaultPatternPreview;
        patternNameText.text = "PATTERN NAME";
    }

    private void ResetStageTemplate()
    {
        ResetGrids();

        stageTemplate.TryGetComponent(out StageVariant stageVariant);
       
        if (stageVariant != null)
            Destroy(stageVariant);
    }

    private void ResetFabricatorUI()
    {
        Ev_ResetMechanics();
        Ev_ResetLevelDesign();
    }
}
