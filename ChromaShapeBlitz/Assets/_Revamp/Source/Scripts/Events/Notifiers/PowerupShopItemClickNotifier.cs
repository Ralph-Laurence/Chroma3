using UnityEngine.Events;

public class PowerupShopItemClickEvent : UnityEvent<PowerupShopItemCard> { }

public class PowerupShopItemClickNotifier
{
    private static readonly PowerupShopItemClickEvent _event = new();
    public static void NotifyObserver(PowerupShopItemCard sender) => _event.Invoke(sender);
    public static void BindEvent(UnityAction<PowerupShopItemCard> action)
    {
        _event.AddListener(action);
    }

    public static void UnbindEvent(UnityAction<PowerupShopItemCard> action)
    {
        _event.RemoveListener(action);
    }
}