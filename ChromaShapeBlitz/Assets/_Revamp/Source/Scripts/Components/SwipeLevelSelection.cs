using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeLevelSelection : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public float swipeThreshold = 50f; // Minimum distance to be considered a swipe
    public float swipeDuration = 0.5f; // Maximum duration to consider a swipe valid

    private Vector2 dragStartPos;
    private float dragStartTime;
    private bool isSwiping;

    public RectTransform levelMenu; // Assign your level menu RectTransform
    
    [SerializeField] private Button levelScrollNext;
    [SerializeField] private Button levelScrollBack;

    [SerializeField] private bool isSwipable;

    public void MakeSwipable(bool swipable) => isSwipable = swipable;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isSwipable)
            return;

        isSwiping = true;
        dragStartPos = eventData.position;
        dragStartTime = Time.time;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // No need to handle anything during the drag for swipe detection
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isSwipable)
            return;

        isSwiping = false;
        Vector2 dragEndPos = eventData.position;
        float dragEndTime = Time.time;
        float dragDistance = (dragEndPos - dragStartPos).magnitude;
        float dragTime = dragEndTime - dragStartTime;

        if (dragDistance > swipeThreshold && dragTime < swipeDuration)
        {
            Vector2 direction = dragEndPos - dragStartPos;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x > 0)
                    OnSwipeRight();
                else
                    OnSwipeLeft();
            }
        }
    }

    private void OnSwipeRight()
    {
        if (levelScrollNext.gameObject.activeInHierarchy)
            levelScrollNext.onClick.Invoke();
        // Debug.Log("Swiped Right");
        // Implement your logic to scroll the level menu to the right
        // Example: levelMenu.anchoredPosition += new Vector2(xValue, 0);
    }

    private void OnSwipeLeft()
    {
        if (levelScrollBack.gameObject.activeInHierarchy)
            levelScrollBack.onClick.Invoke();
        // Debug.Log("Swiped Left");
        // Implement your logic to scroll the level menu to the left
        // Example: levelMenu.anchoredPosition -= new Vector2(xValue, 0);
    }
}
