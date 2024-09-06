using System.IO;
using UnityEngine;

public class Constants
{
    public struct FabricatorTags
    {
        public static string DestinationMarker => "DestinationMarker";
    }

    public readonly struct Scenes
    {
        public static readonly string MainMenu = "MainMenu";
    }

    public readonly struct Tags
    {
        //public static readonly string GameManager = "Game Manager";
        public static readonly string MainCamera  = "MainCamera";
    }

    public readonly struct CurrencySprites
    {
        public static readonly string GoldCoin = "<sprite=2>";
        public static readonly string GemCoin = "<sprite=3>";
    }

    public readonly struct ColorSwatches
    {
        public static readonly Color BLUE           = new(r: 0.0F, g: 0.3921566F, b: 1.0F, a: 1.0F);
        public static readonly Color GREEN          = new(r: 0.0F, g: 0.6792453F, b: 0.30837283F, a: 1.0F);
        public static readonly Color ORANGE         = new(r: 1.0F, g: 0.5254902F, b: 0.1254902F, a: 1.0F);
        public static readonly Color PURPLE         = new(r: 0.41568628F, g: 0.2F, b: 0.99607843F, a: 1.0F);
        public static readonly Color MAGENTA        = new(r: 1.0F, g: 0.0F, b: 0.43137255F, a: 1.0F);
        public static readonly Color YELLOW         = new(r: 1.0F, g: 0.6862745F, b: 0.1254902F, a: 1.0F);
        public static readonly Color RED            = new(r: 0.99607843F, g: 0.13725491F, b: 0.22745098F, a: 1.0F);
        public static readonly Color DARK_PURPLE    = new(r: 0.3254902F, g: 0.0F, b: 1.0F, a: 1.0F);
        public static readonly Color TEAL           = new(r: 0.0F, g: 0.7176471F, b: 0.6F, a: 1.0F);
        public static readonly Color DARK_TEAL      = new(r: 0.0F, g: 0.49411765F, b: 0.41960785F, a: 1.0F);
        public static readonly Color WHITE          = new(r: 1, g: 1, b: 1, a: 1);
    }

    public static readonly Color DefaultAmbientColor = new(0.212F, 0.227F, 0.259F, 1.0F);

    public readonly struct PackedAssetAddresses
    {
        public static readonly string PowerupsLUT = "PowerupsLookup";
        public static readonly string CountIndicatorSpriteSheets = "CountIndicatorSpriteSheets";
    }

    public readonly struct SubspriteIndeces
    {
        // public static readonly int ITEM_COUNT_0 = 0;
        // public static readonly int ITEM_COUNT_1 = 1;
        // public static readonly int ITEM_COUNT_2 = 2;
        // public static readonly int ITEM_COUNT_3 = 3;
        // public static readonly int ITEM_COUNT_4 = 4;
        // public static readonly int ITEM_COUNT_5 = 5;
        public static readonly int ITEM_COUNT_N = 6;    // Readonly Items
        public static readonly int ITEM_COUNT_X = 7;    // Special Warning
    }
}