using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum TutorialContentType
{
    /// <summary>
    /// When an element doesn't have to be clickable but should be highlighted
    /// </summary>
    InformativeUI,

    /// <summary>
    /// Highlight the target element then make it clickable
    /// </summary>
    InteractiveUI,

    /// <summary>
    /// Visualize the target elements in 3D World.
    /// </summary>
    Informative3D,

    /// <summary>
    /// Interactable elements in 3D world
    /// </summary>
    Interactive3D,

    /// <summary>
    /// This wont show any dialogs but will execute an action instead.
    /// Make sure to register the action in the "OnBeginContent" event.
    /// </summary>
    ExecuteAction
}

[Serializable]
public struct TutorialContentReplaceArgs
{ 
    public string Find { get; set; }
    public string Replace { get; set; }
}

public class TutorialContent : MonoBehaviour
{
    //public TutorialSteps TutorialStep;
    public TutorialContentType ContentType;
    public GameObject TargetElement;

    [Space(10)]
    [Header("Only for BUTTON target elements")]
    public bool AutoHighlight = false;

    [Space(10)]
    [Header("Only for Informative3D content types")]
    public bool ShowContinueButton = false;
    [Space(10)]

    public UnityEvent OnBeginContent;
    
    [Space(5)]
    public bool UseCustomInteractEvent;
    public UnityEvent onInteracted;

    [Space(5)][TextArea]
    public string Description;
    //public List<TutorialContentReplaceArgs> ReplaceArgs;
    
    [Space(5)]
    public float DialogYOffset;
    public DialogDirections DialogDirection;

    [Space(5)]
    public bool MoveNextStepWhenDone = true;

    public GameObject CloneTargetElement(Transform parent)
    {
        var target = Instantiate(TargetElement, parent);
        return target;
    }

    /// <summary>
    /// Only takes effect when the target element is a UI Button.
    /// This effectively toggles the "Hovered" state, thus appearing
    /// as "Highlighted"
    /// </summary>
    public void HighlightTargetIfButton(GameObject target, TutorialContent content)
    {
        // Make sure that the target element is indeed a button.
        target.TryGetComponent(out Button clonedButton);

        if (clonedButton == null)
            return;

        // Get the button's colors
        ColorBlock colorBlock = clonedButton.colors;

        var highlightedColor = colorBlock.highlightedColor;

        // If the button was only for informative purposes, remove its Button component
        // but retain its Image component with highlighted color
        if (content.ContentType == TutorialContentType.InformativeUI)
        {
            clonedButton.TryGetComponent(out Image image);

            if (image != null)
                image.color = highlightedColor;

            Destroy(clonedButton);
            return;
        }

        // Set the hovered state color to the current color
        colorBlock.normalColor = highlightedColor;

        // Apply the color block
        clonedButton.colors = colorBlock;

        // Optionally, disable the button's interactability to keep it in hovered state
        // clonedButton.interactable = false;
    }
}
