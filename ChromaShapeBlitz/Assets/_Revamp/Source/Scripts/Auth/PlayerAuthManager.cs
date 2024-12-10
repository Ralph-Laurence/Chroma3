using System.Collections.Generic;
using UnityEngine;

public class PlayerAuthManager
{
    #region SINGLETON

    private static PlayerAuthManager instance;

    public static PlayerAuthManager Instance
    {
        get
        {
            instance ??= new PlayerAuthManager();
            return instance;
        }
    }
    #endregion SINGLETON

    public const string PK_AUTH_TOKEN = "auth_token";
    public const string PK_USERNAME   = "username";
    public const string PK_USERID     = "userid";

    public void RememberUser(PlayerAuthData authData)
    {
        if (authData == null)
        {
            Debug.LogWarning("Auth data is not available!");
            return;
        }

        // We cant save an empty auth data
        if (!ValidateAuthData(authData))
            return;

        string authToken = authData.Token;
        string userId    = authData.UserID;
        string userName  = authData.Username;

        Debug.Log($"{authToken}, {userId}, {userName}");

        PlayerPrefs.SetString(PK_AUTH_TOKEN, authToken);
        PlayerPrefs.SetString(PK_USERID,     userId);
        PlayerPrefs.SetString(PK_USERNAME,   userName);
        PlayerPrefs.Save();
    }

    public void ForgetUser()
    {
        PlayerPrefs.DeleteKey(PK_AUTH_TOKEN);
        PlayerPrefs.DeleteKey(PK_USERID);
        PlayerPrefs.DeleteKey(PK_USERNAME);
        PlayerPrefs.Save();
    }

    public PlayerAuthData LoadAuthData()
    {
        return new PlayerAuthData()
        {
            Token     = PlayerPrefs.GetString(PK_AUTH_TOKEN, string.Empty),
            UserID    = PlayerPrefs.GetString(PK_USERID,     string.Empty),
            Username  = PlayerPrefs.GetString(PK_USERNAME,   string.Empty)
        };
    }

    /// <summary>
    /// Check if all the necessary auth data arent empty
    /// </summary>
    public bool IsComplete(PlayerAuthData authData) => ValidateAuthData(authData);

    public bool ValidateAuthData(PlayerAuthData authData)
    {
        var data = new string[]
        {
            authData.Token,
            authData.Username,
            authData.UserID
        };

        for (var i = 0; i < data.Length; i++)
        {
            var o = data[i];

            if (string.IsNullOrEmpty(o))
            {
                Debug.LogWarning($"{o} is empty!");
                return false;
            }
        }
        
        return true;
    }
}