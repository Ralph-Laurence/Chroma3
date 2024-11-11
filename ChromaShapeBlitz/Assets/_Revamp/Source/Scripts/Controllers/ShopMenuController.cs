using UnityEngine;

public class ShopMenuController : MonoBehaviour
{
    private GameSessionManager gsm;
    private UserData userData;
    private RectTransform selfRect;

    private bool isInitialized;
    private readonly Vector3 ScaleDownSize = new(0.0001F, 0.0001F, 1);

    void Awake() => Initialize();

    void OnEnable()
    {
        Initialize();
        Show();
    }

    private void Initialize()
    {
        if (isInitialized)
            return;

        gsm = GameSessionManager.Instance;
        userData = gsm.UserSessionData;

        TryGetComponent(out selfRect);

        PlayerCurrencyNotifier.NotifyObserver(new PlayerCurrencyEventArgs
        {
            Amount = userData.TotalCoins,
            Currency = CurrencyType.Coin,
        });

        PlayerCurrencyNotifier.NotifyObserver(new PlayerCurrencyEventArgs
        {
            Amount = userData.TotalGems,
            Currency = CurrencyType.Gem,
        });

        isInitialized = true;
    }

    public void Close()
    {
        if (selfRect == null)
            return;

        LeanTween.scale(selfRect, ScaleDownSize, 0.25F).setOnComplete(() => gameObject.SetActive(false));
    }

    private void Show()
    {
        if (selfRect == null)
            return;

        LeanTween.scale(selfRect, Vector3.one, 0.25F)
                 .setOnComplete(() => {
                    ShopMenuShownNotifier.NotifyObserver();
                 });
    }
}