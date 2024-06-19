using System.Collections.Generic;
using UnityEngine;

public partial class StageFabricator : MonoBehaviour
{
    private Dictionary<string, GameObject> destinationMarkers = new Dictionary<string, GameObject>();

    [Space(10)] 
    [SerializeField] private GameObject destinationMarkerPrefab;
    [SerializeField] private Transform destinationMarkersContainer;

    public void RemoveDestinationMarker(GameObject selectedBlock)
    {
        if (destinationMarkers.ContainsKey(selectedBlock.name))
        {
            Destroy(destinationMarkers[selectedBlock.name]);
            destinationMarkers.Remove(selectedBlock.name);
        }
    }

    public void ResetDestinationMarkers() => destinationMarkers?.Clear();
    public void AddDestinationMarker(Block block)
    {
        if (destinationMarkers.ContainsKey(block.gameObject.name))
            return;
            
        var spawnPos = block.transform.position;
        spawnPos.y += (block.transform.localScale.y / 2.0F) + 0.025F;
        var marker = Instantiate
            (
                destinationMarkerPrefab,
                spawnPos,
                Quaternion.Euler(0.0F, block.transform.rotation.eulerAngles.y, 0.0F),
                destinationMarkersContainer
            );

        destinationMarkers.Add(block.gameObject.name, marker);
    }
    public bool DestinationMarkerExists(string markerName) => destinationMarkers.ContainsKey(markerName);
}
