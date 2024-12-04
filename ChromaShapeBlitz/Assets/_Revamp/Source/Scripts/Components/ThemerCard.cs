using UnityEngine;
using UnityEngine.UI;

public class ThemerCard : MonoBehaviour
{
    public MainMenuThemeIdentifier themeIdentifier;
    private Button m_button;

    void Awake() => TryGetComponent(out m_button);

    public void Lock() => m_button.interactable = false;
}
