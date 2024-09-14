
using Revamp;
using UnityEngine;

public class StageFactory : MonoBehaviour
{
    private StageVariant _createdStage;
    private GameSessionManager gsm;
    private float fillRate;

    void Awake()
    {
        gsm = GameSessionManager.Instance;

        fillRate = Constants.BlockFillRates[gsm.UserSessionData.SequenceFillRate];
    }

    public void Create(LevelDifficulties difficulty, int stageNumber)
    {
        // Build the path to the folder containing the variants
        var prefabPath = $"Stages/{difficulty}/Stage_{stageNumber}";

        // Load all prefabs in the specified folder
        var variants = Resources.LoadAll<GameObject>(prefabPath);

        if (variants == null || variants?.Length == 0)
        {
            Debug.LogWarning("Stage prefab does not exist.");
            return;
        }

        // Select a random prefab from the loaded prefabs
        var stage = variants[Random.Range(0, variants.Length)];

        var stageObj = Instantiate(stage);
        stageObj.TryGetComponent(out _createdStage);

        _createdStage.SetFillRate(fillRate);

        // For powerups (ie. Reveal guide blocks)
        _createdStage.CacheDefaultBlockMaterialReferences
        (
            light: gsm.BLOCK_MAT_LIGHT,
            dark:  gsm.BLOCK_MAT_DARK
        );

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
            TotalReward     = _createdStage.TotalReward,
            StageTransform  = stageObj.transform
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