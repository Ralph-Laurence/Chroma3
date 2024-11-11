using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SequenceTriggerEditorContextMenu : MonoBehaviour
{
    private Dictionary<string, ColorSwatches> colorMap;
    private bool isDataInitialized;

    public BlockContextMenuData ContextMenuData { get; set; }

    [Space(5)]
    [SerializeField] private StageFabricator stageFabricator;
    [SerializeField] private GameObject parentContainer;
    [SerializeField] private TMP_Dropdown colorSelect;
    [SerializeField] private TMP_Dropdown triggerPlacementSelect;

    void OnEnable()
    {
        if (!isDataInitialized)
        {
            InitializeData();
            isDataInitialized = true;
            return;
        }

        BindData();
    }

    private void InitializeData()
    {
        colorMap = new Dictionary<string, ColorSwatches>
        {
            { "Blue",    ColorSwatches.Blue    },
            { "Green",   ColorSwatches.Green   },
            { "Magenta", ColorSwatches.Magenta },
            { "Orange",  ColorSwatches.Orange  },
            { "Purple",  ColorSwatches.Purple  },
            { "Yellow",  ColorSwatches.Yellow  },
        };

        colorSelect.options.Clear();

        foreach (var color in colorMap.Keys)
        {
            colorSelect.options.Add(new TMP_Dropdown.OptionData(color));
        }

        colorSelect.value = 0;
        colorSelect.RefreshShownValue();

        triggerPlacementSelect.options.Clear();

        triggerPlacementSelect.options.Add(new TMP_Dropdown.OptionData(TriggerPlacements.NORTH.ToString()));
        triggerPlacementSelect.options.Add(new TMP_Dropdown.OptionData(TriggerPlacements.SOUTH.ToString()));
        triggerPlacementSelect.options.Add(new TMP_Dropdown.OptionData(TriggerPlacements.EAST.ToString()));
        triggerPlacementSelect.options.Add(new TMP_Dropdown.OptionData(TriggerPlacements.WEST.ToString()));

        triggerPlacementSelect.value = 0;
        triggerPlacementSelect.RefreshShownValue();
    }

    private void BindData()
    {
        colorSelect.value = 0;
        colorSelect.RefreshShownValue();

        triggerPlacementSelect.value = 0;
        triggerPlacementSelect.RefreshShownValue();
    }

    public void Show()
    {
        if (parentContainer != null)
            parentContainer.SetActive(true);

        gameObject.SetActive(true);
    }

    public void Ev_Add()
    {
        var fillColor        = GetSelectedColor();
        var triggerPlacement = (TriggerPlacements) triggerPlacementSelect.value;

        stageFabricator.AddFillTrigger(triggerPlacement, ContextMenuData.BlockComponent, fillColor);
    }

    private ColorSwatches GetSelectedColor()
    {
        var selectedColor = colorSelect.options[colorSelect.value].text;
        var fillColor     = colorMap[selectedColor];

        return fillColor;
    }
}
