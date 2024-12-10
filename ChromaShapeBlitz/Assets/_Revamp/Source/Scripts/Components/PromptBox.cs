using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PromptBox : MonoBehaviour
{
    [SerializeField] private GameObject overlay;
    [SerializeField] private GameObject dialogRoot;
    [SerializeField] private TextMeshProUGUI messageText;

    [SerializeField] private Button yesButton;

    public void Show(string message, UnityAction onYesButtonClicked = null)
    {
        messageText.text = message;
        overlay.SetActive(true);
        dialogRoot.SetActive(true);

        if (yesButton.TryGetComponent(out UXButton uXButton))
            Destroy(uXButton);

        yesButton.onClick.RemoveAllListeners();
        yesButton.gameObject.AddComponent(typeof(UXButton));

        if (onYesButtonClicked != null)
            yesButton.onClick.AddListener(onYesButtonClicked);

        LeanTween.scale(dialogRoot, Vector3.one, 0.25F);
    }

    public void Close()
    {
        LeanTween.scale(dialogRoot, Vector3.forward, 0.25F)
                 .setOnComplete(() =>
                 {
                     dialogRoot.SetActive(false);
                     overlay.SetActive(false);
                 });
    }
}
