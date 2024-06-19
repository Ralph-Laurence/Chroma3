using UnityEngine;
using UnityEngine.Events;

public struct PlayerBalance
{
    public int TotalCoins {get; set;}
    public int TotalGems {get; set;}
}

/// <summary>
/// This event happens when a skin is about to be purchased,
/// such as before showing the purchase dialog.
/// </summary>
public class UpdatePlayerBalanceEvent : UnityEvent<PlayerBalance> { }

public class OnUpdatePlayerBalanceUINotifier : MonoBehaviour
{
    public static UpdatePlayerBalanceEvent Event = new UpdatePlayerBalanceEvent();

    public static void Publish(PlayerBalance balance) => Event.Invoke(balance);
}
