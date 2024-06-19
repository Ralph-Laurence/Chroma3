using System.Collections;
using UnityEngine;

public class SelectedBlockMarker : MonoBehaviour
{
    public float blinkSpeed = 2.0f;

    public GameObject marker;
    private Material markerMaterial;
    private Color colorStart = Color.red;
    private Color colorEnd = Color.black;

    void Start()
    {
        InitMaterial();
        
        // if (gameObject.activeInHierarchy)
        //     StartCoroutine(nameof(Blink));
    }

    void OnEnable()
    {
        if (markerMaterial == null)
            InitMaterial();
            
        StartCoroutine(nameof(Blink));
    }

    void OnDisable()
    {
        StopCoroutine(nameof(Blink));
    }

    private void InitMaterial()
    {
        markerMaterial = marker.GetComponent<Renderer>().material;
    }

    IEnumerator Blink()
    {
        while (true)
        {
            float t = Mathf.PingPong(Time.time * blinkSpeed, 1.0f);
            markerMaterial.color = Color.Lerp(colorStart, colorEnd, t);
            yield return null;
        }
    }
}