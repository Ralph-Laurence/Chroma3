using UnityEngine.Events;

public class BlockSkinShopItemClickEvent : UnityEvent<SkinShopItemCard> {}

public class BlockSkinShopItemClickNotifier
{
    private static readonly BlockSkinShopItemClickEvent _event = new();
    public static void NotifyObserver(SkinShopItemCard sender) => _event.Invoke(sender);
    public static void BindObserver(UnityAction<SkinShopItemCard> action) 
    {
        _event.AddListener(action);
    }

    public static void UnbindObserver(UnityAction<SkinShopItemCard> action)
    {
        _event.RemoveListener(action);
    }
}