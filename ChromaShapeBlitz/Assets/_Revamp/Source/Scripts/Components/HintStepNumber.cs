using TMPro;
using UnityEngine;

public class HintStepNumber : MonoBehaviour
{
    [SerializeField] private TextMeshPro tmpText;
    public int StepNumber { private get; set; }

    void Start()
    {
        tmpText.text = StepNumber.ToString();
    }
}
