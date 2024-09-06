using System;
using System.Collections.Generic;
using TMPro;
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
    /// Change the renderer mode into transparent
    /// </summary>
    /// <returns>Transparent material</returns>
    public static Material RenderToTransparent(this Material material)
    {
        material.SetFloat("_Mode", 3); // Set to Transparent
        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        return material;
    }

    /// <summary>
    /// Change the renderer mode into opaque
    /// </summary>
    /// <returns>Opaque material</returns>
    public static Material RenderToOpaque(this Material material)
    {
        material.SetFloat("_Mode", 0); // Set to Opaque
        material.SetOverrideTag("RenderType", "Opaque");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

        return material;
    }

    /// <summary>
    /// Change the renderer mode into transparent cutout
    /// </summary>
    /// <returns>Transparent cutout material</returns>
    public static Material RenderToCutout(this Material material)
    {
        material.SetFloat("_Mode", 1); // Set to Cutout
        material.SetOverrideTag("RenderType", "TransparentCutout");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;

        return material;
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

    /// <summary>
    /// Translate Powerup type into its equivalent inventory item type
    /// </summary>
    public static InventoryItemType ToInventoryItemType(this PowerupType powerupType) => powerupType switch
    {
        PowerupType.Permanent           => InventoryItemType.ReadOnly,
        PowerupType.ConsumableSingle    => InventoryItemType.Consumable,
        PowerupType.ConsumableStackable => InventoryItemType.Consumable,
        _ => InventoryItemType.Unset
    };

    /// <summary>
    /// Translate the numeric amount into its sprite-based amount indicator equivalent
    /// </summary>
    public static Sprite MapAmountToSprite
    (
        this int amount,
        Sprite[] map,
        InventoryItemType itemType = InventoryItemType.Consumable
    )
    {
        if (itemType == InventoryItemType.ReadOnly)
            return map[Constants.SubspriteIndeces.ITEM_COUNT_N];

        if (itemType == InventoryItemType.Unset || amount > map.Length)
            return map[Constants.SubspriteIndeces.ITEM_COUNT_X];

        return map[amount];
    }
}
