using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct BuyPowerupResult
{
    public string DialogMessage;
    public Sprite ItemIcon;
}

public abstract class BuyItemResultDialog : MonoBehaviour
{
    
    [SerializeField] private GameObject dialogContent;
    [SerializeField] private Button[] dialogCloseButtons;
    [SerializeField] private GameObject btnSuccess;
    [SerializeField] private GameObject btnFail;
    [SerializeField] private TextMeshProUGUI messageLabel;
    [SerializeField] private Image itemThumbnail;
    
    [SerializeField] private AudioClip  sfxShowSuccessResult;
    [SerializeField] private AudioClip  sfxShowFailResult;

    private RectTransform dialogContentRect;
    private Image m_overlay;

    private readonly string SUCCESS_TEXT_COLOR = "#7DFF00";
    private readonly string FAILURE_TEXT_COLOR = "#EC4531";
    private SoundEffects sfx;

    void Awake()
    {
        TryGetComponent(out m_overlay);

        sfx = SoundEffects.Instance;

        dialogContent.TryGetComponent(out dialogContentRect);

        foreach (var btn in dialogCloseButtons)
        {
            btn.onClick.AddListener(CloseDialog);
        }
    }

    public void ShowFailResult(string message, Sprite itemIcon)
    {
        messageLabel.text = $"<size=120%><color={FAILURE_TEXT_COLOR}>Purchase Failed.</color></size>\n\n{message}";
        itemThumbnail.sprite = itemIcon;
        
        sfx.PlayOnce(sfxShowFailResult);
        
        btnFail.SetActive(true);
        ShowDialog();
    }

    public void ShowSuccessResult(Sprite itemIcon, string message)
    {
        messageLabel.text = $"<size=120%><color={SUCCESS_TEXT_COLOR}>Purchase Successful!</color></size>\n\n{message}";
        itemThumbnail.sprite = itemIcon;

        sfx.PlayOnce(sfxShowSuccessResult);
        
        btnSuccess.SetActive(true);
        ShowDialog();
    }

    private void CloseDialog()
    {
        LeanTween.scale(dialogContentRect, Vector3.zero, 0.25F)
                 .setOnComplete(() => {
                    dialogContent.SetActive(false);
                    m_overlay.enabled = false;
                    btnFail.SetActive(false);
                    btnSuccess.SetActive(false);

                    OnDialogHidden();
                 });
    }

    private void ShowDialog()
    {
        m_overlay.enabled = true;
        
        dialogContent.SetActive(true);
        LeanTween.scale(dialogContentRect, Vector3.one, 0.25F)
                 .setOnComplete(() => OnDialogShown());
    }

    protected abstract void OnDialogShown();
    protected abstract void OnDialogHidden();
}