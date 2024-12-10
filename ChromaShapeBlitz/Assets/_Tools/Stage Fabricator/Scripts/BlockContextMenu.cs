using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct BlockContextMenuData
{
    public Block BlockComponent;
    public string BlockName;
}

public struct BlockColorVariant
{
    public string ColorName;
    public Material Light;
    public Material Dark;
}

public class BlockContextMenu : MonoBehaviour
{
    private readonly string OPTION_BLOCK_TYPE_NORMAL = "Normal";
    private readonly string OPTION_BLOCK_TYPE_DESTINATION = "Destination";

    private Dictionary<string, ColorSwatches> blockColors;
    private Dictionary<string, int> blockTypes;
    private Dictionary<ColorSwatches, Material> materialColorMap;
    // private Dictionary<string, GameObject> destinationMarkers;

    [SerializeField] private StageFabricator fabricator;

    [Space(10)]
    [SerializeField] private GameObject parentContainer;
    [SerializeField] private Text blockNameText;
    [SerializeField] private TMP_Dropdown blockColorSelect;
    [SerializeField] private TMP_Dropdown blockTypeSelect;

    // [Space(10)] 
    // [SerializeField] private GameObject destinationMarkerPrefab;

    [Space(10)]
    [SerializeField] private Material BlueMat;
    [SerializeField] private Material GreenMat;
    [SerializeField] private Material MagentaMat;
    [SerializeField] private Material YellowMat;
    [SerializeField] private Material OrangeMat;
    [SerializeField] private Material PurpleMat;

    [Space(5)]
    [SerializeField] private Material LightMat;
    [SerializeField] private Material DarkMat;

    public BlockContextMenuData ContextMenuData { get; set; }

    void OnEnable()
    {
        if (blockColors != null)
        {
            BindData();
            return;
        }

        blockTypes = new Dictionary<string, int>
        {
            { OPTION_BLOCK_TYPE_NORMAL,      0 },
            { OPTION_BLOCK_TYPE_DESTINATION, 1 }
        };

        blockTypeSelect.options.Clear();
        foreach (var type in blockTypes.Keys)
        {
            blockTypeSelect.options.Add(new TMP_Dropdown.OptionData(type));
        }
        blockTypeSelect.value = 0;
        blockTypeSelect.RefreshShownValue();

        //destinationMarkers = new Dictionary<string, GameObject>();

        blockColors = new Dictionary<string, ColorSwatches>
            {
                { "None",                               ColorSwatches.None      },
                { ColorSwatches.Blue.ToString(),     ColorSwatches.Blue      },
                { ColorSwatches.Green.ToString(),    ColorSwatches.Green     },
                { ColorSwatches.Magenta.ToString(),  ColorSwatches.Magenta   },
                { ColorSwatches.Yellow.ToString(),   ColorSwatches.Yellow    },
                { ColorSwatches.Orange.ToString(),   ColorSwatches.Orange    },
                { ColorSwatches.Purple.ToString(),   ColorSwatches.Purple    },
            };

        materialColorMap = new Dictionary<ColorSwatches, Material>
            {
                { ColorSwatches.Blue   ,  BlueMat    },
                { ColorSwatches.Green  ,  GreenMat   },
                { ColorSwatches.Magenta,  MagentaMat },
                { ColorSwatches.Yellow ,  YellowMat  },
                { ColorSwatches.Orange ,  OrangeMat  },
                { ColorSwatches.Purple ,  PurpleMat  },
            };

        blockColorSelect.options.Clear();

        foreach (var color in blockColors.Keys)
        {
            blockColorSelect.options.Add(new TMP_Dropdown.OptionData(color));
        }

        blockColorSelect.value = 0;
        blockColorSelect.RefreshShownValue();

        // If the selected block's required color is not "NONE", it has been
        // painted manually. Thus, we bind its existing data into the UI
        if (ContextMenuData.BlockComponent.RequiredColor != ColorSwatches.None)
            BindData();
    }

    public void Show()
    {
        if (parentContainer != null)
            parentContainer.SetActive(true);

        gameObject.SetActive(true);
    }

    private void BindData()
    {
        blockNameText.text = ContextMenuData.BlockName;
        
        var colorName  = ContextMenuData.BlockComponent.RequiredColor.ToString();
        var colorIndex = blockColors.Keys.ToList().IndexOf(colorName);

        blockColorSelect.value = colorIndex;
        blockColorSelect.RefreshShownValue();

        var blockType = ContextMenuData.BlockComponent.IsDestinationBlock 
                      ? OPTION_BLOCK_TYPE_DESTINATION 
                      : OPTION_BLOCK_TYPE_NORMAL;

        var typeIndex = blockTypes.Keys.ToList().IndexOf(blockType);
        blockTypeSelect.value = typeIndex;
        blockTypeSelect.RefreshShownValue();
    }

    public void Ev_DeleteSelectedBlock()
    {
        var block = ContextMenuData.BlockComponent;

        if (block != null)
        {
            Destroy(block.gameObject);
            SelectedBlockMarkerNotifier.NotifyReset();
        }

        //RemoveDestinationMarker(block.gameObject);
        fabricator.RemoveDestinationMarker(block.gameObject);
    }

    public void Ev_ApplyContextMenuData()
    {
        var block = ContextMenuData.BlockComponent;

        if (block == null)
            return;
        
        var color = GetSelectedColor();

        if (materialColorMap.ContainsKey(color) && color != ColorSwatches.None)
        {
            block.SetRequiredColor( color );
            block.ApplyMaterial( materialColorMap[color], color );
        }
        else
        {
            ResetMaterial(block);
        }

        if (blockTypeSelect.options[blockTypeSelect.value].text.Equals("Destination"))
        {
            // if (fabricator.DestinationMarkerExists(block.name))
            //     return;

            fabricator.AddDestinationMarker(block);
            block.IsDestinationBlock = true;
            return;
        }

        block.IsDestinationBlock = false;
        fabricator.RemoveDestinationMarker(block.gameObject);
    }

    public void Ev_ResetSelectedBlock()
    {
        var selectedBlock = ContextMenuData.BlockComponent.gameObject;

        if (selectedBlock == null)
            return;

        fabricator.RemoveDestinationMarker(selectedBlock);

        selectedBlock.TryGetComponent(out Block blockComponent);

        var isDarkFill = blockComponent.DarkerFill;

        if (blockComponent != null)
            Destroy(blockComponent);

        var newBlockComponent = selectedBlock.AddComponent(typeof(Block)) as Block;
        newBlockComponent.DarkerFill = isDarkFill;

        ResetMaterial(newBlockComponent);

        ContextMenuData = new BlockContextMenuData();
    }

    private void ResetMaterial(Block block)
    {
        block.SetRequiredColor(ColorSwatches.None);
        block.SetColor(ColorSwatches.None);
        block.ApplyMaterial
        (
            block.DarkerFill ? DarkMat : LightMat,
            ColorSwatches.None
        );
    }

    private ColorSwatches GetSelectedColor()
    {
        var color = blockColorSelect.options[ blockColorSelect.value ].text;
        return blockColors[color];
    }

    public void Ev_HardReset()
    {
        ContextMenuData = new BlockContextMenuData();
        // destinationMarkers?.Clear();
        fabricator.ResetDestinationMarkers();
        GameObject.FindGameObjectsWithTag
        (
            //Constants.FabricatorTags.DestinationMarker
            Constants.Tags.DestinationMarker
        )
        .ToList()
        .ForEach(g => Destroy(g));
    }
}
