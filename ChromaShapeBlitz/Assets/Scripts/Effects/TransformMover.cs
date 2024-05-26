using UnityEngine;

/// <summary>
/// Moves a transform by its direction
/// </summary>
public class TransformMover : MonoBehaviour
{
    [SerializeField] private MoveDirections moveDirection;
    [SerializeField] private float speed = 10.0F;
    [SerializeField] private bool delayedStart = false;
    [SerializeField] private float delay = 1.0F;
    
    private bool canMove = true;
    private Vector3 direction = Vector3.forward;

    void Awake()
    {
        switch (moveDirection)
        {
            case MoveDirections.Forward:
                direction = transform.forward;
                break;
            case MoveDirections.Backward:
                direction = transform.forward * -1.0F;
                break;
            case MoveDirections.Left:
                direction = transform.right * -1.0F;
                break;
            case MoveDirections.Right:
                direction = transform.right;
                break;
            case MoveDirections.None:
            default:
                direction = Vector3.zero;
            break;
        }
    }

    void Start()
    {
        if (delayedStart)
        {
            canMove = false;
            Invoke(nameof(SetCanMove), delay);
        }
    }

    void Update()
    {
        if (canMove)
            ApplyMove();
    }

    private void SetCanMove() => canMove = true;
    private void ApplyMove() => transform.position += speed * Time.deltaTime * direction;
}