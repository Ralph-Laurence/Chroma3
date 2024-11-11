using UnityEngine;
using UnityEngine.Events;

public enum InventoryHotbarEventNames
{
    ItemEnqueued,
    ItemDequeued
}

public class InventoryHotbarNotifier : MonoBehaviour
{
    private class InventoryHotbarEvent : UnityEvent<InventoryHotbarEventNames> { }
    private static readonly InventoryHotbarEvent _event = new();

    public static void BindObserver(UnityAction<InventoryHotbarEventNames> observer) => _event.AddListener(observer);
    public static void UnbindObserver(UnityAction<InventoryHotbarEventNames> observer) => _event.RemoveListener(observer);
    public static void NotifyObserver(InventoryHotbarEventNames eventName) => _event.Invoke(eventName);
}
