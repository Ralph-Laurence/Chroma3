using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class Extensions
{
    public static GameObject ChangeMaterial(this GameObject gameObject, Material material)
    {
        if (gameObject.TryGetComponent(out Renderer renderer))
            renderer.material = material;

        return gameObject;
    }

    /// <summary>
    /// Converts the angle into Inspector angles
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static float WrapAngles(this float angle)
    {
        if (angle > 180)
            angle -= 360;

        return angle;
    }

    /// <summary>
    /// Convert the enum color to its string equivalent
    /// </summary>
    /// <param name="blockColors">The source color</param>
    /// <returns>Descriptive name</returns>
    // public static string ToColorName(this ColorSwatches blockColors)
    // {
    //     switch (blockColors)
    //     {
    //         case ColorSwatches.Blue:      return "Blue";
    //         case ColorSwatches.Green:     return "Green";
    //         case ColorSwatches.Magenta:   return "Magenta";
    //         case ColorSwatches.Yellow:    return "Yellow";
    //         case ColorSwatches.Orange:    return "Orange";
    //         case ColorSwatches.Purple:    return "Purple";
    //         default: return string.Empty;
    //     }
    // }

    public static string ToColorName(this ColorSwatches blockColors)
    {
        switch (blockColors)
        {
            case ColorSwatches.Blue:      return "Blue";
            case ColorSwatches.Green:     return "Green";
            case ColorSwatches.Magenta:   return "Magenta";
            case ColorSwatches.Yellow:    return "Yellow";
            case ColorSwatches.Orange:    return "Orange";
            case ColorSwatches.Purple:    return "Purple";
            default: return string.Empty;
        }
    }

    /// <summary>
    /// Convert the BlockColor enumeration into visual colors
    /// </summary>
    /// <returns>Unity Color</returns>
    // public static Color ToUnityColor(this ColorSwatches blockColors)
    // {
    //     switch(blockColors)
    //     {
    //         case ColorSwatches.Blue:      return Constants.ColorSwatches.BLUE;
    //         case ColorSwatches.Green:     return Constants.ColorSwatches.GREEN;
    //         case ColorSwatches.Magenta:   return Constants.ColorSwatches.MAGENTA;
    //         case ColorSwatches.Yellow:    return Constants.ColorSwatches.YELLOW;
    //         case ColorSwatches.Orange:    return Constants.ColorSwatches.ORANGE;
    //         case ColorSwatches.Purple:    return Constants.ColorSwatches.PURPLE;
    //         default: return Color.white;
    //     }
    // }

    public static Color ToUnityColor(this ColorSwatches blockColors)
    {
        switch(blockColors)
        {
            case ColorSwatches.Blue:      return Constants.ColorSwatches.BLUE;
            case ColorSwatches.Green:     return Constants.ColorSwatches.GREEN;
            case ColorSwatches.Magenta:   return Constants.ColorSwatches.MAGENTA;
            case ColorSwatches.Yellow:    return Constants.ColorSwatches.YELLOW;
            case ColorSwatches.Orange:    return Constants.ColorSwatches.ORANGE;
            case ColorSwatches.Purple:    return Constants.ColorSwatches.PURPLE;
            default: return Color.white;
        }
    }

    /// <summary>
    /// Add a coin or gem icon to a price text.
    /// This requires that the TextMesh component has an attached
    /// sprite glyps and styles.
    /// </summary>
    /// <param name="currencyType"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public static string PrefixWithCurrencyIcon(this CurrencyType currencyType, int amount)
    {
        switch  (currencyType)
        {
            case CurrencyType.Coin: return $"<sprite=5> {amount}";
            case CurrencyType.Gem : return $"<sprite=1> {amount}";
            default: return string.Empty;
        }
    }

    public static string ToCurrencyName(this CurrencyType currencyType, bool plural = true)
    {
        switch (currencyType)
        {
            case CurrencyType.Coin:
                return plural ? "Coins" : "Coin";

            case CurrencyType.Gem:
                return plural ? "Gems" : "Gem";

            default:
                return string.Empty;
        }
    }

    public static float ReadFloat(this InputField input) 
    {
        if (string.IsNullOrEmpty(input.text))
            return 0.0F;

        return Convert.ToSingle(input.text);
    }

    public static int ReadInt(this InputField input) 
    {
        if (string.IsNullOrEmpty(input.text))
            return 0;

        return Convert.ToInt32(input.text);
    }
}
