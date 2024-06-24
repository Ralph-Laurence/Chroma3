using UnityEngine;

public class ShopMenuController : MonoBehaviour
{
    private GameSessionManager gsm;

    void Awake()
    {
        gsm = GameSessionManager.Instance;
    }

    void OnEnable()
    {
        if (gsm == null)
            return;

        var userData = gsm.UserSessionData;

        if (userData == null)
            return;

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
    }
}