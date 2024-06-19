using UnityEngine;
using UnityEngine.Events;

public class FabricatorTriggerClickedEvent : UnityEvent<BlockSequenceController> { }

public class FabricatorTriggerClickedNotifier : MonoBehaviour
{
    public static FabricatorTriggerClickedEvent Event = new FabricatorTriggerClickedEvent();

    public static void Publish(BlockSequenceController sender) => Event.Invoke(sender);
}