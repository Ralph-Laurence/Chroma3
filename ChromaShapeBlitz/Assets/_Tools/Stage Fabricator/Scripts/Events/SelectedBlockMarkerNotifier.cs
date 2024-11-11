using UnityEngine;
using UnityEngine.Events;

public class SelectedBlockMarkerResetEvent : UnityEvent { }

public class SelectedBlockMarkerNotifier
{
    public static SelectedBlockMarkerResetEvent Event = new SelectedBlockMarkerResetEvent();

    public static void NotifyReset() => Event.Invoke();
}