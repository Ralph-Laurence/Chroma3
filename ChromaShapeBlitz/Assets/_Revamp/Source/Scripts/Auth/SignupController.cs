using TMPro;
using UnityEngine;
using static LoginController;
using UnityEngine.Networking;
using System.Collections;

public class SignupController : MonoBehaviour
{
    [SerializeField] private MessageBox messageBox;
    [SerializeField] private GameObject authFormsContainer;
    [SerializeField] private TMP_InputField inputUsername;
    [SerializeField] private TMP_InputField inputPassword;
    [SerializeField] private TMP_InputField inputRePassword;
    [SerializeField] private ProfileController profileController;

    // URL of the signup endpoint
    private readonly string signupUrl = Constants.Url.RegisterRoute;

    private GameSessionManager gsm;

    private void Awake()
    {
        gsm = GameSessionManager.Instance;
    }

    // Method to perform signup
    public void Signup()
    {
        var username  = inputUsername.text;
        var password1 = inputPassword.text;
        var password2 = inputRePassword.text;

        if (string.IsNullOrEmpty(username))
        {
            messageBox.ShowError("Please enter your username.", "Oops!", () =>
            {
                inputUsername.ActivateInputField();
            });

            return;
        }

        if (string.IsNullOrEmpty(password1))
        {
            messageBox.ShowError("Please enter your password.", "Oops!", () =>
            {
                inputPassword.ActivateInputField();
            });

            return;
        }

        if (username.Length < 4)
        {
            messageBox.ShowError("Your username must be atleast 4 characters long.", "Too short!", () =>
            {
                inputUsername.ActivateInputField();
            });

            return;
        }

        if (password1.Length < 4)
        {
            messageBox.ShowError("Password must be atleast 4 characters long.", "Too short!", () =>
            {
                inputPassword.ActivateInputField();
            });

            return;
        }

        if (string.IsNullOrEmpty(password2) || !password1.Equals(password2))
        {
            messageBox.ShowError("Please re-enter your password.", "Passwords Mismatch", () =>
            {
                inputRePassword.ActivateInputField();
            });

            return;
        }

        StartCoroutine(SignupCoroutine(username, password1));
    }

    // Coroutine to handle the login request
    private IEnumerator SignupCoroutine(string username, string password)
    {
        // Show the progress bar
        ProgressLoaderNotifier.NotifyFourSegment(true);

        // Create the login data object. This can also be used with signup
        var loginData = new LoginData { username = username, password = password };

        // Convert the login data object to JSON
        string jsonData = JsonUtility.ToJson(loginData);
        byte[] bodyRaw  = new System.Text.UTF8Encoding().GetBytes(jsonData);

        // Create a new UnityWebRequest for the POST request,
        // Then attach the JSON data to the request
        var www = new UnityWebRequest(signupUrl)
        {
            method = "POST",
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
            Debug.LogError("Signup failed: " + www.error);
            HandleFailed(www);
        }
        else
        {
            // Handle the response
            Debug.Log("Signup successful: " + www.downloadHandler.text);
            HandleSuccess(www);
        }
    }

    private void HandleSuccess(UnityWebRequest www)
    {
        // Parse the JSON response
        var responseData = JsonUtility.FromJson<ServerAuthResponseData>(www.downloadHandler.text);
        
        PlayerAuthManager.Instance.RememberUser(responseData);
        gsm.AuthData = responseData;

        profileController.CheckAuth(responseData);
        // authFormsContainer.SetActive(false);
        
        // Clear the input fields upon signup success
        inputUsername.text = string.Empty;
        inputPassword.text = string.Empty;
        inputRePassword.text = string.Empty;

        var msg = "Syncing safeguards your data, allowing you to continue from where you left off on any device.";
        messageBox.ShowSuccess(msg, "Logged In");
    }

    private void HandleFailed(UnityWebRequest www)
    {
        var statusCode = www.responseCode;
        string errMsg = "There was a problem while creating your account. Please try again later.";

        switch (statusCode)
        {
            case 0:
                errMsg = "Service is unavailable. Please try again later.";
                break;

            case 502:
                errMsg = "Unable to connect to the server. Please check your internet connection and try again.";
                break;

            case 400:
                errMsg = "Username is taken. Please choose another.";
                break;
        }

        messageBox.ShowError(errMsg, "Signup Failed!");
    }
}
