using UnityEngine;

/// <summary>
/// This watches for an event that happens when a skin is about to be purchased,
/// such as clicking the "BUY" button in the purchase dialog.
/// 
/// This script must be attached into the Purchase Dialog
/// </summary>
public class OnSkinPurchasedObserver : MonoBehaviour
{
    [SerializeField] private PurchaseSkinDialog dialog;

    private void OnEnable()  => OnSkinPurchasedNotifier.Event.AddListener(Subscribe);

    private void OnDisable() => OnSkinPurchasedNotifier.Event.RemoveListener(Subscribe);

    public void Subscribe()
    {
        dialog.Close();
    }
}
