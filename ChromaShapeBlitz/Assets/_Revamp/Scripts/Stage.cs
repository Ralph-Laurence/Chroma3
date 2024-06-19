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
        var variantIndex    = Random.Range(0, Variants.Length);
        var variantObject   = Instantiate(Variants[variantIndex]);

        variantObject.TryGetComponent(out StageVariant variantData);

        return variantData;
    }
}