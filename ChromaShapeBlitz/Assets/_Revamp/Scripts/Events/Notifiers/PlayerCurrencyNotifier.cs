using UnityEngine;
using UnityEngine.Events;

public class PlayerCurrencyEvent : UnityEvent<PlayerCurrencyEventArgs> {}

public class PlayerCurrencyNotifier : MonoBehaviour
{
    private static PlayerCurrencyEvent _event = new PlayerCurrencyEvent();
    public static void NotifyObserver(PlayerCurrencyEventArgs e) => _event.Invoke(e);
    public static void BindEvent(UnityAction<PlayerCurrencyEventArgs> unityAction)
    {
        _event.AddListener(unityAction);
    }
    public static void UnbindEvent(UnityAction<PlayerCurrencyEventArgs> unityAction)
    {
        _event.RemoveListener(unityAction);
    }
}