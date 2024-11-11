using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] private float speed = 10.0F;
    [SerializeField] private Vector3 direction;

    [SerializeField] private bool useMultiplier;
    [SerializeField] private float multiplier;

    // Start is called before the first frame update
    void Start()
    {
        if (useMultiplier)
            speed *= multiplier;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(speed * Time.deltaTime * direction);
    }
}
