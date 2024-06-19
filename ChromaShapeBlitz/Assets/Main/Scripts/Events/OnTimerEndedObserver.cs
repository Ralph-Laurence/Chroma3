using UnityEngine;
using UnityEngine.Events;

public class OnTimerEndedObserver : MonoBehaviour
{
    public UnityEvent OnNotified;
    
    private void OnEnable() => OnTimerEndedNotifier.Event.AddListener(Subscribe);
    private void OnDisable() => OnTimerEndedNotifier.Event.RemoveListener(Subscribe);

    public void Subscribe()
    {
        Debug.Log("Notified");

        if (OnNotified != null)
           OnNotified.Invoke();
    }

    // Utility function
    public void DisableSelf() => gameObject.SetActive(false);
}