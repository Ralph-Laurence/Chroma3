using UnityEngine.Events;

public class ColorFilterChangeEvent : UnityEvent<ColorSwatches> { }

public class OnColorFilterChangeNotifier
{
    public static ColorFilterChangeEvent Event = new ColorFilterChangeEvent();

    public static void Publish(ColorSwatches selectedColor) => Event.Invoke(selectedColor);
}
