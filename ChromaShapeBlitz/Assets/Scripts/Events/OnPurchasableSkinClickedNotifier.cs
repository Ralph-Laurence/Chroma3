using UnityEngine;
using UnityEngine.Events;

public class PurchasableSkinItemClickedEvent : UnityEvent<BlockSkinShopItem> { }

public class OnPurchasableSkinClickedNotifier : MonoBehaviour
{
    public static PurchasableSkinItemClickedEvent Event = new PurchasableSkinItemClickedEvent();

    public static void Publish(BlockSkinShopItem sender) => Event.Invoke(sender);
}
