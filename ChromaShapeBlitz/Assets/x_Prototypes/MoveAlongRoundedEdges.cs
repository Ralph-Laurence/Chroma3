using UnityEngine;
using System.Collections.Generic;

public class MoveAlongRoundedEdges : MonoBehaviour
{
    // public RectTransform mover;
    // public RectTransform square;
    // public float speed = 100f; // Units per second
    // public float cornerRadius = 20f; // Radius of the rounded corners
    // public int cornerSegments = 10; // Number of segments to approximate the curve

    // private List<Vector2> path;
    // private int currentPathIndex = 0;

    // void Start()
    // {
    //     path = new List<Vector2>();

    //     // Define the edges and corners of the square (clockwise starting from top-left)
    //     float halfWidth = square.rect.width / 2 - cornerRadius;
    //     float halfHeight = square.rect.height / 2 - cornerRadius;

    //     // Add the straight segments
    //     path.Add(new Vector2(-halfWidth, halfHeight + cornerRadius)); // Top-left straight
    //     path.Add(new Vector2(halfWidth, halfHeight + cornerRadius)); // Top-right straight
    //     path.Add(new Vector2(halfWidth + cornerRadius, halfHeight)); // Top-right corner
    //     path.Add(new Vector2(halfWidth + cornerRadius, -halfHeight)); // Bottom-right straight
    //     path.Add(new Vector2(halfWidth, -halfHeight - cornerRadius)); // Bottom-right corner
    //     path.Add(new Vector2(-halfWidth, -halfHeight - cornerRadius)); // Bottom-left straight
    //     path.Add(new Vector2(-halfWidth - cornerRadius, -halfHeight)); // Bottom-left corner
    //     path.Add(new Vector2(-halfWidth - cornerRadius, halfHeight)); // Top-left straight

    //     // Add the curved segments (approximating with straight lines)
    //     AddRoundedCorner(new Vector2(halfWidth, halfHeight), 180f, 270f); // Top-right corner
    //     AddRoundedCorner(new Vector2(halfWidth, -halfHeight), 270f, 360f); // Bottom-right corner
    //     AddRoundedCorner(new Vector2(-halfWidth, -halfHeight), 0f, 90f); // Bottom-left corner
    //     AddRoundedCorner(new Vector2(-halfWidth, halfHeight), 90f, 180f); // Top-left corner

    //     // Set the initial target position
    //     mover.anchoredPosition = path[currentPathIndex];
    // }

    // void Update()
    // {
    //     // Move towards the target position
    //     mover.anchoredPosition = Vector2.MoveTowards(mover.anchoredPosition, path[currentPathIndex], speed * Time.deltaTime);

    //     // Check if the mover has reached the target position
    //     if (Vector2.Distance(mover.anchoredPosition, path[currentPathIndex]) < 0.01f)
    //     {
    //         // Update the target position to the next point in the path
    //         currentPathIndex = (currentPathIndex + 1) % path.Count;
    //     }
    // }

    // void AddRoundedCorner(Vector2 center, float startAngle, float endAngle)
    // {
    //     float angleStep = (endAngle - startAngle) / cornerSegments;
    //     for (int i = 0; i <= cornerSegments; i++)
    //     {
    //         float angle = startAngle + i * angleStep;
    //         float radian = angle * Mathf.Deg2Rad;
    //         Vector2 point = new Vector2(
    //             center.x + cornerRadius * Mathf.Cos(radian),
    //             center.y + cornerRadius * Mathf.Sin(radian)
    //         );
    //         path.Add(point);
    //     }
    // }

    public RectTransform mover;
    public RectTransform square;
    public float speed = 100f; // Units per second
    public float cornerRadius = 20f; // Radius of the rounded corners
    public int cornerSegments = 10; // Number of segments to approximate the curve

    private List<Vector2> path;
    private int currentPathIndex = 0;

    void Start()
    {
        // Initialize the path
        CalculatePath();

        // Set the initial position of the mover
        mover.anchoredPosition = path[0];
    }

    void Update()
    {
        // Move towards the target position
        mover.anchoredPosition = Vector2.MoveTowards(mover.anchoredPosition, path[currentPathIndex], speed * Time.deltaTime);

        // Check if the mover has reached the target position
        if (Vector2.Distance(mover.anchoredPosition, path[currentPathIndex]) < 0.01f)
        {
            // Update the target position to the next point in the path
            currentPathIndex = (currentPathIndex + 1) % path.Count;
        }
    }

    void CalculatePath()
    {
        path = new List<Vector2>();

        // Define the edges and corners of the square (clockwise starting from top-left)
        float halfWidth = square.rect.width / 2 - cornerRadius;
        float halfHeight = square.rect.height / 2 - cornerRadius;

        // Add the straight segments
        path.Add(new Vector2(-halfWidth, halfHeight + cornerRadius)); // Top-left straight
        path.Add(new Vector2(halfWidth, halfHeight + cornerRadius)); // Top-right straight
        AddRoundedCorner(new Vector2(halfWidth, halfHeight), 180f, 270f); // Top-right corner
        path.Add(new Vector2(halfWidth + cornerRadius, -halfHeight)); // Bottom-right straight
        AddRoundedCorner(new Vector2(halfWidth, -halfHeight), 270f, 360f); // Bottom-right corner
        path.Add(new Vector2(-halfWidth, -halfHeight - cornerRadius)); // Bottom-left straight
        AddRoundedCorner(new Vector2(-halfWidth, -halfHeight), 0f, 90f); // Bottom-left corner
        path.Add(new Vector2(-halfWidth - cornerRadius, halfHeight)); // Top-left straight
        AddRoundedCorner(new Vector2(-halfWidth, halfHeight), 90f, 180f); // Top-left corner
    }

    void AddRoundedCorner(Vector2 center, float startAngle, float endAngle)
    {
        float angleStep = (endAngle - startAngle) / cornerSegments;
        for (int i = 0; i <= cornerSegments; i++)
        {
            float angle = startAngle + i * angleStep;
            float radian = angle * Mathf.Deg2Rad;
            Vector2 point = new Vector2(
                center.x + cornerRadius * Mathf.Cos(radian),
                center.y + cornerRadius * Mathf.Sin(radian)
            );
            path.Add(point);
        }
    }
}
