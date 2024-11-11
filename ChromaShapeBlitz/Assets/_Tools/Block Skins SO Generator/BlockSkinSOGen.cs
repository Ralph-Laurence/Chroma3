using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class BlockSkinGenItem
{
    public int Id;
    public string Name;
    public string Category;
    public string Preview;
    public CurrencyType Cost;
    public int Price;
    public string Material;
}

[Serializable]
public class BlockSkinGenItemsArray
{
    public List<BlockSkinGenItem> BlockSkinsObjectDataSource;
}
#if UNITY_EDITOR
public class BlockSkinSOGen : MonoBehaviour
{
    [SerializeField] private string OutputPath = "Assets/_Revamp/Data/ScriptableObjects/BlockSkins/SkinAssets";
    [SerializeField] private string GroupsOutputPath = "Assets/_Revamp/Data/ScriptableObjects/BlockSkins/SkinGroups";
    [SerializeField] private TextAsset SkinsJSON;

    private BlockSkinGenItemsArray genItems;
    private Dictionary<string, string> outFolders;
    private Dictionary<string, ColorSwatches> colorCategories;
    private Dictionary<ColorSwatches, List<BlockSkinsAsset>> soGroups;

    void Awake()
    {
        outFolders = new Dictionary<string, string>()
        {
            {"Blue",    $"{OutputPath}/Blue"},
            {"Green",   $"{OutputPath}/Green"},
            {"Magenta", $"{OutputPath}/Magenta"},
            {"Orange",  $"{OutputPath}/Orange"},
            {"Purple",  $"{OutputPath}/Purple"},
            {"Yellow",  $"{OutputPath}/Yellow"},
        };

        colorCategories = new Dictionary<string, ColorSwatches>()
        {
            {"Blue",    ColorSwatches.Blue},
            {"Green",   ColorSwatches.Green},
            {"Magenta", ColorSwatches.Magenta},
            {"Orange",  ColorSwatches.Orange},
            {"Purple",  ColorSwatches.Purple},
            {"Yellow",  ColorSwatches.Yellow},
        };

        soGroups = new Dictionary<ColorSwatches, List<BlockSkinsAsset>>()
        {
            { ColorSwatches.Blue    , new List<BlockSkinsAsset>()},
            { ColorSwatches.Green   , new List<BlockSkinsAsset>()},
            { ColorSwatches.Magenta , new List<BlockSkinsAsset>()},
            { ColorSwatches.Orange  , new List<BlockSkinsAsset>()},
            { ColorSwatches.Purple  , new List<BlockSkinsAsset>()},
            { ColorSwatches.Yellow  , new List<BlockSkinsAsset>()},
        };
    }

    void Start() => StartCoroutine
    (
        GenerateSO()
    );

    private IEnumerator ValidOutFolders()
    {
        foreach (var folder in outFolders)
        {
            if (!AssetDatabase.IsValidFolder(folder.Value))
                AssetDatabase.CreateFolder(OutputPath, folder.Key);

            yield return null;
        }
    }

    private IEnumerator GenerateSO()
    {
        // Check if there are folders that exists for output paths
        yield return StartCoroutine(ValidOutFolders());

        // Read from the json data
        yield return StartCoroutine(Deserialize((d) => genItems = d));

        // Create the scriptable objects
        yield return StartCoroutine(CreateTemplates());

        // Add each scriptable objects according to their color group
        yield return StartCoroutine(CreateGroups());
        
        AssetDatabase.SaveAssets();
    }

    private IEnumerator Deserialize(Action<BlockSkinGenItemsArray> callback)
    {
        var data = JsonUtility.FromJson<BlockSkinGenItemsArray>(SkinsJSON.text);

        if (data == null)
            Debug.Log("data is null");

        callback?.Invoke(data);

        yield return null;
    }

    private IEnumerator CreateTemplates()
    {
        foreach (var item in genItems.BlockSkinsObjectDataSource){
            // Construct the file name
            var filename      = $"{item.Id}_{item.Name}.asset";
            var outpath       = outFolders[item.Category];
            var assetFile     = $"{outpath}/{filename}";
            var skinSOAsset   = ScriptableObject.CreateInstance<BlockSkinsAsset>();

            // Construct the preview paths and material paths
            var materialsPath = $"Materials/BlockSkins/{item.Category}/{item.Material}";
            var previewPath   = $"Sprites/ShopIcons/{item.Category}/{item.Preview}";
            var materialAsset = Resources.Load<Material>(materialsPath);
            var previewAsset  = Resources.Load<Sprite>(previewPath);

            var colorCategory = colorCategories[item.Category];

            if (materialAsset == null)
                Debug.LogWarning($"{materialsPath} cant be found");

            if (previewAsset == null)
                Debug.Log($"{previewPath} not found");

            skinSOAsset.Id            = item.Id;
            skinSOAsset.Name          = item.Name;
            skinSOAsset.Price         = item.Price;
            skinSOAsset.Cost          = item.Cost;

            skinSOAsset.ColorCategory    = colorCategory;
            skinSOAsset.MaterialFilename = item.Material;
            
            if (materialAsset != null)
                skinSOAsset.Material = materialAsset;

            if (previewAsset != null)
                skinSOAsset.PreviewImage = previewAsset;
            
            soGroups[colorCategory].Add(skinSOAsset);

            AssetDatabase.CreateAsset(skinSOAsset, assetFile);
            yield return null;
        }
    }

    private IEnumerator CreateGroups()
    {
        foreach (var group in soGroups)
        {
            var file  = $"{GroupsOutputPath}/Skin-Group-{group.Key}.asset";
            var asset = ScriptableObject.CreateInstance<BlockSkinsAssetGroup>();
            
            asset.SkinGroup = group.Value;
            asset.GroupName = $"{group.Key} Skins Group";

            AssetDatabase.CreateAsset(asset, file);

            yield return null;
        }
    }
}
#endif