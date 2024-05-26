using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Listen for color filter select
/// 
/// This script must be attached into the Customize Menu Base Controller gameobject
/// </summary>
public class OnColorFilterChangeObserver : MonoBehaviour
{
    public UnityEvent OnFilterValueChanged;

    private void OnEnable() => OnColorFilterChangeNotifier.Event.AddListener(Subscribe);

    private void OnDisable() => OnColorFilterChangeNotifier.Event.RemoveListener(Subscribe);

    public void Subscribe(ColorSwatches selectedColor) => OnFilterValueChanged?.Invoke();
}