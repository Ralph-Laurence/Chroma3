using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TriggerPlacements
{
    NORTH = 0,
    SOUTH = 1,
    EAST = 2,
    WEST = 3,
}

public partial class  StageFabricator : MonoBehaviour
{
    private const float TRIGGER_PLACEMENT_OFFSET = 0.25F;

    private Dictionary<ColorSwatches, GameObject> fillTriggerMap;
    private Dictionary<string, GameObject> fillTriggerRefs; 

    [Space(10)] [Header("Sequence Trigger Editor")]
    [SerializeField] private Transform sequenceTriggersParent;
    [SerializeField] private Transform blockGridsParent;

    private void InitializeSequenceEditorData()
    {
        fillTriggerRefs = new Dictionary<string, GameObject>();

        fillTriggerMap = new Dictionary<ColorSwatches, GameObject>
        {
            { ColorSwatches.Blue    ,  fillTriggerObj_Blue    },
            { ColorSwatches.Green   ,  fillTriggerObj_Green   },
            { ColorSwatches.Magenta ,  fillTriggerObj_Magenta },
            { ColorSwatches.Orange  ,  fillTriggerObj_Orange  },
            { ColorSwatches.Purple  ,  fillTriggerObj_Purple  },
            { ColorSwatches.Yellow  ,  fillTriggerObj_Yellow  },
        };
    }

    private void ClearFillTriggers()
    {
        foreach(var f in fillTriggerRefs.Keys)
        {
            var obj = fillTriggerRefs[f];
            Destroy(obj);
        }

        fillTriggerRefs.Clear();
    }

    private Vector3 RepositionTriggerSpawnPlacement(TriggerPlacements placement, GameObject sender, GameObject trigger)
    {
        var extents = sender.transform.localScale;
        var fromPos = trigger.transform.localPosition;
        var pos     = new Vector3
        (
            fromPos.x,
            fromPos.y + extents.y / 2.0F,
            fromPos.z
        );

        switch (placement)
        {
            case TriggerPlacements.NORTH:
                pos.z += extents.z + TRIGGER_PLACEMENT_OFFSET;
                break;

            case TriggerPlacements.EAST:
                pos.x += extents.x + TRIGGER_PLACEMENT_OFFSET;
                break;

            case TriggerPlacements.WEST:
                pos.x -= extents.x + TRIGGER_PLACEMENT_OFFSET;
                break;

            case TriggerPlacements.SOUTH:
                pos.z -= extents.z + TRIGGER_PLACEMENT_OFFSET;
                break;
        }

        return pos;
    }

    /// <summary>
    /// <param name="triggerObj">The filler pad</param>
    /// <param name="sender">The block object that was clicked</param>
    /// </summary>
    private void AssignSequences(GameObject triggerObj, Block sender, TriggerPlacements triggerPlacement)
    {
        triggerObj.TryGetComponent(out BlockSequenceController sequenceController);
        
        if (sequenceController == null)
            return;

        Transform row;
        List<Block> cols;
        Block destBlock = null;

        switch (triggerPlacement)
        {
            //:::::::::::::: Find the block(s) coming from upper rows downwards :::::::::::::://
            case TriggerPlacements.NORTH:

                for (int i = sender.RowIndex; i < blockGridsParent.childCount; i++)
                {
                    row = blockGridsParent.transform.GetChild(i);
                    cols = GetColsInRow(row);

                    var block = cols.FirstOrDefault(b => b.ColumnIndex == sender.ColumnIndex);

                    if (block == null)
                        break;

                    sequenceController.BlockSequence.Add(block);
                    sequenceController.Destination = block;
                    destBlock = block;
                    ColorizeBlock(block, sequenceController.FillColorValue);
                }

                SetAsDestinationBlock(destBlock, sequenceController);
                break;

            //::::::::::::::  Get the rows from down upwards :::::::::::::://
            case TriggerPlacements.SOUTH:

                for (int i = sender.RowIndex; i >= 0; i--)
                {
                    row = blockGridsParent.transform.GetChild(i);
                    cols = GetColsInRow(row);

                    var block = cols.FirstOrDefault(b => b.ColumnIndex == sender.ColumnIndex);

                    if (block == null)
                        break;

                    sequenceController.BlockSequence.Add(block);
                    sequenceController.Destination = block;
                    destBlock = block;
                    ColorizeBlock(block, sequenceController.FillColorValue);
                }

                SetAsDestinationBlock(destBlock, sequenceController);
                break;

            //:::::::::::::: Get the rows from right to left :::::::::::::://

            case TriggerPlacements.EAST:

                // This is a reference to the single row
                row = blockGridsParent.transform.GetChild(sender.RowIndex);
                cols = GetColsInRow(row);

	            for (int i = sender.ColumnIndex; i >= 0; i--)
	            {
	                var block = cols.FirstOrDefault(b => b.ColumnIndex == i);

	                if (block == null)
                        break;

	                sequenceController.BlockSequence.Add(block);
                    sequenceController.Destination = block;
                    destBlock = block;
                    ColorizeBlock(block, sequenceController.FillColorValue);
	            }

                SetAsDestinationBlock(destBlock, sequenceController);
                break;

            //::::::::::::::  Get the rows from left to right :::::::::::::://
            case TriggerPlacements.WEST:

                // This is a reference to the single row
                row = blockGridsParent.transform.GetChild(sender.RowIndex);
                cols = GetColsInRow(row);

                for (int i = sender.ColumnIndex; i <= cols.Count; i++)
                {
                    var block = cols.FirstOrDefault(b => b.ColumnIndex == i);

                    if (block == null)
                        break;

                    sequenceController.BlockSequence.Add(block);
                    sequenceController.Destination = block;
                    destBlock = block;
                    ColorizeBlock(block, sequenceController.FillColorValue);
                }

                if (destBlock == null)
                {
                    sequenceController.BlockSequence.Add(sender);
                    sequenceController.Destination = sender;
                    destBlock = sender;
                    ColorizeBlock(sender, sequenceController.FillColorValue);
                }
                
                SetAsDestinationBlock(destBlock, sequenceController);
                break;
        }

        sequenceController.IsEditMode = false;
    }

    private void SetAsDestinationBlock(Block block, BlockSequenceController sequenceController)
    {
        if (block == null)
            block = sequenceController.BlockSequence.LastOrDefault();

        block.IsDestinationBlock = true;
        sequenceController.Destination = block;
        AddDestinationMarker(block);
    }

    private List<Block> GetColsInRow(Transform row)
    {
        var cols = new List<Block>();

        foreach (Transform obj in row)
        {
            obj.TryGetComponent(out Block block);
            cols.Add(block);
        }

        return cols;
    }

    public void AddFillTrigger(TriggerPlacements placement, Block sender, ColorSwatches fillColor)
    {
        var triggerName = $"{sender.gameObject.name}_{placement}";

        if (fillTriggerRefs.ContainsKey(triggerName))
            return;

        // The sender object is what was clicked to trigger the
        // sequence editor menu, which is the Block
        var senderObj = sender.gameObject;

        var triggerObj = Instantiate
        (
            fillTriggerMap[fillColor], 
            senderObj.transform.position, 
            senderObj.transform.rotation,
            sequenceTriggersParent
        );
        var spawnPos = RepositionTriggerSpawnPlacement(placement, senderObj, triggerObj);// GetTriggerSpawnPlacement(placement, senderObj, triggerObj);

        triggerObj.name = triggerName;
        triggerObj.transform.localPosition = spawnPos;
        //triggerObj.transform.SetParent(sequenceTriggersParent, false);
        //triggerObj.transform.SetPositionAndRotation(spawnPos, senderObj.transform.rotation);
        
        AssignSequences(triggerObj, sender,placement);
        fillTriggerRefs.Add(triggerName, triggerObj);
    }

    public void RemoveFillTrigger(string triggerName)
    {
        if (!fillTriggerRefs.ContainsKey(triggerName))
            return;

        var target = fillTriggerRefs[triggerName];
        Destroy(target);

        fillTriggerRefs.Remove(triggerName);
    }

    private void ColorizeBlock(Block block, ColorSwatches fillColor)
    {
        var mat = GetColorMap()[fillColor];

        block.SetRequiredColor(fillColor);
        block.ApplyMaterial(mat, fillColor);
    }
}