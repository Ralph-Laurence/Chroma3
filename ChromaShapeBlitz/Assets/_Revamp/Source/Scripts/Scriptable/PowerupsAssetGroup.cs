using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerupsAssetGroup", menuName = "Scriptable Objects/PowerupsAssetGroup")]
public class PowerupsAssetGroup : ScriptableObject
{
    public List<PowerupsAsset> Powerups;
    private Dictionary<int, PowerupsAsset> powerupsLookup;

    /// <summary>
    /// Select all item thumbnails where an item id is in the given List,
    /// this is primarily used in HotBar
    /// </summary>
    /// <param name="ids">Matching Ids</param>
    /// <returns>Item thumbnails of selected ids</returns>
    public Dictionary<int, Sprite> LoadItemThumbnailsWhereIdIn(List<int> ids)
    {
        var collection = new Dictionary<int, Sprite>();

        foreach(var item in Powerups)
        {
            if (!ids.Contains(item.Id))
                continue;

            collection.Add(item.Id, item.PreviewImage);
        }

        return collection;
    }

    public void AddToLookup(Dictionary<int, PowerupsAsset> lookupTable)
    {
        foreach (var so in Powerups)
        {
            lookupTable.Add(so.Id, so);
        }
    }

    public List<PowerupItemData> Select(List<int> powerupIds)
    {
        InnitLookUpSet();

        var selectedPowerups = new List<PowerupItemData>();

        foreach (int id in powerupIds)
        {
            //Debug.LogWarning($"Dict contains id -> {id}");
            //Debug.LogWarning(powerupsLookup.ContainsKey(id));

            if (powerupsLookup.TryGetValue(id, out PowerupsAsset powerup))
            {
                selectedPowerups.Add(new PowerupItemData
                {
                    Id = powerup.Id,
                    PreviewImage = powerup.PreviewImage,
                });
            }
        }

        return selectedPowerups;
    }

    private void InnitLookUpSet()
    {
        Debug.Log("Innit Lookup set");
        if (powerupsLookup != null)
            return;

        powerupsLookup = new();

        foreach (var p in Powerups)
            powerupsLookup.Add(p.Id, p);
    }
}
