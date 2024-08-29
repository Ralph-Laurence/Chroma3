using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class Tagger : MonoBehaviour
{
#if UNITY_EDITOR
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.LogWarning("Begin Retag");
        StartCoroutine(Begin());
    }

    private IEnumerator Begin()
    {
        // Retag Normal levels
        var level = LevelDifficulties.Normal;
        var totalLevels = 16;
        for (var i = 0; i < totalLevels; i++)
        {
            var levelIdx = i + 1;

            // i.e. Stage_1
            var stageFolderPath = $"Assets/Resources/Stages/{level}/Stage_{levelIdx}";
            var prefabAssets = AssetDatabase.FindAssets("t:Prefab", new[] { stageFolderPath });

            // Loop through each prefab asset then load it
            foreach (var guid in prefabAssets)
            {
                // Get the path of the asset
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);

                // Load the prefab
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                yield return StartCoroutine(Retag(prefab));
            }

            yield return null;
        }

        Debug.Log("Finished");
    }

    private IEnumerator Retag(GameObject prefabAsset)
    {
        var root = prefabAsset.transform.Find("Block Grid");

        if (root == null)
            yield break;
        
        var rowCount = root.childCount;
        var tagLight = "LightBlock";
        var tagDark  = "DarkBlock";

        for (int rowIdx = 0; rowIdx < rowCount; rowIdx++)
        {
            var blocks = root.GetChild(rowIdx);

            foreach (Transform block in blocks)
            {
                var colIdxStr = block.name[^1]; // block.name[ block.name.Length - 1 ];
                var colIdx = Convert.ToInt32(colIdxStr);

                // Even Rows, first child must be tagged Light, and second child tagged as Dark
                if (IsEven(rowIdx))
                    block.tag = IsEven(colIdx) ? tagDark : tagLight;

                // Odd Rows, first child must be tagged Dark, and second child tagged as Light
                else
                    block.tag = IsEven(colIdx) ? tagLight : tagDark;
            }
        }

        // Save the changes
        PrefabUtility.SavePrefabAsset(prefabAsset);

        yield return null;
    }
#endif
    private bool IsEven(int value) => value % 2 == 0;
}
