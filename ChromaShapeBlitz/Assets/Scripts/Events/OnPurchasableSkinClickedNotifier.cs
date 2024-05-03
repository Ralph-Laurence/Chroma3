using UnityEngine;
using UnityEngine.Events;

public class PurchasableSkinItemClickedEvent : UnityEvent<PurchasableSkinItem> { }

public class OnPurchasableSkinClickedNotifier : MonoBehaviour
{
    public static PurchasableSkinItemClickedEvent Event = new PurchasableSkinItemClickedEvent();

    public static void Publish(PurchasableSkinItem sender) => Event.Invoke(sender);
}
