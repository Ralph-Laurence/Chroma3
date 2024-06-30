using System.Collections.Generic;
using UnityEngine;

public class BlockSkinsAssetGroup : ScriptableObject
{
    [Header("The skin templates")]
    public List<BlockSkinsAsset> SkinGroup;
    public string GroupName;

    public void AddTo(Dictionary<int, BlockSkinsAsset> lookupTable)
    {
        foreach (var so in SkinGroup)
        {
            lookupTable.Add(so.Id, so);
        }
    }
}