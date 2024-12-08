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

    /// <summary>
    /// Prevent the button from recieving clicks
    /// </summary>
    public void DisableClicks() => ButtonComponent.interactable = false;

    /// <summary>
    /// Allow the button to recieve clicks
    /// </summary>
    public void EnableClicks() => ButtonComponent.interactable = true;
}