using UnityEngine;
using UnityEngine.Events;

public class CommonEventObserver : MonoBehaviour
{
    [SerializeField] private UnityEvent actions;
    [SerializeField] private CommonEventTags notifierTag;

    void OnEnable()  => CommonEventNotifier.BindEvent(ObserveLongTask);
    void OnDisable() => CommonEventNotifier.UnbindEvent(ObserveLongTask);

    private void ObserveLongTask(CommonEventTags tag)
    {
        if (!tag.Equals(notifierTag))
            return;

        actions?.Invoke();
    }
}