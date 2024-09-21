using UnityEngine.Events;

public class HintMarkerNotifier
{
    private class HintMarkerEvent : UnityEvent<StageVariant> {};
    private static HintMarkerEvent _event = new();

    public static void NotifyObserver(StageVariant sender) => _event.Invoke(sender);
    public static void BindObserver(UnityAction<StageVariant> observer) => _event.AddListener(observer);
    public static void UnbindObserver(UnityAction<StageVariant> observer) => _event.RemoveListener(observer);
}