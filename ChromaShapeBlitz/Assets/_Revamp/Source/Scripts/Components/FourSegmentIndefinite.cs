using UnityEngine;

public class FourSegmentIndefinite : MonoBehaviour
{
    [SerializeField] private RectTransform fill;
    [SerializeField] private float maxX = 220.0F;
    [SerializeField] private float speed = 64.0F;

    private Vector3 fillPosition;
    private float minX;

    void Awake()
    {
        fillPosition = fill.anchoredPosition;
        minX = -fill.sizeDelta.x;
    }

    void Update()
    {
        fillPosition.x += Time.deltaTime * speed;
        fillPosition.x = Mathf.Clamp(fillPosition.x, minX, maxX);

        if (fillPosition.x >= maxX)
            ResetPosition();

        fill.anchoredPosition = fillPosition;
    }

    private void ResetPosition()
    {
        fillPosition.x = minX;
        fill.anchoredPosition = fillPosition;
    }

    void OnDisable() => ResetPosition();
}