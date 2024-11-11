using System;
using TMPro;
using UnityEditor;
using UnityEngine;

public class PlayerCurrencyIndicator : MonoBehaviour
{
    private int lastAppliedAmount;

    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private CurrencyType currencyType;

    [Header("For animations", order = 1)]
    [Space(5, order = 2)]
    [Header("Transform inside the canvas with higher sibling index", order = 3)]
    [SerializeField] private Transform animationParent;
    [SerializeField] private GameObject animationPrefab;
    [SerializeField] private float numberSpinDuration = 0.65F;
    [SerializeField] private Color colorOnDecrease = new(0.96F, 0.13F, 0.28F, 1.0F);
    [SerializeField] private Color colorOnIncrease = new(0.14F, 0.85F, 0.03F, 1.0F);

    private Color defaultTextColor;

    void Start()
    {
        defaultTextColor = textMesh.color;
    }

    void OnEnable() => PlayerCurrencyNotifier.BindEvent(ObserveCurrencyUpdates);
    void OnDisable() => PlayerCurrencyNotifier.UnbindEvent(ObserveCurrencyUpdates);
    
    private void ObserveCurrencyUpdates(PlayerCurrencyEventArgs e)
    {
        // Prevent unnecessary updates by checking if the properties
        // from the event recieved matched this object's properties
        if (e.Currency != currencyType || lastAppliedAmount == e.Amount)
            return;

        if (!e.Animate)
        {
            // Directly apply the text without animation
            textMesh.text = e.Amount.ToString();
        }
        else
        {
            if (animationParent != null)
                AnimateDecrease(e);
        }

        lastAppliedAmount = e.Amount;
    }

    private void AnimateDecrease(PlayerCurrencyEventArgs e)
    {
        // We skip the animation if the event was fired but
        // this object is not the subject for animation
         if (e.Currency != currencyType || string.IsNullOrEmpty(textMesh.text))
            return;

        var initial = Convert.ToInt32(textMesh.text);

        if (initial < 0)
            return;

        // Create a UI particle gold coin or gem fading out while sliding down
        Instantiate(animationPrefab, animationParent)
                .TryGetComponent(out PlayerCurrencyParticleAnimator anim);

        // This is the difference between the old amount vs new amount
        var reduced = initial - e.Amount;

        var spawnPosition = transform.position;
        spawnPosition.y -= anim.SpawnYOffset;

        anim.transform.position = spawnPosition;

        anim.SetParams(reduced, e.Currency, e.AnimationType);
        anim.Play();
        
        // Animate the GUI text
        
        AnimateNumbers(initial, e.Amount, e.AnimationType);
    }

    private void AnimateNumbers(int from, int to, int animType)
    {
        textMesh.color = (animType == PlayerCurrencyParticleAnimator.ANIMATION_MODE_DECREASE_AMOUNT)
                       ? colorOnDecrease
                       : colorOnIncrease;

        LeanTween.value
        (
            gameObject, 
            (v) => textMesh.text = Convert.ToInt32(v).ToString(), // callback
            from,
            to,
            numberSpinDuration
        )
        .setOnComplete(() => textMesh.color = defaultTextColor);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            var e = new PlayerCurrencyEventArgs
            {
                Currency = CurrencyType.Coin,
                Amount = 20,
                Animate = true,
                AnimationType = PlayerCurrencyParticleAnimator.ANIMATION_MODE_DECREASE_AMOUNT
            };

            AnimateDecrease(e);
        }
    }
}
