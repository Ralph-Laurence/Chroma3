using UnityEngine;

public class StageCreatedEventArgs
{
    public AudioClip StageBgm;
    public Sprite StagePattern;
    public LevelDifficulties StageLevel;
    public RewardTypes RewardType;
    public Transform StageTransform;

    public int StageNumber;

    public int TotalStageTime;
    public int MinStageTime;
    public int MaxStageTime;
    public int TotalReward;
}