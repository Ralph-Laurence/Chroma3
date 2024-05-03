using UnityEngine;

public class SkinVariationFilter : MonoBehaviour
{
    [SerializeField] private BlockColors colorValue = BlockColors.None;

    public BlockColors ColorValue => colorValue;
}