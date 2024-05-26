using UnityEngine;

public class ColorVariationFilter : MonoBehaviour
{
    [SerializeField] private ColorSwatches colorValue = ColorSwatches.None;

    public ColorSwatches ColorValue => colorValue;

    //public void HandleClicked() => OnColorFilterClickedNotifier.Publish(colorValue);
}