using UnityEngine.Events;

public class NavButtonSelectedEvent : UnityEvent<int> {}

public class NavButtonSelectedNotifier
{
    private static readonly NavButtonSelectedEvent _event = new();

    public static void NotifyObserver(int sender) => _event.Invoke(sender);
    public static void BindEvent(UnityAction<int> action) => _event.AddListener(action);
    public static void UnbindEvent(UnityAction<int> action) => _event.RemoveListener(action);
}