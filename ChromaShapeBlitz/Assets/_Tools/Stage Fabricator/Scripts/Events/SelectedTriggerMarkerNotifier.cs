using UnityEngine;
using UnityEngine.Events;

public class SelectedTriggerMarkerResetEvent : UnityEvent { }

public class SelectedTriggerMarkerNotifier
{
    public static SelectedTriggerMarkerResetEvent Event = new SelectedTriggerMarkerResetEvent();

    public static void NotifyReset() => Event.Invoke();
}