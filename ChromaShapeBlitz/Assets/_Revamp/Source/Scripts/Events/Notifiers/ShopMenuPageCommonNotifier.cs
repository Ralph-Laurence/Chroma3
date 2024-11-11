using UnityEngine.Events;

public enum ShopMenuPageEventNames
{
    SkinsPageFullyShown       = 0x01,
    BackgroundsPageFullyShown = 0x02,
    PowerupsPageFullyShown    = 0x03,
    InventoryPageFullyShown   = 0x04
}

public class ShopMenuPageCommonNotifier
{
    private class ShopMenuPageCommonEvent : UnityEvent<ShopMenuPageEventNames> { }

    private static readonly ShopMenuPageCommonEvent _event = new();

    public static void BindObserver(UnityAction<ShopMenuPageEventNames> observer)
    {
        _event.AddListener(observer);
    }

    public static void UnbindObserver(UnityAction<ShopMenuPageEventNames> observer)
    {
        _event.RemoveListener(observer);
    }

    public static void NotifyObserver(ShopMenuPageEventNames eventName)
    {
        _event.Invoke(eventName);
    }
}