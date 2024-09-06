using UnityEngine.Events;

public class HotbarSlotSelectedNotifier
{
    private class HotbarSlotSelectedEvent : UnityEvent<int> { }

    private static readonly HotbarSlotSelectedEvent _event = new();
    public static void NotifyObserver(int slotIndex) => _event.Invoke(slotIndex);
    public static void BindObserver(UnityAction<int> observer) => _event.AddListener(observer);
    public static void UnbindObserver(UnityAction<int> observer) => _event.RemoveListener(observer);
}