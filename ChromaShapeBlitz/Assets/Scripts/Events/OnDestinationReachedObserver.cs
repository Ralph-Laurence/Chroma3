using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will subscribe to the event notifier to listen for when a block
/// sequence has completed with their color filling (i.e. when the final
/// target block has been reached).
/// 
/// This script must be attached into a child gameobject of Game Manager
/// </summary>
public class OnDestinationReachedObserver : MonoBehaviour
{
    [SerializeField] private BlockSequenceController[]   sequenceControllers;
    [SerializeField] private Material                    incorrectBlockMat;

    private GameManager gameManager;

    private int notifyCount;

    private void OnEnable()  => OnDestinationReachedNotifier.Event.AddListener(Subscribe);

    private void OnDisable() => OnDestinationReachedNotifier.Event.RemoveListener(Subscribe);

    private void Start()
    {
        gameManager = GetComponentInParent<GameManager>();
    }

    public void Subscribe()
    {
        // For every block sequence that has completed color filling,
        // increase the notifyCount by 1. This will be used for tracking
        // the number of completed sequences. The total sequences is
        // determined by the total length of sequenceControllers array.
        notifyCount++;

        // When the notifyCount reached the total length of sequences,
        // begin the validation. We can safely assume that all block
        // sequences have completed their color filling.
        if (notifyCount == sequenceControllers.Length)
            ValidateSequences();
    }

    private void ValidateSequences()
    {
        // Tell the game manager that the block sequences are done
        // with their color filling and we will proceed with the
        // sequence validation. This will stop the game timer.
        // Thus, we can assume that the color filling was finished.
        gameManager.NotifySequencesCompleted();

        // Find all block sequences with incorrect colors
        var incorrectSequences = new List<Block>();

        foreach (var sequence in sequenceControllers)
        {
            var validation = sequence.ValidateSequence();

            if (validation.Count > 0)
                incorrectSequences.AddRange(validation);
        }

        // If there were incorrect blocks, change their material to appear as red "X"
        if (incorrectSequences.Count > 0)
        {
            incorrectSequences.ForEach(block => block.ApplyMaterial(incorrectBlockMat, BlockColors.None));

            gameManager.SetGameFailed();
            return;
        }

        // We assume that there are no incorrect color sequences.
        gameManager.SetGamePassed();
    }
}
