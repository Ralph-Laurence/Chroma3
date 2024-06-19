using UnityEngine;

public class FabricatorTriggerClickedObserver : MonoBehaviour
{
    [SerializeField] private StageFabricator fabricatorController;
    [SerializeField] private GameObject triggerActionsMenuOverlay;
    [SerializeField] private GameObject triggerActionsMenuDialog;
    [SerializeField] private SelectedTriggerMarker selectMarker;
    [SerializeField] private GameObject sequenceTracer;
    [SerializeField] private Transform sequenceTracerParent;

    private BlockSequenceController _sender;

    void OnEnable() => FabricatorTriggerClickedNotifier.Event.AddListener(Subscribe);
    void OnDisable() => FabricatorTriggerClickedNotifier.Event.RemoveListener(Subscribe);

    public void Subscribe(BlockSequenceController sender)
    {
        _sender = sender;

        var marker = selectMarker.gameObject;
        var filler = sender.transform;

        marker.transform.SetPositionAndRotation
        (
            new Vector3
            (
                filler.position.x,
                filler.position.y + 0.01F,
                filler.position.z
            ),
            filler.rotation
        );
        
        marker.SetActive(true);

        triggerActionsMenuOverlay.SetActive(true);
        triggerActionsMenuDialog.SetActive(true);
    }

    public void Ev_RemoveTrigger()
    {
        fabricatorController.RemoveFillTrigger(_sender.gameObject.name);

        // Get the block where the destination marker is positioned
        var triggerDestMarker = _sender.Destination.gameObject;

        if (triggerDestMarker != null)
            fabricatorController.RemoveDestinationMarker(triggerDestMarker);

        Close();
    }

    public void Ev_TraceSequence()
    {
        ClearTracers();

        if (_sender == null)
            return;

        var i = 1;

        foreach (var seq in _sender.BlockSequence)
        {
            var block = seq.gameObject;
            var spawnPos = new Vector3
            (
                block.transform.position.x,
                block.transform.position.y + (block.transform.localScale.y / 2.0F) + 0.12F,
                block.transform.position.z
            );

            var tracerObj = Instantiate(sequenceTracer, spawnPos, block.transform.rotation);
            tracerObj.TryGetComponent(out SequenceTracer tracer);
            tracerObj.transform.SetParent(sequenceTracerParent);

            tracer.Number = i;
            i++;
        }
    }

    public void Ev_Close() => Close();

    private void ClearTracers()
    {
        while (sequenceTracerParent.childCount > 0)
        {
            var child = sequenceTracerParent.GetChild(0);
            child.SetParent(null);
            
            Destroy(child.gameObject);
        }
    }

    private void Close()
    {
        ClearTracers();
        
        triggerActionsMenuOverlay.SetActive(false);
        triggerActionsMenuDialog.SetActive(false);
        selectMarker.gameObject.SetActive(false);
        _sender = null;
    }
}