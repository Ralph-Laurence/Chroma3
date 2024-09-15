using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] private HintMarker hintMarker;
    

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F1))
            StartCoroutine(hintMarker.ShowHints());
    }

    
}

