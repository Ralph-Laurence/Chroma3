using UnityEngine;

public class GameOverDialogObserver : MonoBehaviour
{
    [SerializeField] private GameOverScreenSuccess successDialog;
    [SerializeField] private GameOverScreenFailed failedDialog;

    void OnEnable()
    {
        TutorialEventNotifier.BindObserver(ObserveDialogEvent);
    }

    void OnDisable()
    {
        TutorialEventNotifier.UnbindObserver(ObserveDialogEvent);
    }

    private void ObserveDialogEvent(string key, object data)
    {
        switch (key)
        {
            case TutorialEventNames.GameOverSuccess:
                successDialog.SetGameOverType(GameOverTypes.Success);
                successDialog.gameObject.SetActive(true);
                break;

            case TutorialEventNames.GameOverFailed:
                failedDialog.SetGameOverType(GameOverTypes.Fail);
                failedDialog.gameObject.SetActive(true);
                break;
        }
    }
}
