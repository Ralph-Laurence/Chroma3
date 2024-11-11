using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialUtilities : MonoBehaviour
{

    #region SINGLETON
    public static TutorialUtilities Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else if (Instance != this)
            Destroy(gameObject);
    }
    #endregion SINGLETON

    [SerializeField] private RectTransform canvasRectTransform;

    public void SetCanvasRect(RectTransform canvasRect) => canvasRectTransform = canvasRect;

    /// <summary>
    /// Position the clone exactly at the original target's position
    /// </summary>
    public RectTransform CloneTargetElement(TutorialContent content, GameObject customTarget = null)
    {
        var targetElement = customTarget == null ? content.TargetElement : customTarget;

        // Duplicate the target element and place it into the tutorial canvas
        Instantiate(targetElement, canvasRectTransform).TryGetComponent(out RectTransform cloneRect);

        // Highlight the (cloned) target element only when it is explicitly set
        if (content.AutoHighlight)
            content.HighlightTargetIfButton(cloneRect.gameObject, content);

        // Reference to the original rect transform of the element to clone
        targetElement.TryGetComponent(out RectTransform originalTargetRect);

        //cloneRect.sizeDelta         = originalTargetRect.sizeDelta;
        cloneRect.anchoredPosition = originalTargetRect.anchoredPosition;
        cloneRect.pivot = originalTargetRect.pivot;

        // Move the clone into the original object's position
        MatchCloneToOriginalPosition(cloneRect, originalTargetRect);

        // Disable the object after cloning as we will display it later
        // only when the dialog has been shown
        cloneRect.gameObject.SetActive(false);

        TutorialEventNotifier.NotifyObserver(TutorialEventNames.ElementCloned, cloneRect.gameObject);
        return cloneRect;
    }

    /// <summary>
    /// Position the clone exactly at the original target's position
    /// </summary>
    public void MatchCloneToOriginalPosition(RectTransform clone, RectTransform original)
    {
        // Step 1: Get the target's world position
        var worldPosition = original.TransformPoint(Vector3.zero);

        // Step 2: Calculate the center position offset of the target in local space
        var targetCenterOffset = new Vector2
        (
            (0.5F - original.pivot.x) * original.rect.width,
            (0.5F - original.pivot.y) * original.rect.height
        );

        // Step 3: Convert the target's center to world space
        var targetCenterWorldPosition = worldPosition + original.TransformVector(targetCenterOffset);

        // Step 4: Convert the world position of the center to the canvas local space
        RectTransformUtility.ScreenPointToLocalPointInRectangle
        (
            canvasRectTransform,
            RectTransformUtility.WorldToScreenPoint(null, targetCenterWorldPosition),
            null,
            out Vector2 localPoint
        );

        // Change the anchor of the cloned element into the middle screen.
        // The middle anchor should serve as the element's origin / pivot
        // so that we can move it anywhere and perform calculations easily.
        var center = Vector2.one * 0.5F;

        clone.pivot = center;
        clone.anchorMin = center;
        clone.anchorMax = center;
        clone.sizeDelta = new Vector3(original.rect.width, original.rect.height, 1.0F); //original.sizeDelta;

        // Step 5: Assign this position to the cursor's anchored position
        clone.anchoredPosition = localPoint;
    }

    public Vector2 GetUIBounds(bool whole = false)
    {
        // Get the scaled size of the canvas (this is the actual size in UI coordinates)
        Vector2 canvasSize = canvasRectTransform.rect.size;

        // Divide by 2 to get the range from center to the edge (since the canvas is centered)
        Vector2 uiBounds = canvasSize / 2f;

        // These are the boundary limits (e.g., X: �260, Y: �460 in your case)
        if (!whole)
            return uiBounds;

        // Make the values whole number
        uiBounds.x = Convert.ToInt32(uiBounds.x);
        uiBounds.y = Convert.ToInt32(uiBounds.y);

        return uiBounds;
    }

    /// <summary>
    /// Disable all editor-assigned click listeners for button
    /// </summary>
    public void ClearPersistentEventListeners(Button button)
    {
        // Disable all event listeners
        var count = button.onClick.GetPersistentEventCount();

        for (var i = 0; i < count; i++)
        {
            button.onClick.SetPersistentListenerState(i, UnityEventCallState.Off);
        }

        // Remove the UX Button component to prevent double playing the tap sound
        button.TryGetComponent(out UXButton uXButton);

        if (uXButton != null)
            Destroy(uXButton);
    }

    /// <summary>
    /// Find the attached "Button" component onto a GameObject.
    /// </summary>
    /// <param name="theObject">The gameobject that holds a UI Button component</param>
    /// <param name="clearClickEvents">Should we disable click listeners?</param>
    public Button FindButtonComponent(GameObject theObject, bool clearClickEvents = false)
    {
        // Search the button at the root object first
        theObject.TryGetComponent(out Button button);

        // If no button was found, check the children
        if (button == null)
        {
            button = theObject.GetComponentInChildren<Button>();
        }

        // Clear any existing event listeners
        if (button != null && clearClickEvents)
            ClearPersistentEventListeners(button);

        return button;
    }
}
