using UnityEngine;
using UnityEngine.UI;

public class SlotMachineLightBulb : MonoBehaviour
{
    private Image image;

    void Awake()
    {
        TryGetComponent(out image);    
    }

    public void Toggle(Sprite light)
    {
        if (image == null)
            return;

        image.sprite = light;
    }
}
