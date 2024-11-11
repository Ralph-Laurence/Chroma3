using UnityEngine;

public class TargetFpsSetter : MonoBehaviour
{
    [SerializeField] private int MaxFps = 60;
    private static TargetFpsSetter instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            Application.targetFrameRate = MaxFps;
        }
    }

    public static TargetFpsSetter Instance => instance;
}
