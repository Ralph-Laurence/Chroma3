using UnityEngine;

public class InteractionBlocker : MonoBehaviour
{
    [SerializeField] private GameObject blockingElement;

    void OnEnable() => InteractionBlockerNotifier.BindObserver(ObserverInteractionBlocker);

    void OnDisable() => InteractionBlockerNotifier.UnbindObserver(ObserverInteractionBlocker);

    private void ObserverInteractionBlocker(bool show)
    {
        if (!show)
        {
            blockingElement.SetActive(false);
            return;
        }

        blockingElement.SetActive(true);
    }
}
