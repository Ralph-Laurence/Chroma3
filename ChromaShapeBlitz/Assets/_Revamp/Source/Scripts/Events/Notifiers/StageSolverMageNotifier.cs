using UnityEngine.Events;

public class StageSolverMageNotifier
{
    private class StageSolverMageEvent : UnityEvent<int, StageVariant> {}
    private static readonly StageSolverMageEvent _event = new();

    public static void NotifyObserver(int mageType, StageVariant stageVariant)
    {
        _event.Invoke(mageType, stageVariant);
    }
    public static void BindObserver(UnityAction<int, StageVariant> observer)
    {
        _event.AddListener(observer);
    }
    public static void UnbindObserver(UnityAction<int, StageVariant> observer)
    {
        _event.RemoveListener(observer);
    }
}