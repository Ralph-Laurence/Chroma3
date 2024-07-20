using UnityEngine;

public class StageBackground : MonoBehaviour
{
    [Header("Common Properties")]
    public Material Skybox;
    public float YPositionOffset;
    
    [Header("Post Process Effects")]
    public bool UsePostProcess = true;
    [Space(10)]

    public bool EnableBloom = true;
    public Color BloomColor = Color.white;
    public float BloomIntensity = 0.5F;
    public float BloomDiffuse = 1.0F;
    public float BloomThreshold = 0.0F;
    public float BloomSoftness = 0.5F;
    
    [Space(10)]
    public bool EnableLut = true;
    public Texture2D LutSource;
    public float LutIntensity = 1.0F;

    [Space(10)]
    public bool EnableVignette = true;
    public Color VignetteColor = Color.black;
    public float VignetteIntensity = 0.038F;
    public float VignetteSoftness = 0.422F;
}
