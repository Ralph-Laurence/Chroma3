using System.Collections;
using UnityEngine;

public partial class StageVariant : MonoBehaviour
{
    private Material LIGHT_BLOCK_DEFAULT_MAT;
    private Material DARK_BLOCK_DEFAULT_MAT;

    private const int SPEC_VSN_ONLY_BLOCKS = 0;  // Special Vision for showing only the guide blocks
    private const int SPEC_VSN_REVEAL_ALL = 1;   // Special Vision to show both guide blocks and patterns

    public void CacheDefaultBlockMaterialReferences(Material light, Material dark)
    {
        LIGHT_BLOCK_DEFAULT_MAT = light;
        DARK_BLOCK_DEFAULT_MAT  = dark;
    }

    public void ObservePowerupReceived(HotBarSlot sender, PowerupEffectData effectData)
    {
        if (effectData.Category != PowerupCategories.SpecialVision)
            return;

        switch (effectData.EffectValue)
        {
            case SPEC_VSN_ONLY_BLOCKS:

                StartCoroutine(IERevealGuideBlocks());

                break;

            case SPEC_VSN_REVEAL_ALL:

                break;
        }
    }

    private IEnumerator IERevealGuideBlocks()
    {
        for (var s = 0; s < SequenceSet.Count; s++)
        {
            // Collection of blocks in the sequence
            var blocks = SequenceSet[s].BlockSequence;

            // For each of those blocks in the sequence,
            // determine if the block hasn't been colored yet.
            // Then decide which guide color it must have (ie. white or gray).
            // If it has already been given a color (ie. Color != None),
            // we skip it.
            for (var b = 0; b < blocks.Count; b++)
            {
                var block = blocks[b];

                block.RevealGuideColor(LIGHT_BLOCK_DEFAULT_MAT, DARK_BLOCK_DEFAULT_MAT);
                Debug.Log($"Current Color -> {block.GetColor()}");

                yield return null;
            }

            yield return null;
        }

        yield return null;
    }

    private IEnumerator IERevealAll()
    {
        yield return null;
    }
}