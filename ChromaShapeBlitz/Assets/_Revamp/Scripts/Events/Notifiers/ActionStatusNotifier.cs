using UnityEngine;
using UnityEngine.Events;

public class ActionStatusEvent : UnityEvent<int> {}
public class ActionStatusNotifier
{
    private static readonly ActionStatusEvent _event = new ActionStatusEvent();
    public static void NotifyObserver(int status) => _event.Invoke(status);

    // Add an event listener
    public static void BindEvent(UnityAction<int> eventAction)
    {
        _event.AddListener(eventAction);
    }

    // Remove an event listener
    public static void UnbindEvent(UnityAction<int> eventAction)
    {
        _event.RemoveListener(eventAction);
    }
}