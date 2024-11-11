using UnityEngine.Events;

public enum DialogEventNames
{
    BuyBackgroundDialogResultShown,
    BuySkinsDialogResultShown,
    BuyPowerupsDialogResultShown,

    BuyBackgroundDialogResultHidden,
    BuySkinsDialogResultHidden,
    BuyPowerupsDialogResultHidden,
}

public class DialogEventNotifier
{
    public class DialogEvent : UnityEvent<DialogEventNames> {}
    private readonly static DialogEvent _event = new();
    public static void NotifyObserver(DialogEventNames eventName) => _event.Invoke(eventName);
    public static void BindObserver(UnityAction<DialogEventNames> actionEvent) => _event.AddListener(actionEvent);
    public static void UnbindObserver(UnityAction<DialogEventNames> actionEvent) => _event.RemoveListener(actionEvent);
}