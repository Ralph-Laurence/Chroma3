using UnityEngine;
using UnityEngine.Events;

public class GameOverEvent : UnityEvent<GameOverAnimatableValues> { }

public class OnGameOverNotifier : MonoBehaviour
{
    public static GameOverEvent Event = new GameOverEvent();

    public static void Publish(GameOverAnimatableValues animatableValues) => Event.Invoke(animatableValues);
}
