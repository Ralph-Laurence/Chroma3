using UnityEngine;
using UnityEngine.UI;

public class FourSegmentLoader : MonoBehaviour
{
    [SerializeField] private Sprite[]   fillColors;
    [SerializeField] private Image      fill;
    [SerializeField] private float      fillRate = 1.25F;
    [SerializeField] private Image      caption;
    [SerializeField] private float      captionFillRate = 1.75F;

    private int fillIndex = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        caption.fillAmount = 0.0F;
    }

    // Update is called once per frame
    void Update()
    {
        if (fill.fillAmount >= 1.0F)
        {
            fillIndex++;
            
            if (fillIndex >= fillColors.Length)
                fillIndex = 0;

            fill.sprite     = fillColors[fillIndex];
            fill.fillAmount = 0.0F;
        }
        
        fill.fillAmount += Time.deltaTime * fillRate;
        fill.fillAmount = Mathf.Clamp(fill.fillAmount, 0.0F, 1.0F);

        if (caption.fillAmount <= 1.0F)
            caption.fillAmount += Time.deltaTime * captionFillRate;

        caption.fillAmount = Mathf.Clamp(caption.fillAmount, 0.0F, 1.0F);
    }

    void OnDisable()
    {
        fill.fillAmount = 0;
        fillIndex       = 0;
        fill.sprite     = fillColors[fillIndex];

        caption.fillAmount = 0;
    }
}
