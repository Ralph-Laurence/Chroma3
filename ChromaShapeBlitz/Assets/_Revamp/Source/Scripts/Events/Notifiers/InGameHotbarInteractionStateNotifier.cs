using UnityEngine.Events;

public class InGameHotbarInteractionStateNotifier
{
    private class InGameHotbarEvent : UnityEvent<bool> {}
    private static readonly InGameHotbarEvent _event = new();
    public static void BindObserver(UnityAction<bool> observer) => _event.AddListener(observer);
    public static void UnbindObserver(UnityAction<bool> observer) => _event.RemoveListener(observer);
    public static void NotifyObserver(bool blockInteraction) => _event.Invoke(blockInteraction);
}