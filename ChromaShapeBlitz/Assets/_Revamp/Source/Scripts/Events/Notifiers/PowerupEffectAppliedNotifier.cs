using UnityEngine.Events;

public class PowerupEffectAppliedNotifier
{
    private class PowerupEffectAppliedEvent : UnityEvent<HotBarSlot, PowerupEffectData> {}
    private static readonly PowerupEffectAppliedEvent _event = new();

    public static void NotifyObserver(HotBarSlot sender, PowerupEffectData powerupData)
    {
        _event.Invoke(sender, powerupData);
    }
    public static void BindObserver(UnityAction<HotBarSlot, PowerupEffectData> powerupData)
    {
        _event.AddListener(powerupData);
    }
    public static void UnbindObserver(UnityAction<HotBarSlot, PowerupEffectData> powerupData)
    {
        _event.RemoveListener(powerupData);
    }
}