using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HorizontalScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public RectTransform content;
    public ScrollRect scrollRect;
    
    [Header("The snapping and scrolling depends on this.")]
    public float SmoothTime = 0.12F;

    [Header("Shrink items when scrolling.")]
    public float MinItemScale = 0.50F;

    [Header("Background appearance after scroll")]
    [SerializeField] private Image backgroundUI;
    [SerializeField] private Sprite backgroundOnEasy;
    [SerializeField] private Sprite backgroundOnNormal;
    [SerializeField] private Sprite backgroundOnHard;

    [Space(10)] [Header("Behaviours")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button nextButton;

    [Header("Scroll location tracking")]
    [SerializeField] private Image[] trackers;
    [SerializeField] private Sprite trackerOn;
    [SerializeField] private Sprite trackerOff;

    private readonly Vector2 trackerSizeOn = Vector2.one * 24.0F;
    private readonly Vector2 trackerSizeOff = Vector2.one * 16.0F;

    private const int PAGE_INDEX_EASY = 0;
    private const int PAGE_INDEX_NORMAL = 1;
    private const int PAGE_INDEX_HARD = 2;
    private int itemCount;

    private float[] positions;
    private int targetIndex = 0;
    private bool isScrolling = false;
    private bool isDragging = false;
    private float velocity = 0.0F;

    void Start()
    {
        itemCount = content.childCount;
        positions = new float[itemCount];
        float step = 1f / (itemCount - 1);

        for (int i = 0; i < itemCount; i++)
        {
            positions[i] = step * i;
        }

        // Center the first item
        scrollRect.horizontalNormalizedPosition = positions[0];
    }

    void Update()
    {
        if (isScrolling && !isDragging)
        {
            DoScroll();
        }

        AdjustItemScales();
    }

    public void ScrollLeft()
    {
        if (targetIndex > 0)
        {
            targetIndex--;
            isScrolling = true;
        }
    }

    public void ScrollRight()
    {
        if (targetIndex < positions.Length - 1)
        {
            targetIndex++;
            isScrolling = true;
        }
    }

    private void DoScroll()
    {
        float targetPos = positions[targetIndex];
        scrollRect.horizontalNormalizedPosition = Mathf.SmoothDamp(scrollRect.horizontalNormalizedPosition, targetPos, ref velocity, SmoothTime);

        // Stop scrolling if we're very close to the target position
        if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetPos) < 0.001f)
        {
            scrollRect.horizontalNormalizedPosition = targetPos;
            isScrolling = false;

            // The scrolling was done
            OnAfterScrollAndSnapped(targetIndex);
        }

        UpdateScrollButtons(targetIndex);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        isScrolling = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        SnapToClosest();
    }

    private void SnapToClosest()
    {
        float nearestPos = Mathf.Infinity;
        for (int i = 0; i < positions.Length; i++)
        {
            float distance = Mathf.Abs(scrollRect.horizontalNormalizedPosition - positions[i]);
            if (distance < nearestPos)
            {
                nearestPos = distance;
                targetIndex = i;
            }
        }
        isScrolling = true;
    }

    private void AdjustItemScales()
    {
        for (int i = 0; i < content.childCount; i++)
        {
            RectTransform item = content.GetChild(i) as RectTransform;
            float distance = Mathf.Abs(scrollRect.horizontalNormalizedPosition - positions[i]);
            float scale = Mathf.Lerp(1f, MinItemScale, distance / (1f / (content.childCount - 1)));

            item.localScale = new Vector3(scale, scale, 1f);
        }
    }

    private void OnAfterScrollAndSnapped(int pageIndex)
    {
        switch (pageIndex)
        {
            case PAGE_INDEX_EASY:
                backgroundUI.sprite = backgroundOnEasy;
                break;
            
            case PAGE_INDEX_NORMAL:
                backgroundUI.sprite = backgroundOnNormal;
                break;

            case PAGE_INDEX_HARD:
                backgroundUI.sprite = backgroundOnHard;
                break;
        }
    }

    private void UpdateScrollButtons(int pageIndex)
    {
        nextButton.gameObject.SetActive(pageIndex < (itemCount - 1));
        backButton.gameObject.SetActive(pageIndex > 0);

        UpdateTrackerLocation(pageIndex);
    }

    private void UpdateTrackerLocation(int pageIndex)
    {
        RectTransform rect;

        foreach (var tracker in trackers)
        {
            tracker.sprite = trackerOff;
            tracker.TryGetComponent(out rect);
            rect.sizeDelta = trackerSizeOff;
        }

        trackers[pageIndex].sprite = trackerOn;
        trackers[pageIndex].TryGetComponent(out rect);
        rect.sizeDelta = trackerSizeOn;
    }

    public void ResetScrollPosition()
    {
        targetIndex = 0;
        backgroundUI.sprite = backgroundOnEasy;
        scrollRect.horizontalNormalizedPosition = positions[0];

        nextButton.gameObject.SetActive(true);
        backButton.gameObject.SetActive(false);

        UpdateTrackerLocation(0);
    }

    public void NormalizeScrollPosition()
    {
        scrollRect.horizontalNormalizedPosition = 0.0F;
    }
}
