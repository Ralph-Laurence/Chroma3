using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class LevelStructuresIO : BinarySerializer<LevelStructures>
{
    //============================================
    // Begin Singleton
    //============================================
    private static LevelStructuresIO instance;

    private LevelStructuresIO() { }

    public static LevelStructuresIO Instance
    {
        get
        {
            if (instance == null)
                instance = new LevelStructuresIO();

            return instance;
        }
    }

    //...........................................
    // End Singleton
    //...........................................

    protected override string BinaryPath => Constants.DataPaths.LevelStructure;

    /// <summary>
    /// Read the json data, then convert it to LevelStructures object.
    /// </summary>
    /// <returns>LevelStructures</returns>
    public IEnumerator<LevelStructures> ReadFromTemplate(TextAsset templateAsset, Action<LevelStructures> callback)
    {
        var data = JsonUtility.FromJson<LevelStructures>(templateAsset.text);

        callback?.Invoke(data);

        yield return null;
    }
}
