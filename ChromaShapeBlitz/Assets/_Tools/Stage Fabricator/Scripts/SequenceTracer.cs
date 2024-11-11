using TMPro;
using UnityEngine;

public class SequenceTracer : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    public int Number {get;set;}

    void Start()
    {
        textMesh.text = Number.ToString();
    }
}
