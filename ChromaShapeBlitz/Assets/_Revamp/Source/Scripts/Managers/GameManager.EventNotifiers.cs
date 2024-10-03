using UnityEngine.Events;

namespace Revamp
{
    public class StageCompletedEvent : UnityEvent<StageCompletionType> {}
    public class StageCreatedEvent : UnityEvent<StageCreatedEventArgs> {}
    public class GameOverScreenEvent : UnityEvent<GameOverEventArgs> {}
    public class GameManagerStateEvent : UnityEvent<GameManagerStates> {}
    public class GameManagerActionEvent : UnityEvent<GameManagerActionEvents> {}

    public class GameManagerStateNotifier
    {
        private static readonly GameManagerStateEvent _event = new GameManagerStateEvent();
        public static void NotifyObserver(GameManagerStates state) => _event.Invoke(state);
        public static void BindEvent(UnityAction<GameManagerStates> eventAction)
        {
            _event.AddListener(eventAction);
        }
        public static void UnbindEvent(UnityAction<GameManagerStates> eventAction)
        {
            _event.RemoveListener(eventAction);
        }
    }

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
        public static void BindObserver(UnityAction<StageCreatedEventArgs> eventAction)
        {
            _event.AddListener(eventAction);
        }
        public static void UnbindObserver(UnityAction<StageCreatedEventArgs> eventAction)
        {
            _event.RemoveListener(eventAction);
        }
    }

    public class GameOverScreenNotifier
    {
        private static readonly GameOverScreenEvent _event = new GameOverScreenEvent();
        public static void NotifyObserver(GameOverEventArgs e) => _event.Invoke(e);
        public static void BindEvent(UnityAction<GameOverEventArgs> eventAction)
        {
            _event.AddListener(eventAction);
        }
        public static void UnbindEvent(UnityAction<GameOverEventArgs> eventAction)
        {
            _event.RemoveListener(eventAction);
        }
    }

    public class GameManagerEventNotifier
    {
        public static GameManagerActionEvent _event = new GameManagerActionEvent();
        public static void Notify(GameManagerActionEvents e) => _event.Invoke(e);
        public static void BindObserver(UnityAction<GameManagerActionEvents> eventAction)
        {
            _event.AddListener(eventAction);
        }
        public static void UnbindObserver(UnityAction<GameManagerActionEvents> eventAction)
        {
            _event.RemoveListener(eventAction);
        }
    }
}