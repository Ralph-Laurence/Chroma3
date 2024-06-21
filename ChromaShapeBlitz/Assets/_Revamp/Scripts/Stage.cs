using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "Scriptable Objects/Stage")]
public class Stage : ScriptableObject
{
    public LevelDifficulties Difficulty;
    public int Number;
    
    public GameObject[] Variants;

    public StageVariant PickRandom()
    {
        // Spawn a random variant
        var variantIndex    = ShuffleUniqueIndex(); //Random.Range(0, Variants.Length);
        var variantObject   = Instantiate(Variants[variantIndex]);

        variantObject.TryGetComponent(out StageVariant variantData);

        return variantData;
    }

    private int ShuffleUniqueIndex()
    {
        var gsm = GameSessionManager.Instance;

        if (gsm == null)
            return Random.Range(0, Variants.Length);

        int index = default;
        var maxAttempts = 10;

        for (int i = 0; index == gsm.LastRandomSpawnIndex && i < maxAttempts; i++)
        {
            index = Random.Range(0, Variants.Length);
        }

        gsm.LastRandomSpawnIndex = index;

        return index;
    }
}