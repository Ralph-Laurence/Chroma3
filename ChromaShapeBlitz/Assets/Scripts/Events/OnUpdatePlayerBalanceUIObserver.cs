using TMPro;
using UnityEngine;

/// <summary>
/// This watches for an event that happens when a skin is about to be purchased,
/// such as clicking the "BUY" button in the purchase dialog.
/// 
/// This script must be attached into the Purchase Dialog
/// </summary>
public class OnUpdatePlayerBalanceUIObserver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalCoinsText;
    [SerializeField] private TextMeshProUGUI totalGemsText;

    private void OnEnable() => OnUpdatePlayerBalanceUINotifier.Event.AddListener(Subscribe);

    private void OnDisable() => OnUpdatePlayerBalanceUINotifier.Event.RemoveListener(Subscribe);

    public void Subscribe(PlayerBalance playerBalance)
    {
        totalCoinsText.text = playerBalance.TotalCoins.ToString();
        totalGemsText.text  = playerBalance.TotalGems.ToString();
    }
}
