using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultBackground : MonoBehaviour
{
    [SerializeField] private Camera bgCam;
    private StageCamera stageCamera;

    // Start is called before the first frame update
    void Start()
    {
        var camObj = GameObject.FindWithTag(Constants.Tags.MainCamera);
        camObj.TryGetComponent(out stageCamera);

        if (stageCamera != null)
        {
            bgCam.transform.SetPositionAndRotation
            (
                stageCamera.transform.position, 
                stageCamera.transform.rotation
            );

            bgCam.orthographicSize = stageCamera.AttachedCamera.orthographicSize;
        }
    }

    void LateUpdate()
    {
        if (stageCamera != null)
        {
            bgCam.transform.rotation = stageCamera.transform.rotation;
            bgCam.orthographicSize = stageCamera.AttachedCamera.orthographicSize;
        }
    }
}
