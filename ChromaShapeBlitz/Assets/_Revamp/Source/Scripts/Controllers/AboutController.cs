using UnityEngine;

public class AboutController : MonoBehaviour
{
    [SerializeField] private AudioClip bgmAbout;
    [SerializeField] private GameObject fancySceneLoader;

    private BackgroundMusic bgm;

    void Awake()
    {
        bgm = BackgroundMusic.Instance;
    }

    void Start()
    {
        if (bgm == null)
            return;
           
        bgm.SetClip(bgmAbout);
        bgm.Play();
    }

    public void GoBack()
    {
        Instantiate(fancySceneLoader).TryGetComponent(out FancySceneLoader loader);
        loader.LoadScene(Constants.Scenes.MainMenu);
    }
}
