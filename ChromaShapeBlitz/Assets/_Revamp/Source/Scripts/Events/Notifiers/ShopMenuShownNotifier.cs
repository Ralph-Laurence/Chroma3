using UnityEngine.Events;

public class ShopMenuShownNotifier
{
    private class ShopMenuShownNotifierEvent : UnityEvent{}
    private static readonly ShopMenuShownNotifierEvent _event = new();
    public static void BindEvent(UnityAction action) => _event.AddListener(action);
    public static void UnbindEvent(UnityAction action) => _event.RemoveListener(action);
    public static void NotifyObserver() => _event.Invoke();
}