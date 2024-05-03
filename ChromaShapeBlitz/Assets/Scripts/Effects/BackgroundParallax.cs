using UnityEngine;
using UnityEngine.UI;

public class BackgroundParallax : MonoBehaviour
{
    [SerializeField] private RawImage parallaxOverlay;
    [SerializeField] private Vector2 uvPoint = Vector2.zero;
    [SerializeField] private Vector2 uvScale = Vector2.one;
    [SerializeField] private Vector2 uvSpeed = Vector2.one;
    [SerializeField] private bool reverse = false;

    private Rect uv;

    void Start()
    {
        uv = new Rect(
            uvPoint.x,  // x
            uvPoint.y,  // y
            uvScale.x,  // w
            uvScale.y   // h
        );

        parallaxOverlay.uvRect = uv;
    }

    void Update()
    {
        if (reverse)
        {
            uv.x += uvSpeed.x * Time.deltaTime;
            uv.y += uvSpeed.y * Time.deltaTime;
        }
        else
        {
            uv.x -= uvSpeed.x * Time.deltaTime;
            uv.y -= uvSpeed.y * Time.deltaTime;
        }

        parallaxOverlay.uvRect = uv;
    }
}