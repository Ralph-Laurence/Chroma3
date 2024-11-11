using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachinePrizeCard : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI amountLabel;

    public Sprite Icon { get; set; }
    public int Amount { get; set; }

    void Start()
    {
        icon.sprite = Icon;
        amountLabel.text = $"<size=130%>×</size><voffset=.15rem>{Amount}";
    }
}
