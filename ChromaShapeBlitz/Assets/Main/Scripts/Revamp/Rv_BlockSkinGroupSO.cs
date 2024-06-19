using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block Skin Group", menuName = "Scriptable Objects/RV Block Skin Group")]
public class Rv_BlockSkinGroupSO : ScriptableObject
{
    [Header("The skin templates")]
    public List<Rv_BlockSkinSO> SkinGroup;
    public string GroupName;

    public void AddTo(Dictionary<int, Rv_BlockSkinSO> lookupTable)
    {
        foreach (var so in SkinGroup)
        {
            lookupTable.Add(so.SkinInfo.Id, so);
        }
    }
}