using UnityEngine.Events;

namespace Revamp
{
    public class StageCompletedEvent : UnityEvent<StageCompletionType> {}
    public class StageCreatedEvent : UnityEvent<StageCreatedEventArgs> {}
    public class OnStageCompleted
    {
        public static StageCompletedEvent Event = new StageCompletedEvent();
        public static void NotifyObserver(StageCompletionType stageCompletionType) => Event.Invoke(stageCompletionType);

        // Add an event listener
        public static void BindEvent(UnityAction<StageCompletionType> eventAction)
        {
            Event.AddListener(eventAction);
        }

        // Remove an event listener
        public static void UnbindEvent(UnityAction<StageCompletionType> eventAction)
        {
            Event.RemoveListener(eventAction);
        }
    }

    public class OnStageCreated
    {
        private static readonly StageCreatedEvent _event = new StageCreatedEvent();
        public static void NotifyObserver(StageCreatedEventArgs eventArgs) => _event.Invoke(eventArgs);
        public static void BindEvent(UnityAction<StageCreatedEventArgs> eventAction)
        {
            _event.AddListener(eventAction);
        }
        public static void UnbindEvent(UnityAction<StageCreatedEventArgs> eventAction)
        {
            _event.RemoveListener(eventAction);
        }
    }
}