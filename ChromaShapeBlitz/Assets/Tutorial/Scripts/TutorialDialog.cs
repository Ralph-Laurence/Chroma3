using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DialogArrowDirections
{
    None,
    Left,
    Right,
    Above,
    Below
}

public enum DialogDirections
{
    CenterOfScreen,
    ToLeftOfTarget,
    ToRightOfTarget,
    BelowTarget,
    AboveTarget
}

public class TutorialDialog : MonoBehaviour
{
    [Header("Dialog Behaviour")]
    //[SerializeField] private RectTransform canvasRectTransform;
    [SerializeField] private RectTransform calloutTextWrapper;
    [SerializeField] private TextMeshProUGUI calloutText;
    [SerializeField] private AudioClip calloutShownSfx;
    
    [Space(5)]
    [SerializeField] private DialogDirections dialogDirection;
    // [SerializeField] private RectTransform avatar;
    //[SerializeField] private Button targetButtonCloner;
    //[SerializeField] private float distanceToTarget = 10.0F;


    [Space(5)] [Header("Callout Arrows")]

    [SerializeField] private TutorialDialogArrow arrowLeft;
    [SerializeField] private TutorialDialogArrow arrowRight;
    [SerializeField] private TutorialDialogArrow arrowAbove;

    private List<TutorialDialogArrow> dialogArrows;

    [SerializeField] private Button continueButton;

    private RectTransform m_rect;
    private RectTransform continueButtonRect;
    private UISound uisfx;

    private float selfHeight;
    // private RectTransform cloner;
    // private Image clonerImg;

    void Awake()
    {
        uisfx = UISound.Instance;

        TryGetComponent(out m_rect);
        continueButton.TryGetComponent(out continueButtonRect);
        // targetButtonCloner.TryGetComponent(out cloner);
        // targetButtonCloner.TryGetComponent(out clonerImg);

        selfHeight = m_rect.sizeDelta.y;

        dialogArrows = new List<TutorialDialogArrow>
        {
            arrowLeft,
            arrowRight,
            arrowAbove
        };
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDialogArrow();
        // UpdateDialogPosition();
        UpdateContinueButtonPosition();
    }

    private void UpdateDialogArrow()
    {
        if (arrowLeft.IsActive)
            arrowLeft.SetPositionToLeftOf(calloutTextWrapper);

        if (arrowRight.IsActive)   
            arrowRight.SetPositionToRightOf(calloutTextWrapper);

        if (arrowAbove.IsActive)
            arrowAbove.SetPositionAbove(calloutTextWrapper);
    }

    /// <summary>
    /// The continue button should always be positioned below the dialog box
    /// </summary>
    private void UpdateContinueButtonPosition()
    {
        var calloutHalfHeight = calloutTextWrapper.sizeDelta.y / 2.0F;
        var buttonHalfHeight  = continueButtonRect.sizeDelta.y / 2.0F;

        var posY = (calloutHalfHeight * -1.0F) - buttonHalfHeight;

        continueButtonRect.anchoredPosition = new Vector2(0.0F, posY);
    }

    public void Show(Action onShown = null)
    {
        uisfx.PlayUISfx(calloutShownSfx);

        LeanTween.scale(gameObject, Vector3.one, 0.2F)
                 .setOnComplete(() => onShown?.Invoke());
    }

    public void Hide(Action onHidden = null)
    {
        LeanTween.scale(gameObject, Vector3.forward * 1.0F, 0.2F)
                 .setOnComplete(() => onHidden?.Invoke());
    }

    private void UpdateDialogPosition()
    {
        // if (target == null)
        //     return;
            
        // if (Input.GetKeyUp(KeyCode.C))
        //     RenderCloner();

        // switch (dialogPosition)
        // {
        //     case DialogPositions.AboveTarget:
        //         PositionAboveTarget(target);
        //         break;
        // }
    }

    public void UpdateContent(string content, DialogDirections dialogDirection)
    {
        calloutText.text     = content;
        //this.dialogDirection = dialogDirection;
        
        TutorialDialogArrow targetArrow = default;

        for (var i = 0; i < dialogArrows.Count; i++)
        {
            var arrow = dialogArrows[i];
            arrow.gameObject.SetActive(false);

            if (arrow.Direction == DialogArrowDirections.Left && 
               (dialogDirection == DialogDirections.ToLeftOfTarget ||
                dialogDirection == DialogDirections.CenterOfScreen)
            )
                targetArrow = arrow;
            
            else if (arrow.Direction == DialogArrowDirections.Right && dialogDirection == DialogDirections.ToRightOfTarget)
                targetArrow = arrow;
            
            else if (arrow.Direction == DialogArrowDirections.Above && dialogDirection == DialogDirections.AboveTarget)
                targetArrow = arrow;
        }

        if (targetArrow != null)
            targetArrow.gameObject.SetActive(true);
    }

    // private void PositionAboveTarget(RectTransform targetElement)
    // {
    //     var elementHalfHeight = targetElement.sizeDelta.y / 2.0F;
    //     var selfHalfHeight    = calloutTextWrapper.sizeDelta.y / 2.0F;

    //     var posY = elementHalfHeight + selfHalfHeight - distanceToTarget;

    //     m_rect.anchoredPosition = new Vector2(0.0F, posY);
    // }

    /*
    private void RenderCloner()
    {
        //..........................................................//
        // # Position the cloner exactly at the target's position # //
        //..........................................................//
        
        // Step 1: Get the target's world position
        var worldPosition = target.TransformPoint(Vector3.zero);

        // Step 2: Calculate the center position offset of the target in local space
        var targetCenterOffset = new Vector2
        (
            (0.5F - target.pivot.x) * target.rect.width,
            (0.5F - target.pivot.y) * target.rect.height
        );

        // Step 3: Convert the target's center to world space
        var targetCenterWorldPosition = worldPosition + target.TransformVector(targetCenterOffset);

        // Step 4: Convert the world position of the center to the canvas local space
        RectTransformUtility.ScreenPointToLocalPointInRectangle
        (
            canvasRectTransform,
            RectTransformUtility.WorldToScreenPoint(null, targetCenterWorldPosition),
            null,
            out Vector2 localPoint
        );

        // Step 5: Assign this position to the cursor's anchored position
        cloner.anchoredPosition = localPoint;
    }
    */
}
