using UnityEngine;

/// <summary>
/// Attach this script into the Dialog Overlay
/// </summary>
public class OnSkinPurchasedObserver : MonoBehaviour
{
    [SerializeField] private AudioClip purchasedOnCoinSfx;
    [SerializeField] private AudioClip purchasedOnGemSfx;

    private SfxManager sfxManager;

    void Awake()
    {
        sfxManager = SfxManager.Instance;
    }

    private void OnEnable()  => OnSkinPurchasedNotifier.Event.AddListener(Subscribe);

    private void OnDisable() => OnSkinPurchasedNotifier.Event.RemoveListener(Subscribe);

    public void Subscribe(BlockSkinShopItemInfo info)
    {
        switch (info.Cost)
        {
            case CurrencyType.Coin:
                sfxManager.PlaySfx(purchasedOnCoinSfx);
                break;

            case CurrencyType.Gem:
                sfxManager.PlaySfx(purchasedOnGemSfx);
                break;
        }

        gameObject.SetActive(false);
    }
}
