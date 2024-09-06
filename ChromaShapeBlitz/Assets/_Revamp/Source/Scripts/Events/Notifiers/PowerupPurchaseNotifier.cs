using UnityEngine.Events;

public class PowerupPurchaseEvent : UnityEvent<PowerupShopItemCard> { }
public class PowerupPurchaseNotifier
{
    private static readonly PowerupPurchaseEvent _event = new();
    public static void NotifyObserver(PowerupShopItemCard sender) => _event.Invoke(sender);
    public static void BindEvent(UnityAction<PowerupShopItemCard> eventAction)
    {
        _event.AddListener(eventAction);
    }
    public static void UnbindEvent(UnityAction<PowerupShopItemCard> eventAction)
    {
        _event.RemoveListener(eventAction);
    }
}