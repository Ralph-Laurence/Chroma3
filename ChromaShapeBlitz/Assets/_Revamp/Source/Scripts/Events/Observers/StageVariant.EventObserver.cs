using System.Collections.Generic;
using Revamp;
using UnityEngine;

public partial class StageVariant : MonoBehaviour
{
    void OnEnable() => OnBlockSequenceFillCompleted.BindEvent(Subscribe);
    void OnDisable() => OnBlockSequenceFillCompleted.UnbindEvent(Subscribe);

    // Flag to track how many sequences have been completed
    private int sequenceCompleted = 0;

    // Listen to an event when a block sequence controller has completed filling.
    public void Subscribe()
    {
        sequenceCompleted++;

        // When all sequences have been completed, notify the game manager
        if (sequenceCompleted == SequenceSet.Count)
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
}