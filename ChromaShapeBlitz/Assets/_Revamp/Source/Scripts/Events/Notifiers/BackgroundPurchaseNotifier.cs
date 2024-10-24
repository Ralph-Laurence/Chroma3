using UnityEngine.Events;

public class BackgroundPurchaseEvent : UnityEvent<BackgroundShopItemCard> {}
public class BackgroundPurchaseNotifier
{
    private static readonly BackgroundPurchaseEvent _event = new();
    public static void NotifyObserver(BackgroundShopItemCard sender) => _event.Invoke(sender);
    public static void BindEvent(UnityAction<BackgroundShopItemCard> eventAction)
    {
        _event.AddListener(eventAction);
    }
    public static void UnbindEvent(UnityAction<BackgroundShopItemCard> eventAction)
    {
        _event.RemoveListener(eventAction);
    }
}