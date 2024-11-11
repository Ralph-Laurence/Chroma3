using UnityEngine.Events;

public class InventoryListItemSelectedNotifier
{
    private class InventoryListItemSelectedEvent : UnityEvent<InventoryListItem> { }
    private static readonly InventoryListItemSelectedEvent _event = new();

    public static void NotifyObserver(InventoryListItem sender) => _event.Invoke(sender);
    public static void BindObserver(UnityAction<InventoryListItem> eventAction) => _event.AddListener(eventAction);
    public static void UnbindObserver(UnityAction<InventoryListItem> eventAction) => _event.RemoveListener(eventAction);
}