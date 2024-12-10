using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public partial class LoginController : MonoBehaviour
{
    [SerializeField] private ProfileController profileController;

    [SerializeField] private MessageBox     messageBox;
    [SerializeField] private TMP_InputField inputUsername;
    [SerializeField] private TMP_InputField inputPassword;

    // URL of the login endpoint
    private readonly string loginUrl = Constants.Url.LoginRoute;

    private GameSessionManager gsm;

    private void Awake()
    {
        gsm = GameSessionManager.Instance;
    }

    // Method to perform login
    public void Login() // (string username, string password)
    {
        var username = inputUsername.text;
        var password = inputPassword.text;

        if (string.IsNullOrEmpty(username))
        {
            messageBox.ShowError("Please enter your username.", "Oops!", () =>
            {
                inputUsername.ActivateInputField();
            });

            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            messageBox.ShowError("Please enter your password.", "Oops!", () =>
            {
                inputPassword.ActivateInputField();
            });

            return;
        }

        StartCoroutine(LoginCoroutine(username, password));
    }

    // Coroutine to handle the login request
    private IEnumerator LoginCoroutine(string username, string password)
    {
        // Show the progress bar
        ProgressLoaderNotifier.NotifyFourSegment(true);

        // Create the login data object
        var loginData = new LoginData { username = username, password = password };

        // Convert the login data object to JSON
        string jsonData = JsonUtility.ToJson(loginData);
        byte[] bodyRaw  = new System.Text.UTF8Encoding().GetBytes(jsonData);

        // Create a new UnityWebRequest for the POST request,
        // Then attach the JSON data to the request
        var www = new UnityWebRequest(loginUrl)
        { 
            method          = "POST",
            uploadHandler   = new UploadHandlerRaw(bodyRaw),
            downloadHandler = new DownloadHandlerBuffer()
        };
        
        www.SetRequestHeader("Content-Type", "application/json");

        Debug.LogWarning($"{www.responseCode}--{www.downloadHandler.text}");

        // Send the request and wait for a response
        yield return www.SendWebRequest();

        // Hide the progress bar
        ProgressLoaderNotifier.NotifyFourSegment(false);

        // Check for errors
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Login failed: " + www.error);
            HandleFailed(www);
        }
        else
        {
            // Handle the response
            Debug.Log("Login successful: " + www.downloadHandler.text);
            HandleSuccess(www);
        }
    }

    private void HandleSuccess(UnityWebRequest www)
    {
        // Parse the JSON response
        var responseData = JsonUtility.FromJson<ServerAuthResponseData>(www.downloadHandler.text);

        PlayerAuthManager.Instance.RememberUser(responseData);
        gsm.AuthData = responseData;

        profileController.CheckAuth(gsm.AuthData);

        // Clear the input fields upon login success
        inputUsername.text = string.Empty;
        inputPassword.text = string.Empty;

        var msg = "Syncing safeguards your data, allowing you to continue from where you left off on any device.";
        messageBox.ShowSuccess(msg, "Logged In", () =>
        {
            // Upon signin, download the saved progress
            profileController.DownloadProgress
            (
                "Would you like to load your progress from the cloud backup?\n\nYour local save progress will be overwritten."
            );
        });
    }

    private void HandleFailed(UnityWebRequest www)
    {
        var statusCode = www.responseCode;
        string errMsg  = "There was a problem while logging you in. Please try again later.";

        switch (statusCode)
        {
            case 0:
                errMsg = "Service is unavailable. Please try again later.";
                break;

            case 502:
                errMsg = "Unable to connect to the server. Please check your internet connection and try again.";
                break;

            case 401:
            case 404:
                errMsg = "Invalid username or password. Please try again or create a new account if you don't have one.";
                break;
        }

        messageBox.ShowError(errMsg, "Login Failed!");
    }
}
