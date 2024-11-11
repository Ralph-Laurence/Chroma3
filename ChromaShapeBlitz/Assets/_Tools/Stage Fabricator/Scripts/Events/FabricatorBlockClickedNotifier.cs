using UnityEngine;
using UnityEngine.Events;

public class FabricatorBlockClickedEvent : UnityEvent<GameObject, int> { }
public class FabricatorBlockPaintEvent : UnityEvent<GameObject> { }
public class FabricatorBlockEraseEvent : UnityEvent<GameObject> { }

public class FabricatorBlockClickedNotifier
{
    public static FabricatorBlockClickedEvent Event = new FabricatorBlockClickedEvent();
    public static FabricatorBlockPaintEvent PaintEvent = new FabricatorBlockPaintEvent();
    public static FabricatorBlockEraseEvent EraseEvent = new FabricatorBlockEraseEvent();

    public static void Publish(GameObject selected, int menuMode) => Event.Invoke(selected, menuMode);
    public static void NotifyPaint(GameObject block) => PaintEvent.Invoke(block);
    public static void NotifyErase(GameObject block) => EraseEvent.Invoke(block);
}