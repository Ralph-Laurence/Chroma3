using System;
using System.Collections;
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
    AutoCalculate,
    BelowTarget,
    AboveTarget,
    TopOfScreen,
    CenterOfScreen
}

public class TutorialDialog : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRectTransform;

    [Header("Dialog Base Traits")]
    [SerializeField] private float              overlayAlpha = 0.3215F;
    [SerializeField] private RectTransform      callout;
    [SerializeField] private TextMeshProUGUI    calloutText;
    [SerializeField] private Image              calloutBackground;
    [SerializeField] private AudioClip          sfxDialogShown;

    private Image overlay;
    private UISound         uiSfx;
    private Vector2         m_ScreenBounds;
    private TutorialContent m_CurrentContent;
    private RectTransform   m_CurrentTarget;

    void Awake()
    {
        TryGetComponent(out overlay);
        uiSfx = UISound.Instance;
    }

    void Start()
    {
        // Calculate the Canvas size as Screen Bounds
        m_ScreenBounds = GetUIBounds(true);
    }

    void Update()
    {
        // Position this dialog into the target element
        if (m_CurrentTarget != null)
            MoveToTargetPosition();
    }
    //###################################################
    //==========   LOGIC VISIBLE TO ANY CLASS  ==========
    //###################################################

    #region FRONT_LOGIC

    public void Clear(Action afterClear = null)
    {
        Hide(onHidden: () =>
        {
            m_CurrentContent = null;
            calloutText.text = string.Empty;
            m_CurrentTarget  = null;

            // Position the dialog
            callout.anchoredPosition = Vector2.zero;

            afterClear?.Invoke();
        });
    }

    public void SetContent(TutorialContent content)
    {
        m_CurrentContent = content;
        calloutText.text = m_CurrentContent.Description;

        //var description = m_CurrentContent.Description;

        //if (content.ReplaceArgs != null)
        //    description = ReplaceInContent(description, content.ReplaceArgs);

        //calloutText.text = description;
    }

    public void SetTarget(RectTransform target) => m_CurrentTarget = target;

    public void Show(Action onShown = null, bool noOverlay = false)
    {
        var onCompleteCallback = new Action(() => {
            
            MakeTransparent(new List<Graphic> { calloutBackground, calloutText }, false);
            
            if (uiSfx != null)
                uiSfx.PlayUISfx(sfxDialogShown);

            onShown?.Invoke();
        });

        // Dont show the overlay because interacting with 3D world doesnt need UI
        if (noOverlay)
        {
            // Display the dialog modal
            LeanTween.scale(callout, Vector3.one, 0.2F)
                     .setOnComplete(onCompleteCallback);
        }
        // Fade in to show the UI overlay
        else
        {
            // Tween callback to change alpha
            var onOverlayAlpha = new Action<float>( alpha => {
                var color = overlay.color;
                color.a = alpha;

                overlay.color = color;
            });

            LeanTween.value(overlay.gameObject, onOverlayAlpha, overlay.color.a, overlayAlpha, 0.25F)
           .setOnComplete(() =>
           {
               MakeTransparent(new List<Graphic> { calloutBackground, calloutText }, false);

               // Display the dialog modal
               LeanTween.scale(callout, Vector3.one, 0.2F)
                        .setOnComplete(onCompleteCallback);
           });
        }
    }

    public void Hide(Action onHiding = null, Action onHidden = null)
    {
        m_CurrentTarget = null;

        onHiding?.Invoke();
        
        StartCoroutine(IEHide(onHidden));
    }

    public Image GetOverlay()
    {
        if (overlay == null)
            TryGetComponent(out overlay);

        return overlay;
    }

    /// <summary>
    /// Should the dialog overlay allow or block interactions?
    /// </summary>
    public void AllowOverlayPassThrough(bool allow)
    {
        var overlay = GetOverlay();

        if (overlay != null)
            overlay.raycastTarget = allow;
    }

    #endregion FRONT_LOGIC
    
    //###################################################
    //==========        INTERNAL LOGIC         ==========
    //###################################################

    #region BACK_LOGIC
    
    //private string ReplaceInContent(string contentText, List<TutorialContentReplaceArgs> replaceArgs)
    //{
    //    foreach (var arg in replaceArgs)
    //    {
    //        contentText.Replace(arg.Find, arg.Replace);
    //    }

    //    return contentText;
    //}

    private IEnumerator IEHide(Action onHidden = null)
    {
        LeanTween.scale(callout, Vector3.zero, 0.2F)
            .setOnUpdate((float transitionTime) =>
            {

                // During the middle of transition, hide the avatar
                // if (transitionTime >= 0.5F)
                //     tutorAvatar.gameObject.SetActive(false);
            })
            .setOnComplete(() => {
                MakeTransparent(new List<Graphic> { overlay, calloutBackground, calloutText });
            });

        yield return new WaitForSeconds(0.4F);
        onHidden?.Invoke();
    }

    private void MoveToTargetPosition()
    {
        var targetPos        = m_CurrentTarget.anchoredPosition;
        var targetHalfHeight = m_CurrentTarget.sizeDelta.y / 2.0F;
        var dialogHalfHeight = callout.sizeDelta.y / 2.0F;
        var targetYDistance  = 20.0F + m_CurrentContent.DialogYOffset; // Move the dialog away from target

        Vector2 finalDialogPos = default;

        // if the Target is Above the screen half height, Or the target position
        // is explicitly set, position the dialog BELOW the target
        if ((m_CurrentContent.DialogDirection == DialogDirections.AutoCalculate && m_ScreenBounds.y > 0) || 
            (m_CurrentContent.DialogDirection == DialogDirections.BelowTarget))
        {
            finalDialogPos.y = targetPos.y - targetHalfHeight - dialogHalfHeight - targetYDistance;
        }

        else if (m_CurrentContent.DialogDirection == DialogDirections.TopOfScreen)
        {
            finalDialogPos.y = m_ScreenBounds.y - dialogHalfHeight - m_CurrentContent.DialogYOffset;
        }

        // Position the dialog
        callout.anchoredPosition = finalDialogPos;
    }

    /// <summary>
    /// Make a UI Graphic Element transparent but don't disable it.
    /// </summary>
    /// <param name="graphics">UI Elements</param>
    /// <param name="makeTransparent">Should the graphic elements be made transparent?</param>
    private void MakeTransparent(List<Graphic> graphics, bool makeTransparent = true)
    {
        if (graphics == null || graphics?.Count <= 0)
            return;

        graphics.ForEach(g =>
        {
            var gColor = g.color;
            gColor.a   = makeTransparent ? 0.0F : 1.0F;

            g.color = gColor;
        });
    }

    /// <summary>
    /// Gets the screen size which is already scaled by the canvas.
    /// This is NOT the physical screen size, but the canvas dimensions
    /// acting as "Screen Size".
    /// </summary>
    /// <returns>Canvas Scaled Screen Size</returns>
    private Vector2 GetUIBounds(bool whole = false)
    {
        // Get the scaled size of the canvas (this is the actual size in UI coordinates)
        Vector2 canvasSize = canvasRectTransform.rect.size;

        // Divide by 2 to get the range from center to the edge (since the canvas is centered)
        Vector2 uiBounds = canvasSize / 2f;

        // These are the boundary limits (e.g., X: ±260, Y: ±460 in your case)
        if (!whole)
            return uiBounds;

        // Make the values whole number
        uiBounds.x = Convert.ToInt32(uiBounds.x);
        uiBounds.y = Convert.ToInt32(uiBounds.y);

        return uiBounds;
    }
    #endregion BACK_LOGIC
}
