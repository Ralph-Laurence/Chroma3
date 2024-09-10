using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("Appearance")]
    [SerializeField] private ColorSwatches requiredColor;
    public bool DarkerFill = false;
    [SerializeField] private float darkenAmount = 0.1F;

    private ColorSwatches currentColor = ColorSwatches.None;
    
    [Space(5)]
    [Header("Behaviour")]
    public bool IsDestinationBlock;

    /// <summary>
    /// Set the current color from enumeration constant;
    /// This will be used for comparison which is faster
    /// than comparing the actual material instances
    /// </summary>
    /// 
    /// <param name="color">The color value</param>
    public void SetColor(ColorSwatches color) => currentColor = color;
    
    /// <summary>
    /// Get the current color value applied to this block.
    /// This is not the visible color, but just the value.
    /// </summary>
    /// <returns>Current applied color value</returns>
    public ColorSwatches GetColor() => currentColor;
    /// <summary>
    /// Set the visual color by assigning a material. This is only for
    /// appearance purposes and is not updating the enumeration constant
    /// </summary>
    /// 
    /// <param name="material">The visual color to apply</param>
    public void ApplyMaterial(Material material, ColorSwatches colorGroup)
    {
        // Create a new material to prevent affecting the original. Passing the "material" 
        // as a parameter copies its existing properties.
        var newMat  = new Material(material);

        // If this block was set to use dark color, darken the material's color.
        if (DarkerFill)
        {
            var originalColor = material.color;

            newMat.color = new Color
            (
                originalColor.r - darkenAmount,
                originalColor.g - darkenAmount,
                originalColor.b - darkenAmount,
                originalColor.a
            );
        }

        // Apply the new material
        gameObject.ChangeMaterial(newMat);
    }

    /// <summary>
    /// Check if the applied color is the same as the required color.
    /// </summary>
    public bool IsValidColor() => requiredColor == currentColor;

    #region FOR_POWERUP_EFFECTS
    /// <summary>
    /// This must be only used by powerups. This should reveal the
    /// initial (ie. guide) color before it was made invisible.
    /// This wont change the internal color state other than visuals.
    /// </summary>
    public void RevealGuideColor(Material light, Material dark)
    {
        // If this block has already been given a different color, skip.
        if (currentColor != ColorSwatches.None)
            return;

        var newMat = DarkerFill ? new Material(dark) : new Material(light);
        gameObject.ChangeMaterial(newMat);
    }
    #endregion FOR_POWERUP_EFFECTS

    #region STAGE_FABRICATOR_PROPERTIES
    public int RowIndex;
    public int ColumnIndex;
    public ColorSwatches RequiredColor => requiredColor;
    public void SetRequiredColor(ColorSwatches color) => requiredColor = color; 
    #endregion STAGE_FABRICATOR_PROPERTIES
}