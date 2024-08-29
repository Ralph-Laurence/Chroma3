using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerupsAssetGroup", menuName = "Scriptable Objects/PowerupsAssetGroup")]
public class PowerupsAssetGroup : ScriptableObject
{
    public List<PowerupsAsset> Powerups;
    public void AddToLookup(Dictionary<int, PowerupsAsset> lookupTable)
    {
        foreach (var so in Powerups)
        {
            lookupTable.Add(so.Id, so);
        }
    }
}
