using UnityEngine;

public class GenericProgressLoader : MonoBehaviour
{
    [SerializeField] private GameObject overlay;
    [SerializeField] private GameObject round4Segment;
    [SerializeField] private GameObject barLoader;

    void OnDisable()
    {
        ProgressLoaderNotifier.UnbindObserver(ObserveProgressNotified);
    }

    void OnEnable()
    {
        ProgressLoaderNotifier.BindObserver(ObserveProgressNotified);
    }

    private void ObserveProgressNotified(ProgressLoaderTypes progressType, ProgressLoaderActions action)
    {
        overlay.SetActive(false);
        round4Segment.SetActive(false);
        barLoader.SetActive(false);

        var widget = progressType switch
        {
            ProgressLoaderTypes.FourSegmentRound => round4Segment,
            ProgressLoaderTypes.IndefiniteBar    => barLoader,
            _ => barLoader
        };

        switch (action)
        {
            case ProgressLoaderActions.Begin:

                overlay.SetActive(true);
                widget.SetActive(true);
                break;

            case ProgressLoaderActions.End:

                overlay.SetActive(false);
                widget.SetActive(false);
                break;
        }
    }
}
