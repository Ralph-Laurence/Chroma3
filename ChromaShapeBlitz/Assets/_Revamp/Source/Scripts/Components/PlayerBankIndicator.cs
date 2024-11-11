using UnityEngine;

[RequireComponent(typeof(PlayerBankAnimation))]
public class PlayerBankIndicator : MonoBehaviour
{
    [SerializeField] private CurrencyType currency;

    private void Awake()
    {
        if (TryGetComponent(out PlayerBankAnimation anim))
            anim.Currency = currency;
    }
}
