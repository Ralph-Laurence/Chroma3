using UnityEngine;

/// <summary>
/// LinearPropEntity component uses the gameobject's forward transform for its movement.
/// </summary>
public class LinearPropEntity : MonoBehaviour
{
    public float MoveSpeed = 1.0F;

    [Space(5)]
    [Tooltip("When set to \"Controlled by script\", Bounds Axes values are upto the script's decision.\n\nThe Min and Max Bounds are still used however.")]
    public LinearBounds DisableOnReachedBounds;

    [Space(5)]    
    public Bounds XBounds;
    public Bounds ZBounds;

    [Space(5)]
    public bool CanMove = true;

    void Update()
    {
        if (CanMove)
           Move();
    }

    private void Move()
    {
        // Move the entity to the direction it is facing at
        transform.position += MoveSpeed * Time.deltaTime * transform.forward;

        // Get the current position and check if it goes outside its bounds
        var pos = transform.position;

        switch (DisableOnReachedBounds)
        {
            case LinearBounds.XAxis:
                pos.x = Mathf.Clamp(pos.x, XBounds.Min, XBounds.Max);
                pos.z = 0.0F;

                if (BoundsReached(pos.x, XBounds))
                    Halt();

                break;

            case LinearBounds.ZAxis:
                pos.z = Mathf.Clamp(pos.z, ZBounds.Min, ZBounds.Max);
                pos.x = 0.0F;

                if (BoundsReached(pos.z, ZBounds))
                    Halt();

                break;
        }
    }

    /// <summary>
    /// Stop the entity from moving then disable it
    /// </summary>
    private void Halt()
    {
        CanMove = false;

        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);
    }

    private bool BoundsReached(float value, Bounds bounds) => (value <= bounds.Min || value >= bounds.Max);
}