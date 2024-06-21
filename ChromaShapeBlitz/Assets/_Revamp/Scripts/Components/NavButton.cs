using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

public class NavButton : MonoBehaviour
{
    [SerializeField] private Color NormalColor = Color.white;
    [SerializeField] private Color SelectedColor = new Color(1.0F, 0.78F, 0.15F, 1.0F);
    [SerializeField] private float SelectedTextSize = 26.0F;
    [SerializeField] private float NormalTextSize = 22.0F;
    public bool Selected;

    private TextMeshProUGUI tmp;
    
    private bool isInitialized;

    private void Initialize()
    {
        if (isInitialized)
            return;

        TryGetComponent(out tmp);

        tmp.fontSize = NormalTextSize;
        tmp.color = NormalColor;

        isInitialized = true;
    }

    void OnEnable() => Initialize();
    void Awake() => Initialize();

    public void SetSelected()
    {
        Selected = true;
        tmp.fontSize = SelectedTextSize;
        tmp.color = SelectedColor;
    }
}
