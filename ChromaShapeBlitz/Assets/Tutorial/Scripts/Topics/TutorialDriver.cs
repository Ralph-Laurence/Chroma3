using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialDriver : MonoBehaviour
{
    public TutorialSteps StepIdentifier;

    [SerializeField] private TutorialContent[] contents;
    [SerializeField] private TutorialDialog tutorialDialog;

    [Header("Interactions")]
    [SerializeField] private AudioClip clickSfx;
    [SerializeField] private TutorialPointerHand pointerHand;
    [SerializeField] private RectTransform continueButton;

    [SerializeField] private RectTransform canvasRectTransform;

    private UISound uisfx;
    private RectTransform m_clonedTarget = default;
    private int currentContentIndex = 0;
    private Button m_continueButton;

    [SerializeField] private bool autoPlay = true;
    [SerializeField] private float autoPlayDelay = 0.0F;

    // private bool isTutorialBegan;
    public readonly float ContinueButtonDefaultYPos = 60.0F;

    private TutorialUtilities tutorialUtils;

    void Awake()
    {
        uisfx         = UISound.Instance;
        tutorialUtils = TutorialUtilities.Instance;
        tutorialUtils.SetCanvasRect(canvasRectTransform);

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

                    // Always hide the pointer hand for every next step
                    pointerHand.Hide();
                },

                onHidden: () => MoveNextStep()
            );
            
        });
    }

    void Start() => StartCoroutine(BeginTutorial());

    void OnEnable()
    {
        TutorialEventNotifier.BindObserver(ObserveTutorialEvent);
    }

    void OnDisable()
    {
        TutorialEventNotifier.UnbindObserver(ObserveTutorialEvent);
    }

    private IEnumerator BeginTutorial()
    {
        if (autoPlay)
        {
            yield return new WaitForSeconds(autoPlayDelay);
            ShowTutorialContent();
        }
    }

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

    private void ShowTapToContinue() => StartCoroutine(IEShowTapToContinue());
    
    private IEnumerator IEShowTapToContinue()
    {
        yield return new WaitForSeconds(0.3F);

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
        //currentContentIndex = Mathf.Clamp(currentContentIndex, 0, contents.Length - 1);
        
        if (currentContentIndex >= contents.Length)
        {
            Debug.Log("Last content reached; stopping here...");
            return;
        }

        ShowTutorialContent();
    }

    public void ShowTutorialContent()
    {
        StartCoroutine(IEShowTutorialContent());
    }

    private IEnumerator IEShowTutorialContent()
    {
        var content = contents[currentContentIndex];

        if (content == null)
            yield break;
        //
        // Pause execution
        //
        if (content.Delay > 0.0F)
        {
            yield return new WaitForSeconds(content.Delay);
        }
        //
        //
        //
        tutorialDialog.SetContent(content);

        // Event before the tutorial content begins
        content.OnBeginContent.Invoke();

        // If the content was set to execute only specific actions,
        // we wont show the dialog and exit instead.
        if (content.ContentType == TutorialContentType.ExecuteAction)
        {
            CloseTutorialDialog(content);
            yield break;
        }
        //
        // For 3D-World tutorials
        //
        if (content.ContentType == TutorialContentType.Informative3D ||
            content.ContentType == TutorialContentType.Interactive3D)
        {
            Show3DWorldTutorialContent(content);
            yield break;
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
        var screenBounds = tutorialUtils.GetUIBounds(true);

        var continueButtonPosY = content.ContinueButtonPosition == ContinueButtonPositions.CenterScreen
                               ? screenBounds.y
                               : ContinueButtonDefaultYPos;

        continueButton.anchoredPosition = Vector2.up * continueButtonPosY;

        // If the content has a visual element i.e. target element, show it.
        if (content.TargetElement != null)
        {
            m_clonedTarget = tutorialUtils.CloneTargetElement(content);
            tutorialDialog.SetTarget(m_clonedTarget);

            // Original Code
            // if (content.ContentType == TutorialContentType.InteractiveUI)
            // {
            //     BindClickEventToClone(m_clonedTarget, content);
            //     HideTapToContinue();
            // }
            // else
            // {
            //     ShowTapToContinue();
            // }

            // Modded Code
            HideTapToContinue();

            // Original
            tutorialDialog.Show(() =>
            {
                m_clonedTarget.gameObject.SetActive(true);

                // Modded Code
                switch (content.ContentType)
                {
                    case TutorialContentType.InteractiveUI:
                        BindClickEventToClone(m_clonedTarget, content);
                        pointerHand.SetTarget(m_clonedTarget);
                        pointerHand.Show();
                        break;

                    case TutorialContentType.InformativeUI:
                    
                        // Turn off graphic raycasting for root
                        if (m_clonedTarget.TryGetComponent(out Graphic graphicRoot))
                                graphicRoot.raycastTarget = false;

                        // Turn off graphic raycasting for children
                        for (var i = 0; i < m_clonedTarget.childCount; i++)
                        {
                            var child = m_clonedTarget.GetChild(i);

                            if (child.TryGetComponent(out Graphic graphic))
                                graphic.raycastTarget = false;
                        }

                        ShowTapToContinue();
                        break;

                    default:
                        // We only show the "tap to continue" after the dialog was shown.
                        // Calling it early causes it to show up even before the dialog
                        // was made visible. For whatever type it is, just show "continue"
                        ShowTapToContinue();
                        break;
                }

                // Original
                // if (content.ContentType == TutorialContentType.InteractiveUI)
                // {
                //     pointerHand.SetTarget(m_clonedTarget);
                //     pointerHand.Show();
                // }
            });

            return;
        }

        tutorialDialog.Show(onShown: () => ShowTapToContinue());
    }

    private void BindClickEventToClone(RectTransform clone, TutorialContent content)
    {
        // Get the button component of the cloned button object
        var cloneButton = tutorialUtils.FindButtonComponent(clone.gameObject, true);

        // If no button was found, no need to proceed
        if (cloneButton == null) return;

        // Add new event listener
        cloneButton.onClick.AddListener(() =>
        {
            // Trigger custom interaction event or invoke original button click
            if (content.UseCustomInteractEvent)
            {
                content.onInteracted.Invoke();
                uisfx.PlayUISfx(clickSfx);
            }
            else
            {
                var originalButton = tutorialUtils.FindButtonComponent(content.TargetElement);

                if (originalButton != null)
                    originalButton.onClick.Invoke();
            }

            pointerHand.Hide();

            // Destroy the cloned target object
            Destroy(cloneButton.gameObject);

            CloseTutorialDialog(content);
        });
    }

    private void CloseTutorialDialog(TutorialContent content)
    {
        // Close dialog and move to the next tutorial step
        tutorialDialog.Hide(() =>
        {
            if (content.MoveNextStepWhenDone)
                MoveNextStep();
        });
    }


    //
    // Jump to specific content
    //
    public void JumpToContentIndex(int contentIndex)
    {
        if (contentIndex >= contents.Length)
            return;

        currentContentIndex = contentIndex;
    }
}