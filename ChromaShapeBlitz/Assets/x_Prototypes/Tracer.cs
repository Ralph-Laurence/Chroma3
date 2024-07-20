using UnityEngine;
using UnityEngine.UI;

public class Tracer : MonoBehaviour
{
    public Image fill;
    public Transform[] waypoints; // An array of your rect transforms
    public float speed = 5f; // Movement speed

    private int currentWaypointIndex = 0;

    private void Update()
    {
        if (currentWaypointIndex < waypoints.Length)
        {
            // Move towards the current waypoint
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Check if we've reached the waypoint
            if (transform.position == targetPosition)
            {
                currentWaypointIndex++; // Move to the next waypoint
            }
        }
    }
}