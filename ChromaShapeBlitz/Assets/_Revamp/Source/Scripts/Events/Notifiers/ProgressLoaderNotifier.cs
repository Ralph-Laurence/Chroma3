using UnityEngine.Events;

public enum ProgressLoaderTypes
{
    FourSegmentRound,
    IndefiniteBar
}

public enum ProgressLoaderActions
{ 
    Begin,
    End
}

public class ProgressLoaderNotifier
{
    private class ProgressLoaderEvent : UnityEvent<ProgressLoaderTypes, ProgressLoaderActions> { }
    private static readonly ProgressLoaderEvent _event = new();

    public static void NotifyObserver(ProgressLoaderTypes type, ProgressLoaderActions action)
    {
        _event.Invoke(type, action);
    }
    public static void BindObserver(UnityAction<ProgressLoaderTypes, ProgressLoaderActions> observer)
    {
        _event.AddListener(observer);
    }
    public static void UnbindObserver(UnityAction<ProgressLoaderTypes, ProgressLoaderActions> observer)
    {
        _event.RemoveListener(observer);
    }

    public static void NotifyFourSegment(bool show)
    {
        if (!show)
        {
            NotifyObserver(ProgressLoaderTypes.FourSegmentRound, ProgressLoaderActions.End);
            return;
        }

        NotifyObserver(ProgressLoaderTypes.FourSegmentRound, ProgressLoaderActions.Begin);
    }

    public static void NotifyIndefiniteBar(bool show)
    {
        if (!show)
        {
            NotifyObserver(ProgressLoaderTypes.IndefiniteBar, ProgressLoaderActions.End);
            return;
        }

        NotifyObserver(ProgressLoaderTypes.IndefiniteBar, ProgressLoaderActions.Begin);
    }
}