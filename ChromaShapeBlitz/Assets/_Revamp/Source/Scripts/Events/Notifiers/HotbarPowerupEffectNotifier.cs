using UnityEngine.Events;

public struct PowerupEffectData
{
    public int PowerupId;
    public PowerupCategories Category;
    public int EffectValue;
}

public class HotbarPowerupEffectNotifier
{
    private class HotbarPowerupEffectEvent : UnityEvent<HotBarSlot, PowerupEffectData> {}
    private static readonly HotbarPowerupEffectEvent _event = new();
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
