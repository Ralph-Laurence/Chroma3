using UnityEngine;

public class SoundBank : MonoBehaviour
{
    private static SoundBank instance;

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
        }
    }

    public static SoundBank Instance => instance;

    public string GetName() => gameObject.name;
}
