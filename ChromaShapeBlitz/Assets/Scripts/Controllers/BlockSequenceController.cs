using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class BlockSequenceController : MonoBehaviour
{
    private GameManager gameManager;

    [Header("Main Behaviour")]
    [SerializeField] private Block       destination;
    [SerializeField] private Block[]     blockSequence;
    [SerializeField] private BlockColors fillColorValue;
    [SerializeField] private Material    fillMaterial;
    [SerializeField] private float       fillRate = 0.24F;

    private bool isFillingColor;

    [Header("Activator Object")]
    [SerializeField] private GameObject activator;
    [SerializeField] private float      fadeOutRate = 10.0F;

    private Renderer activatorRenderer;

    private Material activatorMaterialClone;
    private Material activatorSharedMaterial;
    private Color    activatorTargetColor;

    private bool isActivatorFading;

    private SfxManager sfxManager;

    private WaitForSeconds waitFillRate;

    private void Start()
    {
        waitFillRate = new WaitForSeconds(fillRate);

        if (SfxManager.Instance != null)
            sfxManager = SfxManager.Instance;

        // Find the game manager
        GameObject.FindWithTag(Constants.Tags.GameManager).TryGetComponent(out gameManager);

        activator.TryGetComponent(out activatorRenderer);

        // The shared material is the "Original" material attached.
        activatorSharedMaterial = activatorRenderer.material;

        // We must clone (copy) it to prevent the original from being modified.
        activatorMaterialClone  = new Material(activatorSharedMaterial);

        // This cloned (copied) material will then be used instead of the original.
        activatorRenderer.material = activatorMaterialClone;

        // Force transparency on the cloned material by setting the "alpha" (a) into zero
        activatorTargetColor = activatorMaterialClone.color;
        activatorTargetColor.a = 0.0F;
    }

    // Update is called once per frame
    void Update()
    {
        //
        // Fade out the color activator after clicking
        //
        if (isActivatorFading)
        {
            var currentColor = activatorMaterialClone.color;

            // The fading happens by setting its "alpha" Transparency to 0
            activatorMaterialClone.color = Color.Lerp(currentColor, activatorTargetColor, fadeOutRate * Time.deltaTime);

            // We cannot guarantee that Color.Lerp() will exactly reach "zero".
            // If the alpha is "near zero", we must set it to "zero" instead.
            // We then hide the activator after reaching zero by turning off 
            // its renderer component.
            if (currentColor.a <= 0.05F)
            {
                activatorRenderer.material.color = activatorTargetColor;
                activatorRenderer.enabled        = false;

                isActivatorFading = false;
            }
        }
    }

    /// <summary>
    /// Check each block in the sequence if it holds the correct color
    /// </summary>
    /// <returns>List of blocks with incorrect colors</returns>
    public List<Block> ValidateSequence()
    {
        var incorrectBlocks = new List<Block>();

        foreach (var block in blockSequence)
        {
            // Assume that the color is valid, then skip
            if (block.IsValidColor())
                continue;

            // Incorrect color assigned, add it to the list
            incorrectBlocks.Add(block);
        }

        return incorrectBlocks;
    }

    /// <summary>
    /// This should be called from a click event. This starts the coloring.
    /// </summary>
    public void FillSequence()
    {
        if (isFillingColor || gameManager.IsGameOver())
            return;

        StartCoroutine(ColorizeSequence());
    }

    /// <summary>
    /// Fill the colors of each blocks in the sequence.
    /// The visible color is assigned by ApplyColor().
    /// The color value is assigned with SetColor().
    /// </summary>
    private IEnumerator ColorizeSequence()
    {
        isFillingColor    = true;
        isActivatorFading = true;

        foreach (var block in blockSequence)
        {
            // Do not colorize (fill) the blocks when the game is over
            if (gameManager.IsGameOver())
            {
                yield break;
            }

            // Colorize the blocks then set its value
            block.SetColor(fillColorValue);
            block.ApplyMaterial(fillMaterial, fillColorValue);
            
            sfxManager.PlayBlockFilled();

            // Some blocks may have intersecting (crossing) destinations.
            // We must only check and notify for our target destination.
            if (block.IsDestinationBlock() && block == destination)
                OnDestinationReachedNotifier.Publish();

            // Delay before the next fill
            yield return waitFillRate;
        }
    }
}
