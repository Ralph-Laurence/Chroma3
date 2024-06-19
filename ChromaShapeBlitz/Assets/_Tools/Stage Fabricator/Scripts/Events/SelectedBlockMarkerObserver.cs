using UnityEngine;

public class SelectedBlockMarkerObserver : MonoBehaviour
{

    void OnEnable() => SelectedBlockMarkerNotifier.Event.AddListener(SubscribeReset);
    void OnDisable() => SelectedBlockMarkerNotifier.Event.RemoveListener(SubscribeReset);

    public void SubscribeReset()
    {
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
    }
}