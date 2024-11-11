using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBlockFiller : MonoBehaviour
{
    [SerializeField] private ColorSwatches fillColorValue;
    [SerializeField] private Block[] sequence;
    [SerializeField] private Material fillMaterial;
    [SerializeField] private Material transparentMaterial;
    [SerializeField] private GameObject padObject;

    private bool fillBegan;

    private readonly WaitForSeconds fillRate = new(0.2F);

    private GameSessionManager gsm;
    private Material LIGHT_BLOCK_DEFAULT_MAT;
    private Material DARK_BLOCK_DEFAULT_MAT;

    void Awake()
    {
        gsm = GameSessionManager.Instance;    

        LIGHT_BLOCK_DEFAULT_MAT = gsm.BLOCK_MAT_LIGHT;
        DARK_BLOCK_DEFAULT_MAT = gsm.BLOCK_MAT_DARK;
    }

    public void BeginFillSequence()
    {
        if (fillBegan)
            return;
        
        fillBegan = true;
        padObject.SetActive(false);

        StartCoroutine(IEFillSequences());
    }

    private IEnumerator IEFillSequences()
    {
        for (var i = 0; i < sequence.Length; i++)
        {
            var block = sequence[i];
            block.SetColor(fillColorValue);
            block.ApplyMaterial(fillMaterial, fillColorValue);
            TutorialEventNotifier.NotifyObserver(TutorialEventNames.BlockFilled, null);

            yield return fillRate;
        }

        // Notify the observer that a sequence has been fully filled.
        // A fully filled sequence is different from a fully filled stage.
        TutorialEventNotifier.NotifyObserver(TutorialEventNames.SequenceFilled, null);
    }

    public void ValidateSequence(out List<Block> wrongBlocks)
    {
        wrongBlocks = new List<Block>();

        for (var i = 0; i < sequence.Length; i++)
        {
            var block = sequence[i];
            
            if (!block.IsValidColor())
                wrongBlocks.Add(block);
        }
    }

    public IEnumerator IEHideSequences()
    {
        var delay = new WaitForSeconds(0.1F);

        for (var i = 0; i < sequence.Length; i++)
        {
            var block = sequence[i];
            block.ApplyMaterial(transparentMaterial, ColorSwatches.None);

            yield return delay;
        }
    }

    public IEnumerator IEShowSequences()
    {
        var delay = new WaitForSeconds(0.1F);

        // For each of those blocks in the sequence,
        // determine if the block hasn't been colored yet.
        // Then decide which guide color it must have (ie. white or gray).
        // If it has already been given a color (ie. Color != None),
        // we skip it.
        for (var b = 0; b < sequence.Length; b++)
        {
            var block = sequence[b];

            block.RevealGuideColor(LIGHT_BLOCK_DEFAULT_MAT, DARK_BLOCK_DEFAULT_MAT);

            yield return delay;
        }
    }

    public ColorSwatches GetFillColor() => fillColorValue;
    
    public void ResetWithColor(Material light, Material dark)
    {
        for (var i = 0; i <= sequence.Length-1; i++)
        {
            var block = sequence[i];
            var fill  = block.DarkerFill ? dark : light;

            block.SetColor(ColorSwatches.None);
            block.ApplyMaterial(fill, ColorSwatches.None);
        }
    }
}