using UnityEngine;
using UnityEngine.Events;

public class LevelMenuPageClickedEvent : UnityEvent<LevelSelectPageData> {}

public class LevelMenuNotifier : MonoBehaviour
{
    private static readonly LevelMenuPageClickedEvent pageClickedEvent = new LevelMenuPageClickedEvent();

    public static void NotifyLevelPageClicked(LevelSelectPageData sender) => pageClickedEvent.Invoke(sender);

    // Add an event listener
    public static void BindLevelPageClickEvent(UnityAction<LevelSelectPageData> eventAction)
    {
        pageClickedEvent.AddListener(eventAction);
    }

    // Remove an event listener
    public static void UnbindLevelPageClickEvent(UnityAction<LevelSelectPageData> eventAction)
    {
        pageClickedEvent.RemoveListener(eventAction);
    }
}