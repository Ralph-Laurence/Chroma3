using TMPro;
using UnityEngine;

public class PlayerCurrencyIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CurrencyType currencyType;

    void OnEnable() => PlayerCurrencyNotifier.BindEvent(ObserveCurrencyUpdates);
    void OnDisable() => PlayerCurrencyNotifier.UnbindEvent(ObserveCurrencyUpdates);

    private void ObserveCurrencyUpdates(PlayerCurrencyEventArgs e)
    {
        if (e.Currency != currencyType)
            return;
            
        Debug.LogWarning("This is called: " + e.Amount);
        text.text = e.Amount.ToString();
    }
}
