using UnityEngine;

public class SelectedTriggerMarkerObserver : MonoBehaviour
{

    void OnEnable() => SelectedTriggerMarkerNotifier.Event.AddListener(SubscribeReset);
    void OnDisable() => SelectedTriggerMarkerNotifier.Event.RemoveListener(SubscribeReset);

    public void SubscribeReset()
    {
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
    }
}