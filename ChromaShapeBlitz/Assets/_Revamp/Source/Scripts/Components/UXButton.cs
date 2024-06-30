using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UXButton : MonoBehaviour
{
    [SerializeField] private UXButtonClickTypes clickType;
    [SerializeField] private bool disableOnClicked;
    private UISound uiSound;
    private Button button;

    private bool isInitialized;

    void OnEnable()
    {
        if (!isInitialized)
            Initialize();
    }

    void Start()
    {
        if (!isInitialized)
            Initialize();
    }

    void Initialize()
    {
        uiSound = UISound.Instance;
        TryGetComponent(out button);
        
        button.onClick.AddListener(ApplyClick);
        isInitialized = true;
    }

    public void ApplyClick()
    {
        if (disableOnClicked)
        {
            enabled = false;

            if (button != null)
                button.interactable = false;
        }

        if (uiSound == null)
        {
            Debug.Log("NO UI SFX");
            return;
        }

        switch (clickType)
        {
            case UXButtonClickTypes.Negative:
                uiSound.PlayUxNegativeClick();
                break;

            case UXButtonClickTypes.Positive:
                uiSound.PlayUxPositiveClick();
                break;
        }
    }
}
