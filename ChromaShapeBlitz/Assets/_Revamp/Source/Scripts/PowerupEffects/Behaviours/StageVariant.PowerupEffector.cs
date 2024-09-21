using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class StageVariant : MonoBehaviour
{
    [SerializeField] private HintMarker hintMarker;

    private Material LIGHT_BLOCK_DEFAULT_MAT;
    private Material DARK_BLOCK_DEFAULT_MAT;

    private bool blocksAlreadyShown;

    public void CacheDefaultBlockMaterialReferences(Material light, Material dark)
    {
        LIGHT_BLOCK_DEFAULT_MAT = light;
        DARK_BLOCK_DEFAULT_MAT  = dark;
    }

    // # EVENT OBSERVER #
    public void ObservePowerupReceived(HotBarSlot sender, PowerupEffectData effectData)
    {
        switch (effectData.Category)
        {
            case PowerupCategories.SpecialVision:
                HandleSpecialVisionEffects(sender, effectData);
                break;

            case PowerupCategories.StageSolver:
                HandleStageSolverEffects(sender, effectData);
                break;
        }
    }
    
    #region SPECIAL_VISION_POWERUP_EFFECTS

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

    private void HandleSpecialVisionEffects(HotBarSlot sender, PowerupEffectData effectData)
    {
        if (blocksAlreadyShown)
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
            // Reveal just the guide blocks
            case Constants.PowerupEffectValues.POWERUP_EFFECT_VISOR:

                var visor = Constants.PowerupEffectValues.POWERUP_EFFECT_VISOR;
                SpecialVisionPowerupNotifier.NotifyObserver(visor);
                StartCoroutine(IERevealGuideBlocks(totalBlocksPerSequence));
                PowerupEffectAppliedNotifier.NotifyObserver(sender, effectData);

                break;

            // Reveal both Pattern frame and guide blocks
            case Constants.PowerupEffectValues.POWERUP_EFFECT_XRAY:

                var xray = Constants.PowerupEffectValues.POWERUP_EFFECT_XRAY;
                SpecialVisionPowerupNotifier.NotifyObserver(xray);
                StartCoroutine(IERevealAll(totalBlocksPerSequence));
                // Notifier must be handled in PatternTimer.PowerupEffects

                break;
        }
    }

    #endregion SPECIAL_VISION_POWERUP_EFFECTS

    #region STAGE_SOLVER_EFFECTS
    
    private void HandleStageSolverEffects(HotBarSlot sender, PowerupEffectData effectData)
    {
        switch(effectData.EffectValue)
        {
            // Solve the first 3 patterns
            case Constants.PowerupEffectValues.POWERUP_EFFECT_WIZARD:
                Debug.Log("Solve the first 3 patterns");
                break;

            // Solve the entire pattern
            case Constants.PowerupEffectValues.POWERUP_EFFECT_GRANDMASTER:
                Debug.Log("Solve the entire pattern");
                break;

            // Show which tiles to tap
            case Constants.PowerupEffectValues.POWERUP_EFFECT_IDEA:
                
                // We can only show hints when we haven't touched the tiles yet (for now)
                if (sequenceFilled > 0)
                    return;

                HintMarkerNotifier.NotifyObserver(this);
                
                break;
        }
    }

    /*
    private Vector2 WorldPosToCanvasCoords(Transform target)
    {
        // Get the world position of the gameobject
        Vector3 cubeWorldPosition = target.position;

        // Convert the cube's world position to screen space
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(cubeWorldPosition);

        // Convert screen position to canvas coordinates
        RectTransform canvasRectTransform = uiImage.canvas.GetComponent<RectTransform>();
        Vector2 uiPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPosition, Camera.main, out uiPosition);

        // Set the UI Image position in the canvas
        RectTransform uiImageRectTransform = uiImage.GetComponent<RectTransform>();
        uiImageRectTransform.anchoredPosition = uiPosition;
    }
    */
    #endregion STAGE_SOLVER_EFFECTS
}