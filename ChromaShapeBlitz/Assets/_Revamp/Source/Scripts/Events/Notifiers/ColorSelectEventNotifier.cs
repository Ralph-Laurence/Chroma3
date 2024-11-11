using UnityEngine.Events;

public enum ColorSelectEventNames
{
    ColorSelected,
    OptionsShown
}

public class ColorSelectEventNotifier
{
    private class ColorSelectEvent : UnityEvent<ColorSelectEventNames, object> { }

    private static readonly ColorSelectEvent _event = new();

    public static void NotifyObserver(ColorSelectEventNames sender, object data)
    {
        _event.Invoke(sender, data);
    }

    public static void BindObserver(UnityAction<ColorSelectEventNames, object> observer)
    {
        _event.AddListener(observer);
    }

    public static void UnbindObserver(UnityAction<ColorSelectEventNames, object> observer)
    {
        _event.RemoveListener(observer);
    }
}
