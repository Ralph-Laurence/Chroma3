using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class TutorialControllerBase : MonoBehaviour
{
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private Image tutorialOverlay;
    [SerializeField] private List<TutorialActor> actors;
    [SerializeField] private TutorialDialog tutorialDialog;
    [SerializeField] private AudioClip clickSfx;

    private int currentActor;
    //protected virtual string Identity => name;

    [SerializeField] private Button continueButton;

    private GameSessionManager gsm;
    private UISound uisfx;
    private GameObject actorTargetClone;
    private RectTransform canvasRectTransform;
    private RectTransform continueButtonRect;

    void Awake()
    {
        gsm = GameSessionManager.Instance;
        uisfx = UISound.Instance;

        tutorialCanvas.TryGetComponent(out canvasRectTransform);
        continueButton.TryGetComponent(out continueButtonRect);

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() => OnContinueButton());
    }

    void Start()
    {
        var tutorialStep = gsm.UserSessionData.TutorialStep;

        if (tutorialStep == Constants.TutorialSteps.STEP1_LEVEL_NAVIGATION)
            ShowTutorial();
    }

    #region EVENT_OBSERVERS
    //
    //
    //
    void OnEnable()
    {
        TutorialEventNotifier.BindObserver(ObserveTutorialEventNotified);
    }

    void OnDisable()
    {
        TutorialEventNotifier.UnbindObserver(ObserveTutorialEventNotified);
    }

    protected abstract void ObserveTutorialEventNotified(string reciever, string data);
    //
    //
    //
    #endregion EVENT_OBSERVERS

    public virtual void MoveNext()
    {
        currentActor++;
        currentActor = Mathf.Clamp(currentActor, 0, actors.Count-1);
    }

    public virtual void ShowTutorial() => StartCoroutine(IEShowTutorial());

    private IEnumerator IEShowTutorial()
    {
        if (!tutorialCanvas.activeInHierarchy)
            tutorialCanvas.SetActive(true);

        if (IsOverlayTransparent())
        {
            LeanTween.alpha(tutorialOverlay.gameObject, 0.2F, 0.25F).setDelay(0.25F);
            yield return new WaitForSeconds(0.25F);
        }
        
        var actor = actors[currentActor];

        tutorialDialog.UpdateContent(actor.Description, actor.DialogDirection);
        tutorialDialog.Show();

        if (actor.ActorType == TutorialActorType.Informative)
        {
            ShowContinueButton();
            yield break;
        }

        HideContinueButton();

        yield return new WaitForSeconds(0.4F);
        ShowTarget(actor);
    }

    public void OnContinueButton()
    {
        uisfx.PlayUISfx(clickSfx);

        // Hide the continue button first
        HideContinueButton();

        // Close the dialog then show the next tutorial step.
        // This should show the dialog back again
        tutorialDialog.Hide(() => {
            MoveNext();
            ShowTutorial();
        });
    }

    public bool IsOverlayTransparent()
    {
        var alpha = tutorialOverlay.color.a;

        if (Mathf.RoundToInt(alpha) == 0)
            return true;

        return false;
    }

    public void ShowTarget(TutorialActor actor)
    {
        // Reference to the original position of the element to clone
        actor.TargetElement.TryGetComponent(out RectTransform originalTargetPos);

        // Duplicate the target element. We do this to preserve the original traits
        actorTargetClone = actor.CloneTargetElement(tutorialOverlay.transform);
        actorTargetClone.TryGetComponent(out RectTransform cloneRect);

        //..........................................................//
        // # Delete cloned target after it has been clicked       # //
        //..........................................................//

        // Get the button at the clone's parent level
        actorTargetClone.TryGetComponent(out Button cloneButton);
        var cloneButtonFoundIn = 0; // 0 -> Parent level

        // If there is no button attached to the parent, find it down to the child level
        if (cloneButton == null)
        {
            cloneButton = actorTargetClone.GetComponentInChildren<Button>();
            cloneButtonFoundIn = 1; // 1 -> Child Level
        }

        // We assume that the button was found
        if (cloneButton != null)
        {
            // Clear the event bindings of the cloned element's button.
            // We will trigger the event from the original level instead.
            ClearPersistentEventListeners(cloneButton);
            cloneButton.onClick.AddListener(() => {
            
                Button origLevelButton;

                if (cloneButtonFoundIn == 0)
                    actor.TargetElement.TryGetComponent(out origLevelButton);
                else
                    origLevelButton = actor.TargetElement.GetComponentInChildren<Button>();

                origLevelButton.onClick.Invoke();
                Destroy(actorTargetClone);

                // Close the dialog then show the next tutorial step.
                // This should show the dialog back again
                tutorialDialog.Hide(() =>
                {
                    MoveNext();
                    ShowTutorial();
                });
            });
        }

        // Change the anchor of the cloned element into the middle screen.
        // The middle anchor should serve as the element's origin / pivot
        // so that we can move it anywhere and perform easy calculations.
        var center = Vector2.one * 0.5F;

        cloneRect.pivot = center;
        cloneRect.anchorMin = center;
        cloneRect.anchorMax = center;
        cloneRect.anchoredPosition = Vector2.zero;

        //..........................................................//
        // # Position the cloner exactly at the target's position # //
        //..........................................................//
        
        // Step 1: Get the target's world position
        var worldPosition = originalTargetPos.TransformPoint(Vector3.zero);

        // Step 2: Calculate the center position offset of the target in local space
        var targetCenterOffset = new Vector2
        (
            (0.5F - originalTargetPos.pivot.x) * originalTargetPos.rect.width,
            (0.5F - originalTargetPos.pivot.y) * originalTargetPos.rect.height
        );

        // Step 3: Convert the target's center to world space
        var targetCenterWorldPosition = worldPosition + originalTargetPos.TransformVector(targetCenterOffset);

        // Step 4: Convert the world position of the center to the canvas local space
        RectTransformUtility.ScreenPointToLocalPointInRectangle
        (
            canvasRectTransform,
            RectTransformUtility.WorldToScreenPoint(null, targetCenterWorldPosition),
            null,
            out Vector2 localPoint
        );

        // Step 5: Assign this position to the cursor's anchored position
        cloneRect.anchoredPosition = localPoint;
    }

    public virtual void ShowContinueButton()
    {
        continueButton.gameObject.SetActive(true);

        LeanTween.scale(continueButton.gameObject, Vector3.one * 1.08F, 0.3F)
                 .setLoopPingPong();
    }

    public virtual void HideContinueButton()
    {
        LeanTween.cancel(continueButton.gameObject);

        // Reset the scale to its original value
        continueButtonRect.localScale = Vector3.one;

        continueButton.gameObject.SetActive(false);
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