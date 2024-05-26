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

    // This is responsible for loading and saving data into Binary Format.
    private CustomizationsIO customizationsIO;

    // When we load data from Binary, we store them here.
    private Customizations deserializedData;
    
    /**************************************************************/
    /******************* BLOCK SKIN PROPERTIES ********************/
    /**************************************************************/

    // The templates for building the Visual Skins.
    [SerializeField] private BlockSkinGroupSO[] skinGroups;

    // These are the Visual Skins we see. Every skin
    // details such as Id, Name and Price are stored here.
    public Dictionary<int, BlockSkinSO> GraphicSkins { get; private set; }

    // These are the Skin Ids that were last applied.
    public Dictionary<BlockColors, int> EquippedSkinIds { get; private set; }

    // Default Skins are the initial appearance of blocks,
    // before we purchased custom skins. These are the
    // ids of PLAIN-Color skins.
    public List<int> DefaultSkinIds { get; private set; }

    // Owned Skins are the ids of skins we currently have.
    // These are the ids of all Purchased skins and also
    // includes the Default skins ids
    public List<int> OwnedSkinIds { get; private set; }

    /**************************************************************/
    /******************* BACKGROUND PROPERTIES ********************/
    /**************************************************************/
    
    // The templates for building the Visual Backgrounds.
    //[SerializeField] private BackgroundGroupSO[] backgroundGroups;

    // These are the Visual Backgrounds we see. Every background
    // details such as Id, Name and Price are stored here.
    //public Dictionary<int, BackgroundSO> GraphicBackgrounds { get; private set; }

    // This is the Background Id that we last applied.
    //public int EquippedBackgroundId { get; private set; }

    // Owned Backgrounds are the ids of backgrounds we currently have.
    // These are the ids of all Purchased Backgrounds and also
    // includes the Default Background id
    //public List<int> OwnedBackgroundIds { get; set; }

    //public int DefaultBackgroundId => 0;

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
        EquippedSkinId = new EquippedSkinIds
        {
            Blue    = 1,
            Green   = 2,
            Magenta = 3,
            Orange  = 4,
            Purple  = 5,
            Yellow  = 6,
        },
        
        //DefaultBackgroundId  = DefaultBackgroundId,
        EquippedBackgroundId = 0, // NONE
        OwnedBackgrounds     = new List<int>() { 0 },
        OwnedSkins           = new List<int>() { 1, 2, 3, 4, 5, 6 },
        DefaultSkins         = new List<int>() { 1, 2, 3, 4, 5, 6 },
    };

    private IEnumerator PostInitialize()
    {
        //OwnedBackgroundIds  = deserializedData.OwnedBackgrounds;
        OwnedSkinIds        = deserializedData.OwnedSkins;
        DefaultSkinIds      = deserializedData.DefaultSkins;
        GraphicSkins        = new Dictionary<int, BlockSkinSO>();
        //GraphicBackgrounds  = new Dictionary<int, BackgroundSO>();

        // Fill the lookup table (dictionary) with the skins and
        // backgrounds data loaded from the Scriptable Objects.
        foreach (var skinGroup in skinGroups)
        {
            skinGroup.AddTo(GraphicSkins);
            yield return null;
        }

        // foreach (var bgGroup in backgroundGroups)
        // {
        //     bgGroup.AddTo(GraphicBackgrounds);
        //     yield return null;
        // }

        EquippedSkinIds = new Dictionary<BlockColors, int>
        {
            { BlockColors.Blue,     deserializedData.EquippedSkinId.Blue    },
            { BlockColors.Green,    deserializedData.EquippedSkinId.Green   },
            { BlockColors.Orange,   deserializedData.EquippedSkinId.Orange  },
            { BlockColors.Purple,   deserializedData.EquippedSkinId.Purple  },
            { BlockColors.Magenta,  deserializedData.EquippedSkinId.Magenta },
            { BlockColors.Yellow,   deserializedData.EquippedSkinId.Yellow  },
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
        if (!OwnedSkinIds.Contains(skinId))
            OwnedSkinIds.Add(skinId);

        yield return StartCoroutine(EquipSkin(skinId, colorGroup));
    }

    /// <summary>
    /// Purchase the background then equip it.
    /// </summary>
    /// <param name="backgroundId">The ID of the desired background</param>
    /// <param name="colorGroup">The color category where the background belongs</param>
    /// <returns>Coroutine</returns>
    // public IEnumerator TakeBackgroundOwnership(int backgroundId)
    // {
    //     if (!OwnedBackgroundIds.Contains(backgroundId))
    //         OwnedBackgroundIds.Add(backgroundId);

    //     yield return StartCoroutine(EquipBackground(backgroundId));
    // }

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
            { BlockColors.Blue    , (id) => deserializedData.EquippedSkinId.Blue    = id  },
            { BlockColors.Green   , (id) => deserializedData.EquippedSkinId.Green   = id  },
            { BlockColors.Orange  , (id) => deserializedData.EquippedSkinId.Orange  = id  },
            { BlockColors.Purple  , (id) => deserializedData.EquippedSkinId.Purple  = id  },
            { BlockColors.Magenta , (id) => deserializedData.EquippedSkinId.Magenta = id  },
            { BlockColors.Yellow  , (id) => deserializedData.EquippedSkinId.Yellow  = id  },
        };

        equipActions[colorGroup]?.Invoke(skinId);

        yield return StartCoroutine(customizationsIO.Write(deserializedData));

        // Update the In-Memory equipped skins
        EquippedSkinIds[colorGroup] = skinId;

        yield return null;
    }

    /// <summary>
    /// This will update the last equiped background id with the newer id.
    /// </summary>
    /// <param name="backgroundId">The ID of the desired background</param>
    /// <param name="colorGroup">The color category where the background belongs</param>
    /// <returns>Coroutine</returns>
    // public IEnumerator EquipBackground(int backgroundId)
    // {
    //     // Update the On-Disk equipped background
    //     deserializedData.EquippedBackgroundId = backgroundId;
    //     yield return StartCoroutine(customizationsIO.Write(deserializedData));

    //     // Update the In-Memory equipped background
    //     EquippedBackgroundId = backgroundId;

    //     yield return null;
    // }

    /// <summary>
    /// Apply block texture onto the material.
    /// </summary>
    /// <param name="blockMaterial"></param>
    public bool FillBlockWithSkin(Material blockMaterial, BlockColors colorGroup)
    {
        if (!EquippedSkinIds.ContainsKey(colorGroup))
            return false;
            
        var skinId = EquippedSkinIds[colorGroup];

        // If the currently equipped skin ID is in the list of
        // the default Ids, then we wont apply a skin texture.
        // The default color material will be used instead.
        if (colorGroup == BlockColors.None || DefaultSkinIds.Contains(skinId))
            return false;

        var skinToUse = GraphicSkins[skinId].Skin;

        blockMaterial.color = Color.white;
        
        blockMaterial.SetTexture(Constants.SHADER_MAIN_TEX, skinToUse);
        return true;
    }
}