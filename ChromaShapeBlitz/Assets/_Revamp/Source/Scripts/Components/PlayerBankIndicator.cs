using TMPro;
using UnityEngine;

[RequireComponent(typeof(PlayerBankAnimation))]
public class PlayerBankIndicator : MonoBehaviour
{
    [SerializeField] private CurrencyType currency;
    [SerializeField] private TextMeshProUGUI tmpText;

    private void Awake()
    {
        if (TryGetComponent(out PlayerBankAnimation anim))
            anim.Currency = currency;
    }

    public void RenderValue(int value) => tmpText.text = value.ToString();
}
