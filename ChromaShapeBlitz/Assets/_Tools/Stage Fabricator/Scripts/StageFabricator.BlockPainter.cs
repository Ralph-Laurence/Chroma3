using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public partial class StageFabricator : MonoBehaviour
{
    [Space(10)]
    [SerializeField] private Material BlueMat;
    [SerializeField] private Material GreenMat;
    [SerializeField] private Material MagentaMat;
    [SerializeField] private Material YellowMat;
    [SerializeField] private Material OrangeMat;
    [SerializeField] private Material PurpleMat;

    [Space(10)]
    [SerializeField] private Material IncorrectBlockMat;

    [Space(10)]
    [SerializeField] private Button beginPaintButton;
    [SerializeField] private TMP_Dropdown paintColorSelect;

    private bool _isPaintBegan;

    public bool IsManualPainting => _isPaintBegan == true;

    private Dictionary<ColorSwatches, Material> _colorMap;

    public Dictionary<ColorSwatches, Material> GetColorMap()
    {
        if (_colorMap == null)
        {
            _colorMap = new Dictionary<ColorSwatches, Material>
            {
                { ColorSwatches.Blue    ,  BlueMat    },
                { ColorSwatches.Green   ,  GreenMat   },
                { ColorSwatches.Magenta ,  MagentaMat },
                { ColorSwatches.Orange  ,  OrangeMat  },
                { ColorSwatches.Purple  ,  PurpleMat  },
                { ColorSwatches.Yellow  ,  YellowMat  },
            };
        }

        return _colorMap;
    }

    private void HandleBeginBlockPaintButtonClicked()
    {
        _isPaintBegan = !_isPaintBegan;

        if (_isPaintBegan)
        {
            var paintData = GetPaintData();
            beginPaintButton.GetComponentInChildren<Text>().text = "Stop Paint";
            beginPaintButton.image.color = Color.white;
            paintColorSelect.image.color = paintData.SelectColor;
            paintColorSelect.interactable = false;
        }
        else
        {
            HandleEndBlockPaintButtonClicked();
        }
    }

    private void HandleEndBlockPaintButtonClicked()
    {
        _isPaintBegan = false;
        beginPaintButton.image.color = new Color(0.74F, 0.74F, 0.74F);
        paintColorSelect.image.color = new Color(0.5F, 0.5F, 0.5F);
        beginPaintButton.GetComponentInChildren<Text>().text = "Begin Paint";
        paintColorSelect.interactable = true;
    }
   
    public BlockPaintingData GetPaintData()
    {
        switch (paintColorSelect.value)
        {
            // Blue
            case 0:
                return new BlockPaintingData
                {
                    Material = BlueMat,
                    Color = ColorSwatches.Blue,
                    SelectColor = new Color(0.11F, 0.31F, 0.89F)
                };

            // Green
            case 1:
                return new BlockPaintingData
                {
                    Material = GreenMat,
                    Color = ColorSwatches.Green,
                    SelectColor = new Color(0.31F, 0.65F, 0.12F)
                };

            // Magenta
            case 2:
                return new BlockPaintingData
                {
                    Material = MagentaMat,
                    Color = ColorSwatches.Magenta,
                    SelectColor = new Color(0.76F, 0.00F, 0.44F)
                };

            // Orange
            case 3:
                return new BlockPaintingData
                {
                    Material = OrangeMat,
                    Color = ColorSwatches.Orange,
                    SelectColor = new Color(1.00F, 0.60F, 0.00F)
                };

            // Purple
            case 4:
                return new BlockPaintingData
                {
                    Material = PurpleMat,
                    Color = ColorSwatches.Purple,
                    SelectColor = new Color(0.42F, 0.15F, 0.73F)
                };

            // Yellow
            case 5:
                return new BlockPaintingData
                {
                    Material = YellowMat,
                    Color = ColorSwatches.Yellow,
                    SelectColor = new Color(1.00F, 0.84F, 0.00F)
                };
        }

        return default;
    }

    
}
public struct BlockPaintingData
{
    public Material Material;
    public ColorSwatches Color;
    public Color SelectColor;
}