using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] private HintMarker hintMarker;
    
    // [SerializeField] private StageCamera stageCamera;
    [SerializeField] private PatternTimer timer;

    void Update()
    {
        // if (Input.GetKeyUp(KeyCode.F1))
        //     StartCoroutine(hintMarker.ShowHints());

        // if (Input.GetKeyUp(KeyCode.F3))
        //     timer.AddTime(60);
    }
}

