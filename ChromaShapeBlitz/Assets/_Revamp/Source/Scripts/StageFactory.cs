
using Revamp;
using UnityEngine;

public class StageFactory : MonoBehaviour
{
    private StageVariant _createdStage;

    public void Create(LevelDifficulties difficulty, int stageNumber)
    {
        // Build the path to the folder containing the variants
        var prefabPath = $"Stages/{difficulty}/Stage_{stageNumber}";

        // Load all prefabs in the specified folder
        var variants = Resources.LoadAll<GameObject>(prefabPath);

        // Select a random prefab from the loaded prefabs
        var stage = variants[Random.Range(0, variants.Length)];

        Instantiate(stage).TryGetComponent(out _createdStage);

        OnStageCreated.NotifyObserver(new StageCreatedEventArgs
        {
            StageNumber     = stageNumber,
            StageLevel      = _createdStage.VariantDifficulty,
            StagePattern    = _createdStage.PatternObjective,
            StageBgm        = _createdStage.BgmClip,
            MinStageTime    = _createdStage.MinStageTime,
            MaxStageTime    = _createdStage.MaxStageTime,
            TotalStageTime  = _createdStage.TotalStageTime,
            RewardType      = _createdStage.RewardType,
            TotalReward     = _createdStage.TotalReward
        });
    }

    public void Clear()
    {
        if (_createdStage == null)
            return;

        Destroy(_createdStage.gameObject);
        _createdStage = null;
    }
    
    public StageVariant CreatedStage => _createdStage;
}