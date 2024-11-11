using UnityEngine;

public class SpecialVisionPowerupObserver : MonoBehaviour
{
    [SerializeField] private GameObject visorVFX;
    [SerializeField] private GameObject xrayVFX;

    void OnEnable() => SpecialVisionPowerupNotifier.BindObserver(OnPowerupEffectNotified);

    void OnDisable() => SpecialVisionPowerupNotifier.UnbindObserver(OnPowerupEffectNotified);

    private void OnPowerupEffectNotified(int effectValue)
    {
        switch (effectValue)
        {
            case Constants.PowerupEffectValues.POWERUP_EFFECT_VISOR:
                visorVFX.SetActive(true);
                break;

            case Constants.PowerupEffectValues.POWERUP_EFFECT_XRAY:
                xrayVFX.SetActive(true);
                break;
        }
    }
}