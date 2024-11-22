using UnityEngine;

public class CreditsController : MonoBehaviour
{
    [SerializeField] private AudioClip bgmCredits;
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

        bgm.SetClip(bgmCredits);
        bgm.Play();
    }

    public void GoBack()
    {
        Instantiate(fancySceneLoader).TryGetComponent(out FancySceneLoader loader);
        loader.LoadScene(Constants.Scenes.MainMenu);
    }
}
