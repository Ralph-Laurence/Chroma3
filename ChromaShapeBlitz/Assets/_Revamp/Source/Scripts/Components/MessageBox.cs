using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    [SerializeField] private GameObject overlay;
    [SerializeField] private GameObject dialogRoot;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Image messageBoxIcon;

    [SerializeField] private Button okButton;

    [Space(10)]
    [SerializeField] private Sprite errorIcon;
    [SerializeField] private Sprite successIcon;

    public string successTitleColor = "#00AC78";
    public string errorTitleColor   = "#F02137";

    public void ShowSuccess(string message, string title = "Success!", UnityAction onOKButtonClicked = null)
    {
        messageBoxIcon.sprite = successIcon;
        messageText.text      = message;
        titleText.text        = $"<color={successTitleColor}>{title}</color>";

        Show(onOKButtonClicked);
    }

    public void ShowError(string message, string title = "Failure!", UnityAction onOKButtonClicked = null)
    {
        messageBoxIcon.sprite = errorIcon;
        messageText.text      = message;
        titleText.text        = $"<color={errorTitleColor}>{title}</color>";

        Show(onOKButtonClicked);
    }

    private void Show(UnityAction onOKButtonClicked = null)
    {
        overlay.SetActive(true);
        dialogRoot.SetActive(true);

        if (okButton.TryGetComponent(out UXButton uXButton))
            Destroy(uXButton);

        okButton.onClick.RemoveAllListeners();
        okButton.gameObject.AddComponent(typeof(UXButton));

        if (onOKButtonClicked != null )
            okButton.onClick.AddListener(onOKButtonClicked);

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
