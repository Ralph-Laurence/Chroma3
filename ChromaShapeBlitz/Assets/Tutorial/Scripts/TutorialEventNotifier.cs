using UnityEngine;
using UnityEngine.Events;

public class TutorialEventNotifier : MonoBehaviour
{
    private class TutorialEvent : UnityEvent<string, object>{}
    private static readonly TutorialEvent _event = new();

    public static void NotifyObserver(string reciever, object data) => _event.Invoke(reciever, data);

    public static void BindObserver(UnityAction<string, object> observer) => _event.AddListener(observer);

    public static void UnbindObserver(UnityAction<string, object> observer) => _event.RemoveListener(observer);
}