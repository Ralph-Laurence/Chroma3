using UnityEngine;
using UnityEngine.Events;

public class StageSelectMenuCloseEvent : UnityEvent<LevelSelectPage> {}

public class OnStageSelectMenuCloseNotifier : MonoBehaviour
{
    private static readonly StageSelectMenuCloseEvent _event = new StageSelectMenuCloseEvent();

    public static void NotifyObserver(LevelSelectPage sender) => _event.Invoke(sender);

    // Add an event listener
    public static void BindEvent(UnityAction<LevelSelectPage> eventAction)
    {
        _event.AddListener(eventAction);
    }

    // Remove an event listener
    public static void UnbindEvent(UnityAction<LevelSelectPage> eventAction)
    {
        _event.RemoveListener(eventAction);
    }
}