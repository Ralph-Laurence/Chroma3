using UnityEngine.Events;

public class BlockSequenceFillCompletedEvent : UnityEvent{}

/// <summary>
/// This notifies when a Block Sequence Controller completed filling its blocks.
/// </summary>
public class OnBlockSequenceFillCompleted
{
    private static readonly BlockSequenceFillCompletedEvent _event = new BlockSequenceFillCompletedEvent();
    public static void NotifyObserver() => _event.Invoke();

    // Add an event listener
    public static void BindEvent(UnityAction eventAction)
    {
        _event.AddListener(eventAction);
    }

    // Remove an event listener
    public static void UnbindEvent(UnityAction eventAction)
    {
        _event.RemoveListener(eventAction);
    }
}