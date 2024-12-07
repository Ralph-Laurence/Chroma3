using UnityEngine;
using TMPro;

[RequireComponent(typeof(LoginManager))]
public class ProfileController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputUsername;
    [SerializeField] private TMP_InputField inputPassword;

    private LoginManager loginManager;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        TryGetComponent(out loginManager);
    }

    public void Ev_Login()
    {
        string username = inputUsername.text; // Get this from a UI input field
        string password = inputPassword.text; // Get this from a UI input field

        loginManager.Login(username, password);
    }
}
