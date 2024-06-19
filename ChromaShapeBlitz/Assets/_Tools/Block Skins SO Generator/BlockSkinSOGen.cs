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
    public string Texture;
}

[Serializable]
public class BlockSkinGenItemsArray
{
    public List<BlockSkinGenItem> SOItems;
}
#if UNITY_EDITOR
public class BlockSkinSOGen : MonoBehaviour
{
    [SerializeField] private string BlockSkinsSO_OutPath = "Assets/Scriptable Objects/Block Skins/Templates";
    [SerializeField] private string BlockSkinsSOGroup_OutPath = "Assets/Scriptable Objects/Block Skins/Groups";
    [SerializeField] private TextAsset SkinsDatasource;

    private BlockSkinGenItemsArray genItems;
    private Dictionary<string, string> outFolders;
    private Dictionary<string, ColorSwatches> colorCategories;
    private Dictionary<ColorSwatches, List<Rv_BlockSkinSO>> soGroups;

    void Awake()
    {
        outFolders = new Dictionary<string, string>()
        {
            {"Blue",    $"{BlockSkinsSO_OutPath}/Blue"},
            {"Green",   $"{BlockSkinsSO_OutPath}/Green"},
            {"Magenta", $"{BlockSkinsSO_OutPath}/Magenta"},
            {"Orange",  $"{BlockSkinsSO_OutPath}/Orange"},
            {"Purple",  $"{BlockSkinsSO_OutPath}/Purple"},
            {"Yellow",  $"{BlockSkinsSO_OutPath}/Yellow"},
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

        soGroups = new Dictionary<ColorSwatches, List<Rv_BlockSkinSO>>()
        {
            { ColorSwatches.Blue    , new List<Rv_BlockSkinSO>()},
            { ColorSwatches.Green   , new List<Rv_BlockSkinSO>()},
            { ColorSwatches.Magenta , new List<Rv_BlockSkinSO>()},
            { ColorSwatches.Orange  , new List<Rv_BlockSkinSO>()},
            { ColorSwatches.Purple  , new List<Rv_BlockSkinSO>()},
            { ColorSwatches.Yellow  , new List<Rv_BlockSkinSO>()},
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
                AssetDatabase.CreateFolder(BlockSkinsSO_OutPath, folder.Key);

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
        var data = JsonUtility.FromJson<BlockSkinGenItemsArray>(SkinsDatasource.text);

        if (data == null)
            Debug.Log("data is null");

        callback?.Invoke(data);

        yield return null;
    }

    private IEnumerator CreateTemplates()
    {
        foreach (var item in genItems.SOItems){
            // Construct the file name
            var filename      = $"{item.Id} - {item.Name}.asset";
            var outpath       = outFolders[item.Category];
            var assetFile     = $"{outpath}/{filename}";
            var skinSOAsset   = ScriptableObject.CreateInstance<Rv_BlockSkinSO>();

            // Construct the preview paths and texture paths
            var texturePath   = $"Textures/Blocks/{item.Category}/{item.Texture}";
            var previewPath   = $"Sprites/ShopIcons/{item.Category}/{item.Preview}";
            var textureAsset  = Resources.Load<Texture2D>(texturePath);
            var previewAsset  = Resources.Load<Sprite>(previewPath);

            var colorCategory = colorCategories[item.Category];

            if (textureAsset == null)
                Debug.LogWarning($"{texturePath} cant be found");

            if (previewAsset == null)
                Debug.Log($"{previewPath} not found");

            skinSOAsset.SkinInfo = new BaseItemInfo
            {
                Id            = item.Id,
                Name          = item.Name,
                Price         = item.Price,
                Cost          = item.Cost,
                ColorCategory = colorCategory
            };

            if (textureAsset != null)
                skinSOAsset.SkinTexture = textureAsset;

            if (previewAsset != null)
                skinSOAsset.SkinInfo.PreviewImage = previewAsset;
            
            soGroups[colorCategory].Add(skinSOAsset);

            AssetDatabase.CreateAsset(skinSOAsset, assetFile);
            yield return null;
        }
    }

    private IEnumerator CreateGroups()
    {
        foreach (var group in soGroups)
        {
            var file  = $"{BlockSkinsSOGroup_OutPath}/Skin-Group-{group.Key.ToColorName()}.asset";
            var asset = ScriptableObject.CreateInstance<Rv_BlockSkinGroupSO>();
            
            asset.SkinGroup = group.Value;
            asset.GroupName = $"{group.Key.ToColorName()} Skins Group";

            AssetDatabase.CreateAsset(asset, file);

            yield return null;
        }
    }
}
#endif