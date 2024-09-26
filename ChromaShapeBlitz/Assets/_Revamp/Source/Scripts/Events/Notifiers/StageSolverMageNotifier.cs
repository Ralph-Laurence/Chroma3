using UnityEngine.Events;

public class StageSolverMageNotifier
{
    private class StageSolverMageEvent : UnityEvent<int, StageVariant, HotBarSlot, PowerupEffectData> {}
    private static readonly StageSolverMageEvent _event = new();

    public static void NotifyObserver
    (
        int mageType,
        StageVariant stageVariant,
        HotBarSlot sender,
        PowerupEffectData effectData
    )
    {
        _event.Invoke(mageType, stageVariant, sender, effectData);
    }
    public static void BindObserver(UnityAction<int, StageVariant, HotBarSlot, PowerupEffectData> observer)
    {
        _event.AddListener(observer);
    }
    public static void UnbindObserver(UnityAction<int, StageVariant, HotBarSlot, PowerupEffectData> observer)
    {
        _event.RemoveListener(observer);
    }
}