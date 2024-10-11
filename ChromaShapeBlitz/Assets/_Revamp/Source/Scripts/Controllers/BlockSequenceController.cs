using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Revamp;
using System;

public class BlockSequenceController : MonoBehaviour
{
    private Renderer activatorRenderer;
    private Material activatorMaterialClone;
    private Material activatorSharedMaterial;
    private Color activatorTargetColor;
    private SoundEffects sfx;
    private WaitForSeconds waitFillRate;
    private EventTrigger eventTrigger;

    private bool isActivatorFading;
    private bool isFillingColor;
    //private bool isFillComplete;

    [Header("Main Behaviour")]
    public bool IsEditMode = true;
    public Block Destination;
    public List<Block> BlockSequence;

    public ColorSwatches FillColorValue;
    public Material FillMaterial;
    [SerializeField] private float fillRate = 0.24F;

    [Header("Filler Object")]
    public GameObject PadVisuals;
    [SerializeField] private float fadeOutRate = 10.0F;

    private Dictionary<ColorSwatches, Material> blockMaterialMap;
    private GameSessionManager gsm;

    private bool isGameOver;
    private bool isActivatorBegan;

    void OnEnable() => GameManagerStateNotifier.BindEvent(ObserveGameManagerState);
    void OnDisable() => GameManagerStateNotifier.UnbindEvent(ObserveGameManagerState);

    void Awake()
    {
        TryGetComponent(out eventTrigger);
        
        if (IsEditMode)
            return;

        InitializeComponent();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsEditMode)
            return;
        
        HandleTriggerPadFadeout();
    }

    private void ObserveGameManagerState(GameManagerStates state) => isGameOver = true;

    private void InitializeComponent()
    {
        waitFillRate = new WaitForSeconds(fillRate);

        sfx = SoundEffects.Instance;
        gsm = GameSessionManager.Instance;

        PadVisuals.TryGetComponent(out activatorRenderer);

        // The shared material is the "Original" material attached.
        activatorSharedMaterial = activatorRenderer.material;

        // We must clone (copy) it to prevent the original from being modified.
        activatorMaterialClone  = new Material(activatorSharedMaterial);

        // This cloned (copied) material will then be used instead of the original.
        activatorRenderer.material = activatorMaterialClone;

        // Force transparency on the cloned material by setting the "alpha" (a) into zero
        activatorTargetColor = activatorMaterialClone.color;
        activatorTargetColor.a = 0.0F;

        blockMaterialMap = new()
        {
            { ColorSwatches.Blue,       gsm.BlockSkinMaterialsInUse.BlueSkinMat     },
            { ColorSwatches.Green,      gsm.BlockSkinMaterialsInUse.GreenSkinMat    },
            { ColorSwatches.Magenta,    gsm.BlockSkinMaterialsInUse.MagentaSkinMat  },
            { ColorSwatches.Orange,     gsm.BlockSkinMaterialsInUse.OrangeSkinMat   },
            { ColorSwatches.Purple,     gsm.BlockSkinMaterialsInUse.PurpleSkinMat   },
            { ColorSwatches.Yellow,     gsm.BlockSkinMaterialsInUse.YellowSkinMat   },
        };
    }

    /// <summary>
    /// Fade out the color activator after clicking
    /// </summary>
    private void HandleTriggerPadFadeout()
    {
        if (isActivatorFading)
        {
            if (!isActivatorBegan)
            {
                activatorMaterialClone.RenderToTransparent();
                isActivatorBegan = true;
            }

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
                activatorRenderer.enabled = false;

                isActivatorFading = false;
            }
        }
    }

    /// <summary>
    /// Fill the colors of each blocks in the sequence.
    /// The visible color is assigned by ApplyColor().
    /// The color value is assigned with SetColor().
    /// 
    /// By default, (normal gameplay) this notifies the game manager that 
    /// the entire sequence has finished filling, thus notifyGameManager = true.
    /// When we use a powerup [i.e. GrandMaster] that solves the stage, we set the notifyGameManager to false
    /// </summary>
    private IEnumerator ColorizeSequence(bool notifyGameManager = true)
    {
        isFillingColor    = true;
        isActivatorFading = true;

        foreach (var block in BlockSequence)
        {
            // Do not colorize (fill) the blocks when the game is over
            if (isGameOver)
                yield break;

            // If we are not in fabricator edit mode, we should use
            // the materials equipped by player
            if (blockMaterialMap != null && !IsEditMode)
                FillMaterial = blockMaterialMap[FillColorValue];

            // Colorize the blocks then set its value
            block.SetColor(FillColorValue);
            block.ApplyMaterial(FillMaterial, FillColorValue);
            
            if (sfx != null)
                sfx.PlayBlockFill();

            // Some blocks may have intersecting (crossing) destinations.
            // We must only check and notify for our target destination.
            if (notifyGameManager && block.IsDestinationBlock && block == Destination)
                //OnDestinationReachedNotifier.Publish();
                OnBlockSequenceFillCompleted.NotifyObserver();

            // Delay before the next fill
            yield return waitFillRate;
        }

        // isFillComplete = true;
    }

    /// <summary>
    /// This should be called from a click event. This starts the coloring.
    /// </summary>
    public void FillSequence()
    {
        if (isFillingColor || isGameOver)
            return;

        StartCoroutine(ColorizeSequence());
    }

    /// <summary>
    /// This is used by Grandmaster powerup
    /// </summary>
    public IEnumerator CompleteSequence(Action sequenceFilled)
    {
        if (isGameOver)
            yield break;

        isActivatorFading = true;

        foreach (var block in BlockSequence)
        {
            // Do not colorize (fill) the blocks when the game is over
            if (isGameOver)
                yield break;

            //  Use the materials equipped by player
            if (blockMaterialMap != null)
                FillMaterial = blockMaterialMap[FillColorValue];

            // Colorize the blocks then set its value
            block.SetColor(FillColorValue);
            block.ApplyMaterial(FillMaterial, FillColorValue);
            
            if (sfx != null)
                sfx.PlayBlockFill();

            // Delay before the next fill
            yield return waitFillRate;
        }

        sequenceFilled?.Invoke();
    }

    /// <summary>
    /// This is used by Wizard powerup
    /// </summary>
    public IEnumerator FillFirstThreeSequence()
    {
        if (isFillingColor || isGameOver)
            yield break;

        yield return StartCoroutine(ColorizeSequence());
    }

    /// <summary>
    /// Check each block in the sequence if it holds the correct color
    /// </summary>
    /// <returns>List of blocks with incorrect colors</returns>
    public List<Block> ValidateSequence()
    {
        var incorrectBlocks = new List<Block>();

        foreach (var block in BlockSequence)
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
    /// Adjust the current fill rate. This is used effectively with powerups
    /// </summary>
    public void SetFillRate(float rate) => waitFillRate = new WaitForSeconds(rate);

    //============= USED ONLY DURING FABRICATION ================//

    // Notifies an event to when this object was clicked
    public void Ev_FabricatorNotifySelected(BaseEventData eventData)
    {
        if (eventTrigger == null)
        {
            Debug.LogWarning("No event trigger attached");
            return;
        }

        if (eventData is PointerEventData pointerEventData)
        {
            if (pointerEventData.button == PointerEventData.InputButton.Right)
                FabricatorTriggerClickedNotifier.Publish(this);
        }
    }
}