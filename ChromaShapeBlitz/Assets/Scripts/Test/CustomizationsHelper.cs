using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizationsHelper : MonoBehaviour
{
    /* #region SINGLETON */
    //========================================================
    //
    private static CustomizationsHelper instance;

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

    public static CustomizationsHelper Instance => instance;
    //
    //========================================================
    /* #endregion */

    // The templates for building the Visual Skins.
    [SerializeField] private BlockSkinGroupSO[] skinGroups;

    // This is responsible for loading and saving every
    // data into Binary Format.
    private CustomizationsIO customizationsIO;

    // When we load data from Binary, we store them here.
    private Customizations deserializedData;

    // These are the Visual Skins we see. Every skin
    // details such as Id, Name and Price are stored here.
    public Dictionary<int, BlockSkinSO> GraphicSkins { get; private set; }

    // These are the Skin Ids that were last applied.
    private Dictionary<BlockColors, int> equipedSkinIds;

    // Default Skins are the initial appearance of blocks,
    // before we purchased custom skins. These are the
    // ids of PLAIN-Color skins.
    private List<int> defaultSkinIds;

    // Owned Skins are the ids of skins we currently have.
    // These are the ids of all Purchased skins and also
    // includes the Default skins ids
    private List<int> ownedSkinIds;

    public List<int> GetOwnedSkinIds()   => ownedSkinIds;
    public List<int> GetDefaultSkinIds() => defaultSkinIds;
    public Dictionary<BlockColors, int> GetEquipedSkinIds() => equipedSkinIds;
    
    /// <summary>
    /// Call this in the EntryPoint component
    /// </summary>
    /// <returns>Coroutine</returns>
    public IEnumerator LoadCustomizations()
    {
        // Get a reference to the Singleton instance of the ThemeIO.
        // This handles the saving and loading of binary theme data.
        customizationsIO = CustomizationsIO.Instance;

        // When a skin file data isnt created yet, then
        // we create a fresh binary data
        if (!customizationsIO.FileExists)
        {
            deserializedData = CreateDefaultData();

            yield return StartCoroutine(customizationsIO.Write(deserializedData));
        }

        // Otherwise, we load the existing data from the binary file
        else
        {
            yield return StartCoroutine(customizationsIO.Read((t) => deserializedData = t));
        }

        // The Post Initialize method is used for
        // initialization of Non-Binary related data.
        yield return StartCoroutine(PostInitialize());

        yield return null;
    }

    /// <summary>
    /// Create a fresh theme data, with the skin ids set to default id
    /// </summary>
    /// <returns>New theme data</returns>
    private Customizations CreateDefaultData() => new Customizations()
    {
        BlueTheme       = new ThemeItem { EquippedSkinId = 1 },
        GreenTheme      = new ThemeItem { EquippedSkinId = 2 },
        MagentaTheme    = new ThemeItem { EquippedSkinId = 3 },
        OrangeTheme     = new ThemeItem { EquippedSkinId = 4 },
        PurpleTheme     = new ThemeItem { EquippedSkinId = 5 },
        YellowTheme     = new ThemeItem { EquippedSkinId = 6 },

        OwnedSkins      = new List<int>() {  1, 2, 3, 4, 5, 6 },
        DefaultSkins    = new List<int>() {  1, 2, 3, 4, 5, 6 },
    };

    private IEnumerator PostInitialize()
    {
        ownedSkinIds    = deserializedData.OwnedSkins;
        defaultSkinIds  = deserializedData.DefaultSkins;
        GraphicSkins    = new Dictionary<int, BlockSkinSO>();

        // Fill the lookup table (dictionary) with the skins
        // data loaded from the Scriptable Objects.
        foreach (var skinGroup in skinGroups)
        {
            skinGroup.AddTo(GraphicSkins);
            yield return null;
        }

        equipedSkinIds = new Dictionary<BlockColors, int>
        {
            { BlockColors.Blue,     deserializedData.BlueTheme.EquippedSkinId    },
            { BlockColors.Green,    deserializedData.GreenTheme.EquippedSkinId   },
            { BlockColors.Orange,   deserializedData.OrangeTheme.EquippedSkinId  },
            { BlockColors.Purple,   deserializedData.PurpleTheme.EquippedSkinId  },
            { BlockColors.Magenta,  deserializedData.MagentaTheme.EquippedSkinId },
            { BlockColors.Yellow,   deserializedData.YellowTheme.EquippedSkinId  },
        };
    }

    /// <summary>
    /// Purchase the skin then equip it.
    /// </summary>
    /// <param name="skinId">The ID of the desired skin</param>
    /// <param name="colorGroup">The color category where the skin belongs</param>
    /// <returns>Coroutine</returns>
    public IEnumerator TakeSkinOwnership(int skinId, BlockColors colorGroup)
    {
        if (!ownedSkinIds.Contains(skinId))
            ownedSkinIds.Add(skinId);

        yield return StartCoroutine(EquipSkin(skinId, colorGroup));
    }

    /// <summary>
    /// This will update the last equiped skin id with the newer id.
    /// </summary>
    /// <param name="skinId">The ID of the desired skin</param>
    /// <param name="colorGroup">The color category where the skin belongs</param>
    /// <returns>Coroutine</returns>
    public IEnumerator EquipSkin(int skinId, BlockColors colorGroup)
    {
        // Update the On-Disk equipped skins
        var equipActions = new Dictionary<BlockColors, Action<int>>
        {
            { BlockColors.Blue    , (id) => deserializedData.BlueTheme.EquippedSkinId    = id },
            { BlockColors.Green   , (id) => deserializedData.GreenTheme.EquippedSkinId   = id },
            { BlockColors.Orange  , (id) => deserializedData.OrangeTheme.EquippedSkinId  = id },
            { BlockColors.Purple  , (id) => deserializedData.PurpleTheme.EquippedSkinId  = id },
            { BlockColors.Magenta , (id) => deserializedData.MagentaTheme.EquippedSkinId = id },
            { BlockColors.Yellow  , (id) => deserializedData.YellowTheme.EquippedSkinId  = id },
        };

        equipActions[colorGroup]?.Invoke(skinId);

        yield return StartCoroutine(customizationsIO.Write(deserializedData));

        // Update the In-Memory equipped skins
        equipedSkinIds[colorGroup] = skinId;

        yield return null;
    }

    /// <summary>
    /// Apply block texture onto the material.
    /// </summary>
    /// <param name="blockMaterial"></param>
    public bool FillBlockWithSkin(Material blockMaterial, BlockColors colorGroup)
    {
        if (!equipedSkinIds.ContainsKey(colorGroup))
            return false;
            
        var skinId = equipedSkinIds[colorGroup];

        // If the currently equipped skin ID is in the list of
        // the default Ids, then we wont apply a skin texture.
        // The default color material will be used instead.
        if (colorGroup == BlockColors.None || defaultSkinIds.Contains(skinId))
            return false;

        var skinToUse = GraphicSkins[skinId].Skin;

        blockMaterial.color = Color.white;
        //blockMaterial.SetTexture(Constants.SHADER_BASE_MAP, skinToUse); // URP
        blockMaterial.SetTexture(Constants.SHADER_MAIN_TEX, skinToUse);
        return true;
    }
}