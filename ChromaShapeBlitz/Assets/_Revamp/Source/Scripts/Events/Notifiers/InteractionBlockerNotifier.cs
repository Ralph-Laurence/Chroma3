using UnityEngine.Events;

public class InteractionBlockerNotifier
{
    private class InteractionBlockerEvent : UnityEvent<bool>{}
    private static readonly InteractionBlockerEvent _event = new();

    public static void BindObserver(UnityAction<bool> observer) => _event.AddListener(observer);
    public static void UnbindObserver(UnityAction<bool> observer) => _event.RemoveListener(observer);
    public static void NotifyObserver(bool show) => _event.Invoke(show);
}