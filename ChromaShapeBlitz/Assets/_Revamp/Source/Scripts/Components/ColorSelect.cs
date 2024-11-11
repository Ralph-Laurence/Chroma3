using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public struct ColorSelectUIData
{
    public GameObject UIBlocker;
    public Dictionary<string, Toggle> ColorOptionToggles { get; set; }

    public void Initialize()
    {
        ColorOptionToggles = new();
    }
}

public enum ColorSelectBehaviour
{
    Normal,
    DisableOnValueSelected,
    DisableInteractionValueOnSelected,
    DestroyOnValueSelected
}

public struct ColorSelectValueCallback
{
    public int ColorIndex {get; set;}
    public ColorSwatches ColorValue {get; set;}
}

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

    public ColorSelectBehaviour Behaviour;

    // public Dictionary<string, Toggle> ColorOptionToggles { get; set; } = new();

    public const int COLOR_INDEX_BLUE      = 0;
    public const int COLOR_INDEX_GREEN     = 1;
    public const int COLOR_INDEX_MAGENTA   = 2;
    public const int COLOR_INDEX_ORANGE    = 3;
    public const int COLOR_INDEX_PURPLE    = 4;
    public const int COLOR_INDEX_YELLOW    = 5;

    void Awake() => Initialize();

    void OnEnable() => Initialize();

    void OnTransformChildrenChanged()
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(CheckChildAdded());
    }

    private void Initialize()
    {
        if (_isInitialized)
            return;
        
        colorSelectOptions = new Dictionary<int, ColorSwatches>
        {
            { COLOR_INDEX_BLUE   , ColorSwatches.Blue     },
            { COLOR_INDEX_GREEN  , ColorSwatches.Green    },
            { COLOR_INDEX_MAGENTA, ColorSwatches.Magenta  },
            { COLOR_INDEX_ORANGE , ColorSwatches.Orange   },
            { COLOR_INDEX_PURPLE , ColorSwatches.Purple   },
            { COLOR_INDEX_YELLOW , ColorSwatches.Yellow   },
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

        var outData = new ColorSelectValueCallback
        {
            ColorIndex = index,
            ColorValue = colorSelectOptions[index]
        };

        ColorSelectEventNotifier.NotifyObserver(ColorSelectEventNames.ColorSelected, outData);

        switch (Behaviour)
        {
            case ColorSelectBehaviour.DisableOnValueSelected:
                gameObject.SetActive(false);
                break;

            case ColorSelectBehaviour.DisableInteractionValueOnSelected:
                dropdown.interactable = false;
                break;

            case ColorSelectBehaviour.DestroyOnValueSelected:
                Destroy(gameObject);
                break;
        }
    }

    /// <summary>
    /// Update the dropdown's selected value without triggering its on value changed event
    /// </summary>
    public void SetValueSilent(int colorIndex)
    {
        dropdown.onValueChanged.RemoveListener(HandleValueSelected);
        dropdown.value = colorIndex;
        dropdown.onValueChanged.AddListener(HandleValueSelected);
        Debug.Log($"silent value set to {dropdown.value}");
    }

    /// <summary>
    /// Check if the dropdown is "Shown"
    /// </summary>
    private IEnumerator CheckChildAdded()
    {
        var data = new ColorSelectUIData();
        data.Initialize();

        GameObject dropdownList = null;

        // Loop to find the Dropdown List
        for (var i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);

            if (child.name.Equals("Dropdown List"))
            {
                dropdownList = child.gameObject;
                break;
            }
        }

        // If Dropdown List is not found, exit coroutine
        if (dropdownList == null)
            yield break;

        Transform items = dropdownList.transform.Find("Content");

        // If the items Transform is not found, exit coroutine
        if (items == null)
            yield break;

        // Wait until items are completely rendered, excluding the template item
        while (items.childCount < dropdown.options.Count + 1) // +1 for the template item
        {
            yield return null;
        }

        // Collect the options
        var itemIndex = 0;

        foreach (Transform child in items)
        {
            if (!child.gameObject.activeInHierarchy)
                continue;

            if (child.TryGetComponent(out Toggle toggle))
            {
                if (!data.ColorOptionToggles.ContainsKey(child.name) && itemIndex < dropdown.options.Count)
                {
                    data.ColorOptionToggles.Add(dropdown.options[itemIndex].text, toggle);
                }
            }
            itemIndex++;
            yield return null;
        }

        while (!dropdown.IsExpanded)
        {
            yield return null;
        }

        // Finally, Notify the observer to tell it that the options dropdown was fully shown
        var blocker = transform.parent.Find("Blocker");

        if (blocker != null)
            data.UIBlocker = blocker.gameObject;

        ColorSelectEventNotifier.NotifyObserver(ColorSelectEventNames.OptionsShown, data);
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

    public void AllowOnlyItemWithColor(ColorSwatches color)
    {
        var options = dropdown.options;
        var toggles = dropdown.GetComponentsInChildren<Toggle>();

        for (var i = 0; i < options.Count; i++)
        {
            var enable = options[i].text.Contains(color.ToString());
            toggles[i].interactable = enable;
        }
    }
}