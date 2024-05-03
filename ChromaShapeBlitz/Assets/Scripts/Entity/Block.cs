using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("Appearance")]
    [SerializeField] private BlockColors requiredColor;
    [SerializeField] private bool darkenOnFill = false;
    [SerializeField] private float darkenAmount = 0.1F;

    private BlockColors currentColor = BlockColors.None;
    //private ThemeManager themer;
    private CustomizationsHelper themer;

    [Space(5)]
    [Header("Behaviour")]
    [SerializeField] private bool isDestinationBlock;

    // Start is called before the first frame update
    void Start()
    {
        // The theme manager is responsible for setting 
        // the block skins
        // if (ThemeManager.Instance != null)
        //    themer = ThemeManager.Instance;

        themer = CustomizationsHelper.Instance;
    }

    /// <summary>
    /// Set the current color from enumeration constant;
    /// This will be used for comparison which is faster
    /// than comparing the actual material instances
    /// </summary>
    /// 
    /// <param name="color">The color value</param>
    public void SetColor(BlockColors color) => currentColor = color;

    /// <summary>
    /// The color enumeration constant assigned by block controller.
    /// </summary>
    /// <returns>The current color value</returns>
    public BlockColors GetColor() => currentColor;

    /// <summary>
    /// Get the color this block should have.
    /// </summary>
    /// 
    /// <returns>The required color value</returns>
    public BlockColors GetRequiredColor() => requiredColor;

    /// <summary>
    /// Set the visual color by assigning a material. This is only for
    /// appearance purposes and is not updating the enumeration constant
    /// </summary>
    /// 
    /// <param name="material">The visual color to apply</param>
    public void ApplyMaterial(Material material, BlockColors colorGroup)
    {
        // Create a new material to prevent affecting the original,
        // then we pass the assigned material as a constructor parameter
        // because we want to copy existing properties of that material.
        var newMat  = new Material(material);
        var useSkin = false;

        // Should the material use a texture (skin)?
        // Setting a texture will force the material color to white.
        // if (themer != null)
        //     useSkin = themer.SetBlockSkin(newMat, currentColor);
        if (themer != null || colorGroup == BlockColors.None)
            useSkin = themer.FillBlockWithSkin(newMat, colorGroup);

        // useSkin = false means we wont use skins.
        // If this block was set to use dark color,
        // subtract the material's color by amount.
        if (!useSkin && darkenOnFill)
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
    /// Check if the block is a destination block. This will be used
    /// to trigger the sequence validation.
    /// </summary>
    public bool IsDestinationBlock() => isDestinationBlock;

    /// <summary>
    /// Check if the applied color is the same as the required color.
    /// </summary>
    public bool IsValidColor() => requiredColor == currentColor;
}
