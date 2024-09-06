using UnityEngine.Events;

public class NavContentPageShownNotifier
{
    private class NavContentPageShownEvent : UnityEvent<int> { }
    private static readonly NavContentPageShownEvent _event = new();

    public static void NotifyShown(int pageId) => _event.Invoke(pageId);
    public static void BindObserver(UnityAction<int> observer) => _event.AddListener(observer);
    public static void UnbindObserver(UnityAction<int> observer) => _event.RemoveListener(observer);
}