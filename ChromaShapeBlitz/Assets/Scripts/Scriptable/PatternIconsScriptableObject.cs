using UnityEngine;

[CreateAssetMenu(fileName = "Patterns Data", menuName = "Scriptable Objects/Pattern Icons", order = 3)]
public class PatternIconsScriptableObject : ScriptableObject
{
    public Sprite[] StagePatterns;

    public Sprite GetStagePattern(int stageNumber) => StagePatterns[stageNumber - 1];
}