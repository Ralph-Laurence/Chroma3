using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TutorialDriver : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private TutorialContent[] contents;
    [SerializeField] private TutorialDialog tutorialDialog;

    [Header("Interactions")]
    [SerializeField] private AudioClip clickSfx;
    [SerializeField] private TutorialPointerHand pointerHand;
    [SerializeField] private RectTransform continueButton;

    private UISound uisfx;
    private RectTransform m_clonedTarget = default;
    private int currentContentIndex = 0;
    private Button m_continueButton;

    [SerializeField] private bool autoPlay = true;

    void Awake()
    {
        uisfx = UISound.Instance;

        // The button component attached to the tutorial overlay
        // will be used for "Click To Continue" feature.
        tutorialDialog.GetOverlay().TryGetComponent(out m_continueButton);

        m_continueButton.onClick.AddListener(() => {

            if (m_clonedTarget != null)
                Destroy(m_clonedTarget.gameObject);

            // Close dialog and move to the next tutorial step
            tutorialDialog.Hide
            (   
                onHiding: () => {
                    uisfx.PlayUISfx(clickSfx);
                    HideTapToContinue();
                },

                onHidden: () => MoveNextStep()
            );
            
        });
    }

    void Start()
    {
        if (autoPlay)
            ShowTutorialContent();
    }

    void OnEnable() => TutorialEventNotifier.BindObserver(ObserveTutorialEvent);

    void OnDisable() => TutorialEventNotifier.UnbindObserver(ObserveTutorialEvent);

    private void ObserveTutorialEvent(string key, object data)
    {
        if (!string.IsNullOrEmpty(key))
        {
            switch (key)
            {
                case TutorialEventNames.ShowContinueButton:
                    ShowTapToContinue();
                    break;

                case TutorialEventNames.MoveNextStep:
                    MoveNextStep();
                    break;
            }
        }
    }

    private void ShowTapToContinue()
    {
        continueButton.gameObject.SetActive(true);
        m_continueButton.enabled = true;

        LeanTween.scale(continueButton.gameObject, Vector3.one * 1.08F, 0.3F)
                 .setLoopPingPong();
    }

    private void HideTapToContinue()
    {
        m_continueButton.enabled = false;
        LeanTween.cancel(continueButton.gameObject);

        // Reset the scale to its original value
        continueButton.localScale = Vector3.one;
        continueButton.gameObject.SetActive(false);
    }

    private void MoveNextStep()
    {
        currentContentIndex++;
        currentContentIndex = Mathf.Clamp(currentContentIndex, 0, contents.Length - 1);
        
        // Always hide the pointer hand for every next step
        pointerHand.Hide();

        ShowTutorialContent();
    }

    public void ShowTutorialContent()
    {
        var content = contents[currentContentIndex];

        if (content == null)
            return;

        tutorialDialog.SetContent(content);

        // Event before the tutorial content begins
        content.OnBeginContent.Invoke();

        // If the content was set to execute only specific actions,
        // we wont show the dialog and exit instead.
        if (content.ContentType == TutorialContentType.ExecuteAction)
            return;
        //
        // For 3D-World tutorials
        //
        if (content.ContentType == TutorialContentType.Informative3D ||
            content.ContentType == TutorialContentType.Interactive3D)
        {
            Show3DWorldTutorialContent(content);
            return;
        }
        //
        // For UI-Based tutorials
        //
        ShowUIBasedTutorialContent(content);
    }

    private void Show3DWorldTutorialContent(TutorialContent content)
    {
        tutorialDialog.Show(noOverlay: true, onShown: () =>
        {
            if (content.ContentType == TutorialContentType.Informative3D && content.ShowContinueButton)
                ShowTapToContinue();

            if (content.TargetElement == null)
                return;

            content.TargetElement.TryGetComponent(out Interactive3DTutorial content3D);
            
            if (content3D != null)
                content3D.Execute();
        });
    }

    private void ShowUIBasedTutorialContent(TutorialContent content)
    {
        // If the tutorial content needs an interactable element, clone it
        if (content.TargetElement != null)
        {
            m_clonedTarget = CloneTargetElement(content.TargetElement, content);
            tutorialDialog.SetTarget(m_clonedTarget);
            
            if (content.ContentType == TutorialContentType.InteractiveUI)
                BindClickEventToClone(m_clonedTarget, content);
        }
        else
        {
            ShowTapToContinue();
        }

        // Show the dialog first then show the target when the dialog is fully shown.
        // If a clone target is available, show the pointer and render it as topmost
        tutorialDialog.Show(() =>
        {
            if (m_clonedTarget != null)
            {
                m_clonedTarget.gameObject.SetActive(true);

                if (content.ContentType == TutorialContentType.InformativeUI)
                {
                    ShowTapToContinue();
                }
                else
                {
                    pointerHand.SetTarget(m_clonedTarget);
                    pointerHand.Show();
                }
            }
        });
    }

    private RectTransform CloneTargetElement(GameObject targetElement, TutorialContent content)
    {
        // Duplicate the target element and place it into the tutorial canvas
        Instantiate(targetElement, canvasRectTransform).TryGetComponent(out RectTransform cloneRect);
        
        // Highlight the (cloned) target element only when it is explicitly set
        if (content.AutoHighlight)
            content.HighlightTargetIfButton(cloneRect.gameObject, content);

        // Reference to the original rect transform of the element to clone
        targetElement.TryGetComponent(out RectTransform originalTargetRect);

        // Change the anchor of the cloned element into the middle screen.
        // The middle anchor should serve as the element's origin / pivot
        // so that we can move it anywhere and perform calculations easily.
        var center = Vector2.one * 0.5F;

        cloneRect.pivot = center;
        cloneRect.anchorMin = center;
        cloneRect.anchorMax = center;

        cloneRect.sizeDelta = originalTargetRect.sizeDelta;

        // Move the clone into the original object's position
        MatchCloneToOriginalPosition(cloneRect, originalTargetRect);

        // Disable the object after cloning as we will display it later
        // only when the dialog has been shown
        cloneRect.gameObject.SetActive(false);

        return cloneRect;
    }

    /// <summary>
    /// Position the clone exactly at the original target's position
    /// </summary>
    private void MatchCloneToOriginalPosition(RectTransform clone, RectTransform original)
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

        // Step 5: Assign this position to the cursor's anchored position
        clone.anchoredPosition = localPoint;
    }

    private void BindClickEventToClone(RectTransform clone, TutorialContent content)
    {
        // Get the button component of the cloned button object
        var cloneButton = FindButtonComponent(clone.gameObject, true);

        // If no button was found, no need to proceed
        if (cloneButton == null) return;

        // Add new event listener
        cloneButton.onClick.AddListener(() => {
            
            // Trigger custom interaction event or invoke original button click
            if (content.UseCustomInteractEvent)
            {
                content.onInteracted.Invoke();
                uisfx.PlayUISfx(clickSfx);
            }
            else
            {
                var originalButton = FindButtonComponent(content.TargetElement);

                if (originalButton != null)
                    originalButton.onClick.Invoke();
            }

            // Destroy the cloned target object
            Destroy(cloneButton.gameObject);

            // Close dialog and move to the next tutorial step
            tutorialDialog.Hide(() =>
            {
                if (content.MoveNextStepWhenDone)
                    MoveNextStep();
            });
        });
    }

    private Button FindButtonComponent(GameObject theObject, bool clearClickEvents = false)
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

    private void ClearPersistentEventListeners(Button button)
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
}