/*using UnityEngine;

public class Stage : MonoBehaviour
{
    private BackgroundMusic bgm;

    public LevelDifficulties Difficulty;
    public int Number;
    public GameObject[] Variants;

    void Awake()
    {
        bgm = BackgroundMusic.Instance;
    }

    void Start()
    {
        // Spawn a random variant
        var variantIndex    = Random.Range(0, Variants.Length);
        var variantObject   = Instantiate(Variants[variantIndex]);

        variantObject.TryGetComponent(out StageVariant variantData);

        bgm.SetClip(variantData.BgmClip);
        bgm.Play();
    }
}*/