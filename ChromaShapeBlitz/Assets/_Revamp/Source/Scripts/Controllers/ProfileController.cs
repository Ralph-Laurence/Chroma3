using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ProfileController : MonoBehaviour
{
    [SerializeField] private AudioClip bgmAbout;
    [SerializeField] private GameObject fancySceneLoader;

    [SerializeField] private GameObject cloudSyncCanvas;
    [SerializeField] private GameObject authCanvas;
    [SerializeField] private GameObject loginBox;

    [SerializeField] private MessageBox msgBox;
    [SerializeField] private PromptBox promptBox;

    [SerializeField] private TextMeshProUGUI usernameText;

    private BackgroundMusic bgm;
    private PlayerAuthManager authMan;
    private GameSessionManager gsm;

    private readonly string UploadEndpoint   = Constants.Url.UploadEndpoint;
    private readonly string DownloadEndpoint = Constants.Url.DownloadEndpoint;
    private readonly string DownloadFileName = "user.download.sav";
    
    private string DownloadPath;

    void Awake()
    {
        bgm = BackgroundMusic.Instance;
        gsm = GameSessionManager.Instance;

        authMan = PlayerAuthManager.Instance;

        DownloadPath = Path.Combine(Application.persistentDataPath, DownloadFileName);
    }

    void Start()
    {
        // Check if the user has signed in before
        CheckAuth(gsm.AuthData);

        if (bgm == null)
            return;

        bgm.SetClip(bgmAbout);
        bgm.Play();
    }

    public void GoBack()
    {
        Instantiate(fancySceneLoader).TryGetComponent(out FancySceneLoader loader);
        loader.LoadScene(Constants.Scenes.MainMenu);
    }

    public bool CheckAuth(PlayerAuthData authData)
    {
        // Check auth data
        if (authData == null)
        {
            Debug.LogWarning("Auth data doesn't exist");
            ShowLoginBox();

            return false;
        }

        // Make sure we have the required auth data
        if (!authMan.IsComplete(authData))
        {
            Debug.LogWarning("Need to re-login");
            ShowLoginBox();

            return false;
        }

        Debug.Log("Is logged on");
        HideLoginBox();

        // Assume we have loaded the user auth data
        usernameText.text = authData.Username;

        return true;
    }

    private void ShowLoginBox()
    {
        Debug.Log("Show login box");
        // Hide the canvas where the "save, load and signout" are placed
        cloudSyncCanvas.SetActive(false);

        // SHow the login box
        authCanvas.SetActive(true);
        loginBox.SetActive(true);
    }

    private void HideLoginBox()
    {
        // Show the canvas where the "save, load and signout" are placed
        cloudSyncCanvas.SetActive(true);

        // Hide the login box
        authCanvas.SetActive(false);
        loginBox.SetActive(false);
    }

    public void UploadProgress()
    {
        // Check if the user has signed in before upload
        if (CheckAuth(gsm.AuthData))
        {
            var msg = "Uploading your data will overwrite the saved progress on the cloud.\n\nContinue?";

            // Show confirmation box before initiating the upload
            promptBox.Show(msg, onYesButtonClicked: () =>
            {
                StartCoroutine(IEUploadGameProgress(gsm.AuthData));
            });
        }
    }

    public void DownloadProgress(string msg = default)
    {
        if (string.IsNullOrEmpty(msg))
            msg = "Loading from a backup will overwrite your local save. Are you sure you want to continue?";

        // Check if the user has signed in before download
        if (CheckAuth(gsm.AuthData))
        {
            // Show confirmation box before initiating download
            promptBox.Show(msg, onYesButtonClicked: () =>
            {
                StartCoroutine(IEDownloadGameProgress(gsm.AuthData));
            });
        }
    }

    public void Signout()
    {
        // Show confirmation box before initiating download
        var msg = "When you sign out, you won't be able to backup your progress online. However, your progress is automatically saved locally.\n\nDo you want to continue?";

        promptBox.Show(msg, onYesButtonClicked: () =>
        {
            StartCoroutine(IESignout());
        });
    }

    private IEnumerator IESignout()
    {
        ProgressLoaderNotifier.NotifyFourSegment(show: true);

        // Clear the session tokens
        authMan.ForgetUser();
        gsm.AuthData = default;

        // Overwrite local save
        var helper = UserDataHelper.Instance;
        var fresh = helper.SeedDefault();

        // Update the in-memory and on-disk saved data
        yield return StartCoroutine(helper.SaveUserData(fresh, d => gsm.UserSessionData = d));

        // Confirm the sign out by showing the login screen
        CheckAuth(gsm.AuthData);

        ProgressLoaderNotifier.NotifyFourSegment(show: false);

        msgBox.ShowSuccess("You've been signed out from your account");
    }
    /// <summary>
    /// Send the local save into the server
    /// </summary>
    private IEnumerator IEUploadGameProgress(PlayerAuthData authData)
    {
        ProgressLoaderNotifier.NotifyIndefiniteBar(show: true);

        // Make sure to save the local file first for any recent changes
        // before commiting the upload
        yield return StartCoroutine(UserDataHelper.Instance.SaveUserData(gsm.UserSessionData));

        // After saving the local copy, prepare it for upload
        byte[] fileData;
        
        try
        {
            fileData = File.ReadAllBytes(Constants.SavePath);
        }
        catch (IOException ex)
        {
            ProgressLoaderNotifier.NotifyIndefiniteBar(show: false);
            msgBox.ShowError("Unable to read local save file. Please try again.");

            yield break;
        }

        // Create a form and add the file data
        //var form = new WWWForm();
        //form.AddBinaryData("file", fileData, "user.sav", "application/octet-stream");

        using var www = UnityWebRequest.Put(UploadEndpoint, fileData); // (UploadEndpoint, form);

        // Add user ID and auth token to the request headers
        www.SetRequestHeader("UserID", authData.UserID);
        www.SetRequestHeader("Authorization", $"Bearer {authData.Token}");

        // Tell the server that the data being transmitted is raw binary data
        www.SetRequestHeader("Content-Type", "application/octet-stream");

        // Initiate the upload
        yield return www.SendWebRequest();
        
        // Close the progress dialog
        ProgressLoaderNotifier.NotifyIndefiniteBar(show: false);

        // Evaluate the results
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            
            msgBox.ShowError("Unable to sync your progress. Please check your connection and try again.", "Sync Failed!");
        }
        else
        {
            Debug.Log("Upload complete!");

            msgBox.ShowSuccess("Your progress has been successfully saved to the cloud.");
        }
    }

    /// <summary>
    /// Retrieve the save from server
    /// </summary>
    private IEnumerator IEDownloadGameProgress(PlayerAuthData authData)
    {
        ProgressLoaderNotifier.NotifyIndefiniteBar(show: true);

        using var www = UnityWebRequest.Get(DownloadEndpoint);

        www.SetRequestHeader("UserID", authData.UserID);
        www.SetRequestHeader("Authorization", $"Bearer {authData.Token}");
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        ProgressLoaderNotifier.NotifyIndefiniteBar(show: false);

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Download failed: " + www.error);

            if (www.responseCode == 404)
                msgBox.ShowError("It looks like you haven't saved your progress to the cloud yet.", "Oops!");
            else
                msgBox.ShowError("Sorry, we're having issues. Please check your connection and try again later.");
        }
        else
        {
            Debug.Log("File downloaded successfully!");

            byte[] fileData = www.downloadHandler.data;
            File.WriteAllBytes(DownloadPath, fileData);
            Debug.Log("File saved to: " + DownloadPath);

            // Process the downloaded save backup file
            yield return StartCoroutine(OverwriteLocalSave());
        }
    }

    /// <summary>
    /// Update the local save from the downloaded backup
    /// </summary>
    /// <returns></returns>
    private IEnumerator OverwriteLocalSave()
    {
        ProgressLoaderNotifier.NotifyFourSegment(show: true);

        // Make sure that the downloaded file exists
        if (!File.Exists(DownloadPath))
        {
            ProgressLoaderNotifier.NotifyFourSegment(show: false);
            msgBox.ShowError("Sorry, the save data can't be read. Please try again");
            yield break;
        }

        // Assume that the data exists, we begin deserializing it
        var formatter = new BinaryFormatter();
        var stream = new FileStream(DownloadPath, FileMode.Open);
        var data = formatter.Deserialize(stream) as UserData;

        stream.Close();

        yield return null;

        // Overwrite the in-memory data
        gsm.UserSessionData = data;

        // Overwrite the on-disk data
        yield return StartCoroutine(UserDataHelper.Instance.SaveUserData(data));

        // Delete the downloaded file
        try
        {
            if (File.Exists(DownloadPath))
                File.Delete(DownloadPath);

            Debug.Log("Downloaded file has been deleted");
        }
        catch
        {
            Debug.LogWarning("Unable to delete downloaded file");
        }

        // Done!
        ProgressLoaderNotifier.NotifyFourSegment(show: false);
        msgBox.ShowSuccess("Your save progress has been successfully loaded.");
    }
}
