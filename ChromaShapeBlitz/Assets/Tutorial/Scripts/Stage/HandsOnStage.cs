using System;
using System.Collections.Generic;
using UnityEngine;

public class HandsOnStage : ITutorialStage
{
    [SerializeField] private TutorialPatternTimer timer;

    public int LevelDuration;
    public Sprite PatternObjective;
    
    private GameSessionManager gsm;

    public bool StopBgmWhenComplete = true;
    
    void Awake()
    {   
        gsm = GameSessionManager.Instance;

        timer.OnTimesUp += () =>
        {
            TutorialEventNotifier.NotifyObserver(TutorialEventNames.GameOverFailed, null);
        };
    }

    public override void OnStageFillComplete()
    {
        base.OnStageFillComplete();
        timer.Stop();

        if (StopBgmWhenComplete)
            bgm.Stop();
    }

    public override void OnStageFailed(List<Block> wrongBlocks)
    {
        TutorialEventNotifier.NotifyObserver(TutorialEventNames.GameOverFailed, null);
    }

    public override void OnStagePassed()
    {
        TutorialEventNotifier.NotifyObserver(TutorialEventNames.GameOverSuccess, null);
    }

    public void BeginLevel()
    {
        timer.enabled = true;
        timer.Prepare(LevelDuration, PatternObjective);
        timer.Begin();
    }

    public void Reset()
    {
        for (var i = 0; i < sequenceFillers.Length; i++)
        {
            var sequence = sequenceFillers[i];
            sequence.ResetWithColor(gsm.BLOCK_MAT_LIGHT, gsm.BLOCK_MAT_DARK);
        }
    }
}
