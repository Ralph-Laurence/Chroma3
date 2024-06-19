using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectPage : MonoBehaviour
{
    [SerializeField] private float upScaleRate = 0.25F;
    private RectTransform rect;

    void Awake()
    {
        TryGetComponent(out rect);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        gameObject.SetActive(true);
        LeanTween.scale(rect, Vector3.one, upScaleRate);
    }
}
