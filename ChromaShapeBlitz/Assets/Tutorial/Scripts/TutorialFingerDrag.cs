using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialFingerDrag : MonoBehaviour
{
    [SerializeField] private GameObject animationObject;
    [SerializeField] private Image finger;
    [SerializeField] private Image arrow;
    [SerializeField] private Sprite fingerUp;
    [SerializeField] private Sprite fingerDown;
    [SerializeField] private Sprite arrowLeft;
    [SerializeField] private Sprite arrowRight;
    [SerializeField] private float startingX = 60.0F;
    [SerializeField] private float endingX = 100.0F;

    private RectTransform animationObjRect;
    private bool animationBegan;
    private bool isAnimating; // New flag to control loop

    [Space(10)]
    [Header("Screen Drag")]
    [SerializeField] private Slider dragProgressMeter;
    [SerializeField] private float swipeThreshold = 100F; // Threshold for detecting a swipe
    [SerializeField] private int dragProgress = 0; // Counter for drag progress
    private Vector2 startTouchPosition;
    private Vector2 currentTouchPosition;
    private bool isDragging = false;

    public UnityEvent onDragTutorialComplete;

    private IEnumerator IEBeginAnimation()
    {
        if (animationBegan)
            yield break;

        animationBegan = true;
        isAnimating = true;

        while (isAnimating) // Loop until isAnimating is false
        {
            finger.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.2F);

            // ######## MOVE LEFT ######## //
            finger.sprite = fingerDown;
            yield return new WaitForSeconds(0.3F);

            arrow.sprite = arrowLeft;
            arrow.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.15F);

            LeanTween.moveLocalX(animationObject, endingX * -1.0F, 0.55F)
                .setEase(LeanTweenType.easeInOutCubic)
                .setOnComplete(() =>
                {
                    finger.sprite = fingerUp;
                    arrow.gameObject.SetActive(false);
                });

            yield return new WaitForSeconds(1.25F);
            arrow.sprite = arrowRight;

            // ######## MOVE RIGHT ######## //
            finger.sprite = fingerDown;
            finger.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3F);

            arrow.sprite = arrowRight;
            arrow.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.15F);

            LeanTween.moveLocalX(animationObject, endingX, 0.55F)
                .setEase(LeanTweenType.easeInOutCubic)
                .setOnComplete(() =>
                {
                    finger.sprite = fingerUp;
                    arrow.gameObject.SetActive(false);
                });

            yield return new WaitForSeconds(1.0F);

            finger.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.2F);

            arrow.sprite = arrowLeft;
            animationObjRect.anchoredPosition = Vector2.right * startingX;
        }

        animationBegan = false; // Reset animation flag when loop ends
    }

    void OnEnable() => StartCoroutine(IEBeginAnimation());

    void OnDisable()
    {
        isAnimating = false; // Stop the loop
        LeanTween.cancel(animationObject); // Cancel active tweens
        animationBegan = false;

        // Optionally reset object positions and visibility here for a fresh start
        finger.gameObject.SetActive(false);
        arrow.gameObject.SetActive(false);
    }

    void Awake()
    {
        animationObject.TryGetComponent(out animationObjRect);
    }

    void Update() => WatchForDragged();

    // A minimum drag distance must be met for it to recognized as valid horizontal drag
    // regardless of the drag direction. It should increment the drag progress counter
    // until it reaches 100% before stopping this drag tutorial.
    private void WatchForDragged()
    {
        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPosition = touch.position;
                    isDragging = true;
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        currentTouchPosition = touch.position;
                        float distance = currentTouchPosition.x - startTouchPosition.x;

                        // Check if the swipe distance is greater than the threshold
                        if (Mathf.Abs(distance) > swipeThreshold)
                        {
                            dragProgress = Mathf.Clamp(dragProgress + 1, 0, 100); // Increment the counter
                            //Debug.Log("Drag Progress: " + dragProgress + "%");

                            // If the current progress is closer or greater than 90, set it to 100
                            if (dragProgress >= 90)
                                dragProgress = 100;

                            dragProgressMeter.value = dragProgress;

                            if (dragProgress == 100)
                            {
                                onDragTutorialComplete?.Invoke();
                                isAnimating = false;
                                gameObject.SetActive(false);
                            }

                            // Reset start position for the next incremental calculation
                            startTouchPosition = currentTouchPosition;
                        }
                    }
                    break;

                case TouchPhase.Ended:
                    isDragging = false;
                    break;
            }
        }
    }
}
