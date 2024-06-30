using UnityEngine.Events;

public class StageFillCompletedEvent : UnityEvent{}

/// <summary>
/// This notifies when a Stage's required sequences have all been filled.
/// </summary>
public class OnStageFilled
{
    private static readonly StageFillCompletedEvent _event = new StageFillCompletedEvent();
    public static void NotifyObserver() => _event.Invoke();

    // Add an event listener
    public static void BindEvent(UnityAction eventAction)
    {
        _event.AddListener(eventAction);
    }

    // Remove an event listener
    public static void UnbindEvent(UnityAction eventAction)
    {
        _event.RemoveListener(eventAction);
    }
}