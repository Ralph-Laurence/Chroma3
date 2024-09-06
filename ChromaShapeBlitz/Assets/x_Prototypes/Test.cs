using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F1))
            ProgressLoaderNotifier.NotifyFourSegment(true);

        else if (Input.GetKeyDown(KeyCode.F2))
            ProgressLoaderNotifier.NotifyIndefiniteBar(true);
#endif
    }
}