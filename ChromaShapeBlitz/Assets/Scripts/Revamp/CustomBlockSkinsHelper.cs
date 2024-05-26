using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBlockSkinsHelper : MonoBehaviour
{
    /* #region SINGLETON */
    //========================================================
    //
    private static CustomBlockSkinsHelper instance;

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

    public static CustomBlockSkinsHelper Instance => instance;
    //
    //========================================================
    /* #endregion */

    /**************************************************************/
    /******************* BLOCK SKIN PROPERTIES ********************/
    /**************************************************************/

    // The templates for building the Visual Skins.
    [SerializeField] private Rv_BlockSkinGroupSO[] skinGroups;

    // These are the skin details we can use in shop or when
    // filling the blocks. Every skin details such as Id, 
    // Name and Price are stored here.
    public Dictionary<int, Rv_BlockSkinSO> SkinObjects { get; private set; }
    public Dictionary<ColorSwatches, int> SkinIdsInUse { get; private set; }

    // This is responsible for loading and saving data into Binary Format.
    private BlockSkinsIO io;

    // When we load data from Binary, we store them here.
    private CustomBlockSkins deserializedData;

    // Owned Skins are the ids of skins we currently have.
    // These are the ids of all Purchased skins and also
    // includes the Default skins ids
    public List<int> OwnedSkinIds { get; private set; }

    /// <summary>
    /// Call this in the EntryPoint script
    /// </summary>
    /// <returns>Coroutine</returns>
    public IEnumerator LoadCustomSkins()
    {
        // Get a reference to the Singleton instance of the ThemeIO.
        // This handles the saving and loading of binary theme data.
        io = BlockSkinsIO.Instance;

        // When a skin file data isnt created yet, we create a fresh binary data
        if (!io.FileExists)
        {
            deserializedData = CreateDefaultData();

            yield return StartCoroutine(io.Write(deserializedData));
        }

        // Otherwise, we load the existing data from the binary file
        else
        {
            yield return StartCoroutine(io.Read((t) => deserializedData = t));
        }

        // The Post Initialize method is used for
        // initialization of Non-Binary related data.
        yield return StartCoroutine(PostInitialize());

        yield return null;
    }

    /// <summary>
    /// Create a fresh theme data, with the skin ids set to default id
    /// </summary>
    /// <returns>Default theme data</returns>
    private CustomBlockSkins CreateDefaultData()
    {
        var custom = new CustomBlockSkins
        {
            InUse = new SkinIdsInUse
            {
                Blue    = 1,
                Green   = 2,
                Magenta = 3,
                Orange  = 4,
                Purple  = 5,
                Yellow  = 6
            }
        };

        custom.Owned = custom.DefaultSkins;

        return custom;
    }

    private IEnumerator PostInitialize()
    {
        OwnedSkinIds = deserializedData.Owned;
        SkinObjects  = new Dictionary<int, Rv_BlockSkinSO>();

        // Fill the lookup table (dictionary) with the 
        // skins data loaded from the Scriptable Objects.
        foreach (var skinGroup in skinGroups)
        {
            skinGroup.AddTo(SkinObjects);
            yield return null;
        }

        SkinIdsInUse = new Dictionary<ColorSwatches, int>
        {
            { ColorSwatches.Blue,     deserializedData.InUse.Blue    },
            { ColorSwatches.Green,    deserializedData.InUse.Green   },
            { ColorSwatches.Orange,   deserializedData.InUse.Orange  },
            { ColorSwatches.Purple,   deserializedData.InUse.Purple  },
            { ColorSwatches.Magenta,  deserializedData.InUse.Magenta },
            { ColorSwatches.Yellow,   deserializedData.InUse.Yellow  },
        };

        // Debug.LogWarning($"post init len ({SkinIdsInUse.Count})");
        // foreach (var kvp in SkinIdsInUse)
        // {
        //     Debug.Log($"K -> {kvp.Key} V -> {kvp.Value}");
        // }
    }

    public IEnumerator UseSkin(ColorSwatches color, int skinId)
    {
        // Update the In-Disk values

        if (!deserializedData.Owned.Contains(skinId))
            deserializedData.Owned.Add(skinId);

        var updateSkinInUse = new Dictionary<ColorSwatches, Action>
        {
            { ColorSwatches.Blue,     () => deserializedData.InUse.Blue    = skinId },
            { ColorSwatches.Green,    () => deserializedData.InUse.Green   = skinId },
            { ColorSwatches.Orange,   () => deserializedData.InUse.Orange  = skinId },
            { ColorSwatches.Purple,   () => deserializedData.InUse.Purple  = skinId },
            { ColorSwatches.Magenta,  () => deserializedData.InUse.Magenta = skinId },
            { ColorSwatches.Yellow,   () => deserializedData.InUse.Yellow  = skinId },
        };

        updateSkinInUse[color].Invoke();
        
        // Update the In-Memory value
        SkinIdsInUse[color] = skinId;

        yield return StartCoroutine(io.Write(deserializedData));
    }

    public List<int> DefaultSkins => deserializedData.DefaultSkins;
}