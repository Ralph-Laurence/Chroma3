using UnityEngine;
using UnityEngine.UI;

public partial class StageFabricator : MonoBehaviour
{
    [Space(10)] [Header("Block Eraser")]
    [SerializeField] private Texture2D eraseCursor;
    [SerializeField] private Vector2 eraseCursorHotspot = new Vector2(0.0F, 82.0F);
    [SerializeField] private Button eraseBlockButton;

    public bool _isMultiDeleteBegan;
    public bool IsMultiDelete => _isMultiDeleteBegan == true;
    
    /// <summary>
    /// The point where the cursor actually "clicks" is called the hotspot. 
    /// In our case, if the arrow image is pointing at the bottom left, we 
    /// would want the hotspot to be at that location. Our cursor image is 96x96 px.
    /// The hotspot is defined as a Vector2, where x is the horizontal offset and y 
    /// is the vertical offset. The origin (0, 0) is at the top-left corner of the image. 
    /// So, if the image is 96x96 pixels and the arrow points to the bottom left, 
    /// our hotspot would be at (0, 96).
    /// But in the actual image, the arrow image appears a little above the bottom edge,
    /// such as 82px. Thus our hotspot is 0,82
    /// </summary>
    public void HandleEraseBlockButtonClicked()
    {
        _isMultiDeleteBegan = !_isMultiDeleteBegan;

        if (_isMultiDeleteBegan)
        {
            eraseBlockButton.image.color = new Color(1.00F, 0.60F, 0.00F);
            Cursor.SetCursor(eraseCursor, eraseCursorHotspot, CursorMode.Auto);
        }
        else
        {
            eraseBlockButton.image.color = new Color(0.11F, 0.31F, 0.89F);
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}