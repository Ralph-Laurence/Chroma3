using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GameOverScreenButton : MonoBehaviour
{
    public GameManagerActionEvents Action;
    public Button ButtonComponent
    {
        get
        {
            TryGetComponent(out Button button);
            return button;
        }
    }
}