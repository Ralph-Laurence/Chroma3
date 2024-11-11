using UnityEngine;
using UnityEngine.Events;

public class Interactive3DTutorial : MonoBehaviour
{
    [SerializeField] private UnityEvent onExecute;

    public void Execute()
    {
        onExecute.Invoke();
    }
}
