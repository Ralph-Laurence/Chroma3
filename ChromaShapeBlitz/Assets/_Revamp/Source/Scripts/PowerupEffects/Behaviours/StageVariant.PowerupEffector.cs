using System.Collections;
using UnityEngine;

public partial class StageVariant : MonoBehaviour
{
    private Material LIGHT_BLOCK_DEFAULT_MAT;
    private Material DARK_BLOCK_DEFAULT_MAT;

    private const int SPEC_VSN_ONLY_BLOCKS = Constants.PowerupEffectValues.POWERUP_EFFECT_VISOR;  // Special Vision for showing only the guide blocks
    private const int SPEC_VSN_REVEAL_ALL  = Constants.PowerupEffectValues.POWERUP_EFFECT_XRAY;   // Special Vision to show both guide blocks and patterns

    private bool blocksAlreadyShown;

    public void CacheDefaultBlockMaterialReferences(Material light, Material dark)
    {
        LIGHT_BLOCK_DEFAULT_MAT = light;
        DARK_BLOCK_DEFAULT_MAT  = dark;
    }

    public void ObservePowerupReceived(HotBarSlot sender, PowerupEffectData effectData)
    {
        if (blocksAlreadyShown || effectData.Category != PowerupCategories.SpecialVision)
            return;

        blocksAlreadyShown = true;

        var totalBlocksPerSequence = 0;

        for (var i = 0; i < SequenceSet.Count; i++)
        {
            var blocks = SequenceSet[i].BlockSequence.Count;

            for (var k = 0; k < blocks; k++)
                totalBlocksPerSequence++;
        }
        
        switch (effectData.EffectValue)
        {
            case SPEC_VSN_ONLY_BLOCKS:

                var visor = Constants.PowerupEffectValues.POWERUP_EFFECT_VISOR;
                SpecialVisionPowerupNotifier.NotifyObserver(visor);
                StartCoroutine(IERevealGuideBlocks(totalBlocksPerSequence));
                PowerupEffectAppliedNotifier.NotifyObserver(sender, effectData);

                break;

            case SPEC_VSN_REVEAL_ALL:
                
                var xray = Constants.PowerupEffectValues.POWERUP_EFFECT_XRAY;
                SpecialVisionPowerupNotifier.NotifyObserver(xray);
                StartCoroutine(IERevealAll(totalBlocksPerSequence));
                // Notifier must be handled in PatternTimer.PowerupEffects
                
                break;
        }
    }

    private IEnumerator IERevealGuideBlocks(float totalBlocksPerSequence)
    {
        var duration = Constants.PowerupEffectValues.VISOR_SCAN_DURATION;
        var delay    = new WaitForSeconds( duration / totalBlocksPerSequence );

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

                yield return delay;
            }

            yield return null;
        }

        yield return null;
    }

    private IEnumerator IERevealAll(float totalBlocksPerSequence)
    {
        // The pattern timer listens to this type of event.
        // We can pass "null" because 
        HotbarPowerupEffectNotifier.NotifyObserver(null, new PowerupEffectData
        {
            EffectValue = Constants.PowerupEffectValues.POWERUP_EFFECT_XRAY,
            Category    = PowerupCategories.PatternReveal
        });

        yield return StartCoroutine(IERevealGuideBlocks(totalBlocksPerSequence));
    }
}