using UnityEngine;
using UnityEngine.Rendering;

public class AmbientSetter
{
    /// <summary>
    /// Change the ambient lighting to custom color
    /// </summary>
    public static void ChangeColor(Color ambientColor)
    {
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = ambientColor;
    }

    public static void ChangeSkybox(Material skybox) => RenderSettings.skybox = skybox;

    /// <summary>
    /// Reverts the ambient lighting colors into skybox
    /// </summary>
    public static void Reset()
    {
        RenderSettings.ambientLight = Constants.DefaultAmbientColor;
        RenderSettings.ambientMode = AmbientMode.Skybox;
        RenderSettings.skybox = Resources.Load("Materials/Procedural-Skybox") as Material;
    }
}