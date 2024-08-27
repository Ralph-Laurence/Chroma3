using UnityEngine;

public class Connectors : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform[] points;
    public Transform p1;
    public Transform p2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer.positionCount = points.Length;
        lineRenderer.loop = true;
    }

    // Update is called once per frame
    void Update()
    {
        DrawPoints();
    }

    void DrawPoints()
    {
        float targetWidth = 0.02f; // Set your desired constant width
        float orthoSize = Camera.main.orthographicSize;
        float lineWidth = targetWidth * orthoSize;

        // Needs two points
        if (points.Length < 2)
            return;

        for (var i = 0; i < points.Length; i++)
        {
            var next = i + 1;

            if (next == points.Length)
                break;

            lineRenderer.SetPosition(i, points[i].position);
            lineRenderer.SetPosition(next, points[next].position);
        }

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
    }
}
