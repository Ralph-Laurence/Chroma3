using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{
// URL of the login endpoint
    private string loginUrl = "http://localhost:3000/users/api/login";

    // Class to represent login data
    [System.Serializable]
    public class LoginData
    {
        public string username;
        public string password;
    }

    // Method to perform login
    public void Login(string username, string password)
    {
        StartCoroutine(LoginCoroutine(username, password));
    }

    // Coroutine to handle the login request
    private IEnumerator LoginCoroutine(string username, string password)
    {
        // Create the login data object
        var loginData = new LoginData { username = username, password = password };

        // Convert the login data object to JSON
        string jsonData = JsonUtility.ToJson(loginData);

        // Create a new UnityWebRequest for the POST request
        var www = new UnityWebRequest(loginUrl, "POST");

        // Attach the JSON data to the request
        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for a response
        yield return www.SendWebRequest();

        // Check for errors
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Login failed: " + www.error);
        }
        else
        {
            // Handle the response
            Debug.Log("Login successful: " + www.downloadHandler.text);
        }
    }
}
