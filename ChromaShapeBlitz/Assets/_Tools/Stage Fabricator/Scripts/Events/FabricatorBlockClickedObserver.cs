using UnityEngine;

public class FabricatorBlockClickedObserver : MonoBehaviour
{
    private StageFabricator fabricator;

    [SerializeField] private SelectedBlockMarker selectMarker;
    [SerializeField] private BlockContextMenu blockContextMenu;
    [SerializeField] private SequenceTriggerEditorContextMenu sequenceTriggerEditor;

    void Awake()
    {
        TryGetComponent(out fabricator);
    }

    void OnEnable() 
    {
        FabricatorBlockClickedNotifier.Event.AddListener(Subscribe);
        FabricatorBlockClickedNotifier.PaintEvent.AddListener(SubscribePaint);
        FabricatorBlockClickedNotifier.EraseEvent.AddListener(SubscribeErase);
    }

    void OnDisable()
    {
        FabricatorBlockClickedNotifier.Event.RemoveListener(Subscribe);
        FabricatorBlockClickedNotifier.PaintEvent.RemoveListener(SubscribePaint);
        FabricatorBlockClickedNotifier.EraseEvent.RemoveListener(SubscribeErase);
    }

    public void Subscribe(GameObject selected, int menuMode)
    {
        selected.TryGetComponent(out Block blockComponent);
        
        // Information about selected cube
        var menuData = new BlockContextMenuData
        {
            BlockComponent = blockComponent,
            BlockName = selected.name
        };

        // Selection Marker
        var selectedBlock_Pos = selected.transform.position;
        selectedBlock_Pos.y += 0.6F;

        selectMarker.transform.position = selectedBlock_Pos;

        if (!selectMarker.gameObject.activeInHierarchy)
            selectMarker.gameObject.SetActive(true);

        // Show Context Menu on left click (0)
        if (menuMode == 0)
        {
            blockContextMenu.ContextMenuData = menuData;
            blockContextMenu.Show();
        }   
        else if (menuMode == 1)
        {
            sequenceTriggerEditor.ContextMenuData = menuData;
            sequenceTriggerEditor.Show();
        }
    }

    public void SubscribePaint(GameObject blockObj)
    {
        blockObj.TryGetComponent(out Block block);

        var paintData = fabricator.GetPaintData();

        //block.SetColor(paintData.Color);
        block.SetRequiredColor(paintData.Color);
        block.ApplyMaterial(paintData.Material, paintData.Color);
    }

    public void SubscribeErase(GameObject blockObj)
    {
        blockObj.TryGetComponent(out Block block);

        if (block != null)
        {
            Destroy(block.gameObject);
            SelectedBlockMarkerNotifier.NotifyReset();
        }

        fabricator.RemoveDestinationMarker(block.gameObject);
    }
}