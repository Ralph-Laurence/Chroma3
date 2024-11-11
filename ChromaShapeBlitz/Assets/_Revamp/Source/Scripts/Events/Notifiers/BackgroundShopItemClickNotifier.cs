using UnityEngine.Events;

public class BackgroundShopItemClickEvent : UnityEvent<BackgroundShopItemCard> {}

public class BackgroundShopItemClickNotifier
{
    private static readonly BackgroundShopItemClickEvent _event = new();
    public static void NotifyObserver(BackgroundShopItemCard sender) => _event.Invoke(sender);
    public static void BindObserver(UnityAction<BackgroundShopItemCard> action) 
    {
        _event.AddListener(action);
    }

    public static void UnbindObserver(UnityAction<BackgroundShopItemCard> action)
    {
        _event.RemoveListener(action);
    }
}