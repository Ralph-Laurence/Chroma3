using UnityEngine;
using UnityEngine.Events;

public class DestinationReachedEvent : UnityEvent { }

public class OnDestinationReachedNotifier : MonoBehaviour
{
    public static DestinationReachedEvent Event = new DestinationReachedEvent();

    public static void Publish() => Event.Invoke();
}