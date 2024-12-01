using UnityEngine.Events;

public class GiftItemCardClickNotifier
{
    private class GiftItemCardClickEvent : UnityEvent<DailyGiftItemCard> {}
    private static readonly GiftItemCardClickEvent _event = new();

    public static void NotifyObserver(DailyGiftItemCard sender) => _event.Invoke(sender);
    public static void BindObserver(UnityAction<DailyGiftItemCard> observer)
    {
        _event.AddListener(observer);
    }
    public static void UnbindObserver(UnityAction<DailyGiftItemCard> observer)
    {
        _event.RemoveListener(observer);
    } 
}