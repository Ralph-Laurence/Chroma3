using System;
using UnityEngine;

public enum TutorialActorType
{
    /// <summary>
    /// When an actor doesn't have to be clickable but should be highlighted
    /// </summary>
    Informative,

    /// <summary>
    /// Highlight the actor then make it clickable
    /// </summary>
    Interactive,

    /// <summary>
    /// An actor that is controlled by script.
    /// </summary>
    // ScriptDriven
}

public class TutorialActor : MonoBehaviour
{
    public TutorialSteps TutorialStep;
    public TutorialActorType ActorType;
    public GameObject TargetElement;

    [Space(5)][TextArea]
    public string Description;
    
    [Space(5)]
    public DialogDirections DialogDirection;

    [Header("Behaviours")]
    [ReadOnly]
    public string Identifier = Guid.NewGuid().ToString();

    public GameObject CloneTargetElement(Transform parent)
    {
        var target = Instantiate(TargetElement, parent);
        return target;
    }
}
