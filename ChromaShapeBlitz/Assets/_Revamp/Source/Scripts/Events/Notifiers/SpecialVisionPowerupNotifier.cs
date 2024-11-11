using UnityEngine.Events;

public class SpecialVisionPowerupNotifier
{
    private class SpecialVisionPowerupEvent : UnityEvent<int> {}
    private static readonly SpecialVisionPowerupEvent _event = new();

    public static void NotifyObserver(int effectValue) => _event.Invoke(effectValue);
    public static void BindObserver(UnityAction<int> observer) => _event.AddListener(observer);
    public static void UnbindObserver(UnityAction<int> observer) => _event.RemoveListener(observer);
}