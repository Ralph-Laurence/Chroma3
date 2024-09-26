using System.Collections.Generic;
using Revamp;
using UnityEngine;

public partial class StageVariant : MonoBehaviour
{
    // Flag to track how many sequences have been filled
    private int sequenceFilled = 0;

    void OnEnable()
    {
        OnBlockSequenceFillCompleted.BindEvent(ObserveSequenceFilled);
        HotbarPowerupEffectNotifier.BindObserver(ObservePowerupReceived);
        OnBlockSequenceFillCompleted.BindEvent(ObserveSequencesFilled);
    }
    void OnDisable()
    {
        OnBlockSequenceFillCompleted.UnbindEvent(ObserveSequenceFilled);
        OnBlockSequenceFillCompleted.UnbindEvent(ObserveSequencesFilled);
    }

    /// <summary>
    /// Check if one or more of the sequences have been filled.
    /// </summary>
    public bool SequenceFillBegan => sequenceFilled > 0;

    // Listen to an event when a block sequence controller has completed filling.
    public void ObserveSequenceFilled()
    {
        sequenceFilled++;

        // When all sequences have been completed, notify the game manager
        if (sequenceFilled == SequenceSet.Count)
            ValidateSequenceSet();
    }

    private void ValidateSequenceSet()
    {
        // Find all block sequences with incorrect colors
        var incorrectSequences = new List<Block>();

        foreach (var sequence in SequenceSet)
        {
            var validation = sequence.ValidateSequence();

            if (validation.Count > 0)
                incorrectSequences.AddRange(validation);
        }

        // If there were incorrect blocks, change their material to appear as red "X"
        if (incorrectSequences.Count > 0)
        {
            incorrectSequences.ForEach(block => block.ApplyMaterial(IncorrectBlockMaterial, ColorSwatches.None));

            OnStageCompleted.NotifyObserver(StageCompletionType.Failed);
            return;
        }

        OnStageCompleted.NotifyObserver(StageCompletionType.Success);
    }

    private void ObserveSequencesFilled()
    {
        StageFillBegan = true;
        OnBlockSequenceFillCompleted.UnbindEvent(ObserveSequencesFilled);
    }

    public bool StageFillBegan {get; private set;}
}