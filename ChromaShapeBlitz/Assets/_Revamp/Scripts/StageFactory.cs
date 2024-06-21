using System.Collections.Generic;
using Revamp;
using UnityEngine;

public class StageFactory : MonoBehaviour
{
    [SerializeField] List<Stage> easyStages;
    [SerializeField] List<Stage> normalStages;
    [SerializeField] List<Stage> hardStages;

    private Dictionary<string, Stage> easyStagesMap;
    private Dictionary<string, Stage> normalStagesMap;
    private Dictionary<string, Stage> hardStagesMap;

    void Awake()
    {
        if (easyStagesMap != null || normalStagesMap != null || hardStagesMap != null)
            return;

        easyStagesMap   = RegisterStage(easyStages);
        normalStagesMap = RegisterStage(normalStages);
        hardStagesMap   = RegisterStage(hardStages);

        normalStages    = null;
        hardStages      = null;
        easyStages      = null;
    }

    private Dictionary<string, Stage> RegisterStage(List<Stage> stageList)
    {
        if (stageList == null)
        {
            Debug.LogWarning("Failed to register blank stages");
            return default;
        }

        var stageMap = new Dictionary<string, Stage>();

        stageList.ForEach(stage => stageMap.Add(stage.Number.ToString(), stage));
        stageList.Clear();

        return stageMap;        
    }

    public void Create(LevelDifficulties difficulty, int stageNumber)
    {
        Stage selected;
        var str_stageNumber = stageNumber.ToString();

        switch (difficulty)
        {
            case LevelDifficulties.Easy:
                selected = easyStagesMap[str_stageNumber];
                break;

            case LevelDifficulties.Normal:
                selected = normalStagesMap[str_stageNumber];
                break;

            case LevelDifficulties.Hard:
                selected = hardStagesMap[str_stageNumber];
                break;

            default:
                selected = default;
                break;
        }

        var stageVariant = selected.PickRandom();

        OnStageCreated.NotifyObserver(new StageCreatedEventArgs
        {
            StageNumber     = stageNumber,
            StageLevel      = stageVariant.VariantDifficulty,
            StagePattern    = stageVariant.PatternObjective,
            StageBgm        = stageVariant.BgmClip,
            MinStageTime    = stageVariant.MinStageTime,
            MaxStageTime    = stageVariant.MaxStageTime,
            TotalStageTime  = stageVariant.TotalStageTime,
            RewardType      = stageVariant.RewardType,
            TotalReward     = stageVariant.TotalReward
        });
    }
}