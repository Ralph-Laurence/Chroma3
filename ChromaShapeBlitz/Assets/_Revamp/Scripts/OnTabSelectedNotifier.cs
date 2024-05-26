using UnityEngine.Events;

/// <summary>
/// This event happens when a tab has been selected
/// </summary>
public class TabItemSelectedEvent : UnityEvent<UXTabItem> { }

public class OnTabItemSelectedNotifier
{
    public static TabItemSelectedEvent Event = new TabItemSelectedEvent();

    public static void Publish(UXTabItem tabItem) => Event.Invoke(tabItem);
}