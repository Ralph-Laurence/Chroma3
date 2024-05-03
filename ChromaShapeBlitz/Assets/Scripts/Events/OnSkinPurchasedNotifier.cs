using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This event happens when a skin is about to be purchased,
/// such as before showing the purchase dialog.
/// </summary>
public class SkinPurchasedEvent : UnityEvent { }

public class OnSkinPurchasedNotifier : MonoBehaviour
{
    public static SkinPurchasedEvent Event = new SkinPurchasedEvent();

    public static void Publish() => Event.Invoke();
}
