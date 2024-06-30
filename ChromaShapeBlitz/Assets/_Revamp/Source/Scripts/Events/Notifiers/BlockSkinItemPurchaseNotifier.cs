using UnityEngine.Events;

public class BlockSkinItemPurchaseEvent : UnityEvent<SkinShopItemCard> {}
public class BlockSkinItemPurchaseNotifier
{
    private static readonly BlockSkinItemPurchaseEvent _event = new();
    public static void NotifyObserver(SkinShopItemCard sender) => _event.Invoke(sender);
    public static void BindEvent(UnityAction<SkinShopItemCard> eventAction)
    {
        _event.AddListener(eventAction);
    }
    public static void UnbindEvent(UnityAction<SkinShopItemCard> eventAction)
    {
        _event.RemoveListener(eventAction);
    }
}