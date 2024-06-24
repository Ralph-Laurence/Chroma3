using TMPro;
using UnityEngine;

public class PlayerCurrencyIndicator : MonoBehaviour
{
    private int lastAppliedAmount;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CurrencyType currencyType;

    void OnEnable() => PlayerCurrencyNotifier.BindEvent(ObserveCurrencyUpdates);
    void OnDisable() => PlayerCurrencyNotifier.UnbindEvent(ObserveCurrencyUpdates);
    
    private void ObserveCurrencyUpdates(PlayerCurrencyEventArgs e)
    {
        // Prevent unnecessary updates
        if (e.Currency != currencyType || lastAppliedAmount == e.Amount)
            return;

        // Apply the updated values          
        text.text = e.Amount.ToString();
        lastAppliedAmount = e.Amount;
    }
}
