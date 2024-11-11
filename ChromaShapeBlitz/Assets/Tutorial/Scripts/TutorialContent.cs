using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEditor;

using InspectorUtils;

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

public enum ContinueButtonPositions
{ 
    Default,
    CenterScreen
}


//[Serializable]
//public struct TutorialContentReplaceArgs
//{ 
//    public string Find { get; set; }
//    public string Replace { get; set; }
//}

public class TutorialContent : MonoBehaviour
{
    //public TutorialSteps TutorialStep;
    [Help("When ContentType is set to \"Execute Action\", make sure to register the action in the \"OnBeginContent\" event.")]
    public TutorialContentType ContentType;
    public ContinueButtonPositions ContinueButtonPosition;

    [Space(10)]
    [Help("AutoHighlight expects that the \"Target Element\" has a component of type UnityEngine.UI.Button and will set its state to \"Hovered\"")]
    public GameObject TargetElement;
    public bool AutoHighlight = false;

    [Space(10, order = 1)]
    [Header("\u2731 Execute actions before the content is shown.", order = 2)]
    [Space(10, order = 3)]
    public UnityEvent OnBeginContent;
    
    [Space(10)]
    [Help("By default, we automatically proceed to the next tutorial step after this step was done. If this is set to FALSE, we assume that a script controls it.")]
    public bool MoveNextStepWhenDone = true;

    [Space(5)]
    public bool UseCustomInteractEvent;
    public UnityEvent onInteracted;


    [Space(5)]
    [TextArea(5, 500)]
    public string Description;
    //public List<TutorialContentReplaceArgs> ReplaceArgs;
    
    [Space(5)]
    public float DialogYOffset;
    public float DialogXOffset;
    public DialogDirections DialogDirection;


    [Space(10, order = 1)]
    [Header("\u2731 Only for Informative3D content types", order = 2)]
    [Space(10, order = 3)]
    public bool ShowContinueButton = false;
    
    [Space(10, order = 1)]
    [Header("\u2731 Pause before executing this content", order = 2)]
    [Space(10, order = 3)]
    public float Delay = 0.0F;
    //public GameObject CloneTargetElement(Transform parent)
    //{
    //    var target = Instantiate(TargetElement, parent);
    //    return target;
    //}

    public RectTransform GetOriginalElementRect()
    {
        if (TargetElement == null)
            return default;

        TargetElement.TryGetComponent(out RectTransform rect);
        return rect;
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
