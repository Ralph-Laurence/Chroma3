using UnityEngine.Events;

public struct PlayerBankAnimationParams
{
    /// <summary>
    /// The original value
    /// </summary>
    public int CurrentValue;

    /// <summary>
    /// The amount to increase or decrease
    /// </summary>
    public int Amount;

    public PlayerBankAnimationTypes AnimationType;

    public CurrencyType Currency;
}

public class PlayerBankAnimationNotifier
{
    private class PlayerBankAnimationEvent : UnityEvent<PlayerBankAnimationParams> { }
    private static readonly PlayerBankAnimationEvent _event = new();

    public static void BindObserver(UnityAction<PlayerBankAnimationParams> observer)
    {
        _event.AddListener(observer);
    }

    public static void UnbindObserver(UnityAction<PlayerBankAnimationParams> observer)
    {
        _event.RemoveListener(observer);
    }

    public static void NotifyObserver(PlayerBankAnimationParams data)
    {
        _event.Invoke(data);
    }
}