using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class ColorSelect : MonoBehaviour
{
    [SerializeField] private Sprite optionStyleBlue;
    [SerializeField] private Sprite optionStyleGreen;
    [SerializeField] private Sprite optionStyleMagenta;
    [SerializeField] private Sprite optionStyleOrange;
    [SerializeField] private Sprite optionStylePurple;
    [SerializeField] private Sprite optionStyleYellow;

    private Dictionary<int, ColorSwatches> colorSelectOptions;
    private Dictionary<ColorSwatches, Sprite> indicatorAppearances;
    private TMP_Dropdown dropdown;
    private bool _isInitialized;

    public Action<ColorSwatches> OnColorSelected {get; set;}

    void Awake() => Initialize();

    void OnEnable() => Initialize();

    private void Initialize()
    {
        if (_isInitialized)
            return;
        
        colorSelectOptions = new Dictionary<int, ColorSwatches>
        {
            { 0, ColorSwatches.Blue     },
            { 1, ColorSwatches.Green    },
            { 2, ColorSwatches.Magenta  },
            { 3, ColorSwatches.Orange   },
            { 4, ColorSwatches.Purple   },
            { 5, ColorSwatches.Yellow   },
        };

        indicatorAppearances = new Dictionary<ColorSwatches, Sprite>
        {
            { ColorSwatches.Blue   , optionStyleBlue    },
            { ColorSwatches.Green  , optionStyleGreen   },
            { ColorSwatches.Magenta, optionStyleMagenta },
            { ColorSwatches.Orange , optionStyleOrange  },
            { ColorSwatches.Purple , optionStylePurple  },
            { ColorSwatches.Yellow , optionStyleYellow  },
        };

        TryGetComponent(out dropdown);
        BuildDropdown();

        _isInitialized = true;
    }

    private void BuildDropdown()
    {
        dropdown.enabled = false;
        dropdown.onValueChanged.AddListener(HandleValueSelected);

        dropdown.options.Clear();

        foreach (var kvp in colorSelectOptions)
        {
            var data = new TMP_Dropdown.OptionData
            (
                kvp.Value.ToString(), 
                indicatorAppearances[kvp.Value], 
                Color.white
            );

            dropdown.options.Add(data);
        }

        dropdown.enabled = true;
    }

    private void HandleValueSelected(int index)
    {
        if (!colorSelectOptions.ContainsKey(index))
            return;

        var selectedColor = colorSelectOptions[index];
        OnColorSelected?.Invoke(selectedColor);
    }

    public ColorSwatches Value
    {
        get 
        { 
            var selectedColor = dropdown.value;

            if (colorSelectOptions.ContainsKey(selectedColor))
                return colorSelectOptions[selectedColor];

            return default;
        }
    }

    public void SetInteractable(bool interactable) => dropdown.interactable = interactable;
}