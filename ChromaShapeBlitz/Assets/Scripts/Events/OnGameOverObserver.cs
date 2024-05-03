using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This will subscribe to the event notifier to listen for when the game
/// is over (either passed or failed), which is triggered by GameManager
/// 
/// This script must be attached into a Stage UI Controller gameobject
/// </summary>
public class OnGameOverObserver : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;
    private GameOverAnimation gameOverAnimation;

    private void OnEnable() => OnGameOverNotifier.Event.AddListener(Subscribe);

    private void OnDisable() => OnGameOverNotifier.Event.RemoveListener(Subscribe);

    private void Start()
    {
        gameOverScreen.TryGetComponent(out gameOverAnimation);
    }

    public void Subscribe(GameOverAnimatableValues animatableValues)
    {
        gameOverScreen.SetActive(true);

        gameOverAnimation.SetProperties(animatableValues);
        gameOverAnimation.BeginAnimation();
    }
}
