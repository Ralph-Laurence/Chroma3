using Revamp;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GameManagerActionButton : MonoBehaviour
{
    public GameManagerActionEvents ActionType;
    private Button button;

    private bool isInitialized;

    void Awake()    => Initialize();
    void OnEnable() => Initialize();

    private void Initialize()
    {
        if (isInitialized)
            return;

        TryGetComponent(out button);
        button.onClick.AddListener(() => GameManagerEventNotifier.Notify(ActionType));
        
        isInitialized = true;    
    }
}
