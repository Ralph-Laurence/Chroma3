using UnityEngine.Events;

public class CommonActionEvent : UnityEvent<CommonEventTags> {}
public class CommonEventNotifier
{
    private readonly static CommonActionEvent _event = new();
    public static void NotifyObserver(CommonEventTags eventTag) => _event.Invoke(eventTag);
    public static void BindEvent(UnityAction<CommonEventTags> actionEvent) => _event.AddListener(actionEvent);
    public static void UnbindEvent(UnityAction<CommonEventTags> actionEvent) => _event.RemoveListener(actionEvent);
}