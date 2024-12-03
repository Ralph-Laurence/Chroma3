using UnityEngine;

public class StageSelectPage : MonoBehaviour
{
    [SerializeField] private float upScaleRate = 0.25F;
    private RectTransform rect;

    void Awake()
    {
        TryGetComponent(out rect);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        LeanTween.scale(rect, Vector3.one, upScaleRate);
    }
}
