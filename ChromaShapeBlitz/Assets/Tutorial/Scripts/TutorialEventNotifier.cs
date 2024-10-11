using UnityEngine;
using UnityEngine.Events;

public class TutorialEventNotifier : MonoBehaviour
{
    private class TutorialEvent : UnityEvent<string, string>{}
    private static readonly TutorialEvent _event = new();

    public static void NotifyObserver(string reciever, string data)
    {
        _event.Invoke(reciever, data);
    }

    public static void BindObserver(UnityAction<string, string> observer)
    {
        _event.AddListener(observer);
    }

    public static void UnbindObserver(UnityAction<string, string> observer)
    {
        _event.RemoveListener(observer);
    }
}