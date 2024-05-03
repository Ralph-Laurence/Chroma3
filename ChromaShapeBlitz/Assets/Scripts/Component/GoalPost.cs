using System.Collections.Generic;
using UnityEngine;

public class GoalPost : MonoBehaviour
{
    [SerializeField] private Block[] blockSequence;

    public List<GameObject> ValidateBlockSequence()
    {
        var incorrectBlocks = new List<GameObject>();

        foreach (var block in blockSequence)
        {
            if (block != null && block.GetColor() != block.GetRequiredColor())
                incorrectBlocks.Add(block.gameObject);
        }

        return incorrectBlocks;
    }
}
