using System;
using System.Collections.Generic;
using TMPro;
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
    /// Apply the same text on multiple text meshes
    /// </summary>
    public static TextMeshProUGUI[] SetTextMultiple(this TextMeshProUGUI[] textMeshes, string text)
    {
        foreach (var tmp in textMeshes)
            tmp.text = text;

        return textMeshes;
    }

    // public static string ToColorName(this ColorSwatches blockColors)
    // {
    //     return blockColors switch
    //     {
    //         ColorSwatches.Blue      => "Blue",
    //         ColorSwatches.Green     => "Green",
    //         ColorSwatches.Magenta   => "Magenta",
    //         ColorSwatches.Yellow    => "Yellow",
    //         ColorSwatches.Orange    => "Orange",
    //         ColorSwatches.Purple    => "Purple",
    //         _ => string.Empty,
    //     };
    // }

    public static Color ToUnityColor(this ColorSwatches blockColors)
    {
        return blockColors switch
        {
            ColorSwatches.Blue      => Constants.ColorSwatches.BLUE,
            ColorSwatches.Green     => Constants.ColorSwatches.GREEN,
            ColorSwatches.Magenta   => Constants.ColorSwatches.MAGENTA,
            ColorSwatches.Yellow    => Constants.ColorSwatches.YELLOW,
            ColorSwatches.Orange    => Constants.ColorSwatches.ORANGE,
            ColorSwatches.Purple    => Constants.ColorSwatches.PURPLE,
            _ => Color.white,
        };
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

    public static List<int> ToList(this ActiveBlockSkinIDs activeBlockSkinIDs)
    {
        return new List<int>
        {
            activeBlockSkinIDs.Blue,
            activeBlockSkinIDs.Green,
            activeBlockSkinIDs.Magenta,
            activeBlockSkinIDs.Orange,
            activeBlockSkinIDs.Purple,
            activeBlockSkinIDs.Yellow
        };
    }

    public static string ToRewardText(this int amount, RewardTypes currencyType)
    {
        var rewardStyle = currencyType.Equals(RewardTypes.Gems) ? "Gem" : "Coin";
        return $"<style=\"{rewardStyle}\">\u00d7{amount}";
    }
}
