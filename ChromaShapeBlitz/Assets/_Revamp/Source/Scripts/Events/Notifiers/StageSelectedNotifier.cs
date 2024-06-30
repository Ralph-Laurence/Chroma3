using UnityEngine;
using UnityEngine.Events;

public class StageSelectedEvent : UnityEvent<StageSelectedEventArgs> {}

public class StageSelectedNotifier : MonoBehaviour
{
    private static StageSelectedEvent _event = new StageSelectedEvent();

    public static void NotifyObserver(StageSelectedEventArgs args) => _event.Invoke(args);

    // Add an event listener
    public static void BindEvent(UnityAction<StageSelectedEventArgs> eventAction)
    {
        _event.AddListener(eventAction);
    }

    // Remove an event listener
    public static void UnbindEvent(UnityAction<StageSelectedEventArgs> eventAction)
    {
        _event.RemoveListener(eventAction);
    }
}