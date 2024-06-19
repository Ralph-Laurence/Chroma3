using UnityEngine;

public class BackgroundSetup : MonoBehaviour
{
    [SerializeField] private Material skybox;
    [SerializeField] private Color ambientColor = Constants.DefaultAmbientColor;

    private Camera _mainCamera;
    private Camera MainCamera => _mainCamera;

    void Awake()
    {
        GameObject.FindWithTag(Constants.Tags.MainCamera).TryGetComponent(out _mainCamera);

        AmbientSetter.ChangeColor(ambientColor);
        AmbientSetter.ChangeSkybox(skybox);
    }
}