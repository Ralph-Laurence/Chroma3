using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block Skin Group", menuName = "Scriptable Objects/Block Skin Group")]
public class BlockSkinGroupSO : ScriptableObject
{
    [Header("The skin templates")]
    [SerializeField] private BlockSkinSO[] skinGroup;
    public string GroupName;

    public void AddTo(Dictionary<int, BlockSkinSO> lookupTable)
    {
        foreach (var so in skinGroup)
        {
            lookupTable.Add(so.Id, so);
        }
    }
}