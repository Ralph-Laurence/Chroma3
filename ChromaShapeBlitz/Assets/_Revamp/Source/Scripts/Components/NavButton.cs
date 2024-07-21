using UnityEngine;
using UnityEngine.UI;

public class NavButton : MonoBehaviour
{
    public int TargetPageIndex;
    [SerializeField] private Image navIcon;
    [SerializeField] private Color activeColor = new(0.49F, 1.0F, 0.0F, 1.0F);

    private readonly Color defaultColor = Color.white;
    private Button button;

    void OnEnable() => NavButtonSelectedNotifier.BindEvent(ObserveSelected);
    void OnDisable() => NavButtonSelectedNotifier.UnbindEvent(ObserveSelected);

    private void ObserveSelected(int targetPageIndex)
    {
        if (targetPageIndex != TargetPageIndex)
        {
            navIcon.color = defaultColor;
            return;
        }

        navIcon.color = activeColor;
    }

    public Button ButtonComponent
    {
        get 
        {
            if (button == null)
                TryGetComponent(out button);

            return button;
        }
    }
}
