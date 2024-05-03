using UnityEngine;
using UnityEngine.UI;

public class UXButton : MonoBehaviour
{
    public bool DisableOnClicked;

    private int UX_NEGATIVE = -1;
    private int UX_POSITIVE = 1;

    public void TriggerPositiveClick()
    {
        ApplyCLick();
        PlayClickSfx(UX_POSITIVE);
    }

    public void TriggerNegativeClick()
    {
        ApplyCLick();
        PlayClickSfx(UX_NEGATIVE);
    }

    private void ApplyCLick()
    {
        if (DisableOnClicked)
        {
            enabled = false;

            TryGetComponent(out Button button);

            if (button != null)
                button.enabled = false;
        }
    }

    private void PlayClickSfx(int uxType)
    {
        if (SfxManager.Instance == null)
            return;

        if (uxType == UX_POSITIVE)
        {
            SfxManager.Instance.PlayUxPositiveClick();
            return;
        }

        SfxManager.Instance.PlayUxNegativeClick();
    }
}
