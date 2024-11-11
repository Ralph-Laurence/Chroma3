using UnityEngine;

public class Cheet : MonoBehaviour
{
    private float fillRate = 0.24F;

    public void Spd25()
    {
        Debug.Log("Speed 25%");
        fillRate = 0.2F;
        ApplySpeed(fillRate);
    }

    public void Spd50()
    {
        Debug.Log("Speed 50%");
        fillRate = 0.18F;
        ApplySpeed(fillRate);
    }

    public void Spd75()
    {
        Debug.Log("Speed 75%");
        fillRate = 0.16F;
        ApplySpeed(fillRate);
    }

    public void FullSpd()
    {
        Debug.Log("Full Speed");
        fillRate = 0.08F;
        ApplySpeed(fillRate);
    }

    private void ApplySpeed(float speed)
    {
        var cmp = Object.FindFirstObjectByType<StageVariant>();

        if (cmp != null)
            cmp.SetFillRate(speed);
        else
            Debug.LogWarning("Cant find such object");
    }
}
