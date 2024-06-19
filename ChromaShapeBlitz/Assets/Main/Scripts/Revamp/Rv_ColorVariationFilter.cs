using UnityEngine;
using UnityEngine.UI;

public class Rv_ColorVariationFilter : MonoBehaviour
{
    [SerializeField] private Toggle[] filters;
    [SerializeField] private ColorFilterIndicator colorFilterIndicator;

    public Toggle SelectedFilter { private set; get; }
    public ColorSwatches SelectedColor { private set; get; }

    private bool isInitialized;

    void Awake() => Initialize();

    void OnEnable() => Initialize();

    private void Initialize()
    {
        if (isInitialized)
            return;

        foreach (var toggle in filters)
        {
            toggle.onValueChanged.AddListener(delegate
            {
                ToggleValueChanged(toggle);
            });
        }

        // The very first toggle item must be always toggled
        if (!filters[0].isOn)
            filters[0].isOn = true;
        else
            ApplySelectedColorFilter(filters[0]);

        isInitialized = true;
    }

    public Toggle Selected => SelectedFilter;

    private void ToggleValueChanged(Toggle targetToggle)
    {
        if (targetToggle.isOn)
            ApplySelectedColorFilter(targetToggle);
    }

    private void ApplySelectedColorFilter(Toggle targetToggle)
    {
        targetToggle.TryGetComponent(out ColorVariationFilter variationFilter);
        SelectedFilter = targetToggle;
        SelectedColor = variationFilter.ColorValue;

        colorFilterIndicator.DrawIndicator(SelectedColor);

        OnColorFilterChangeNotifier.Publish(SelectedColor);
    }
}