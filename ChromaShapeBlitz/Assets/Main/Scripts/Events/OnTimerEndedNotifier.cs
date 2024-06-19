using UnityEngine;
using UnityEngine.Events;

public class TimerEndedEvent : UnityEvent {}

public class OnTimerEndedNotifier : MonoBehaviour
{
    public static TimerEndedEvent Event = new TimerEndedEvent();

    public static void Publish() => Event.Invoke();
}