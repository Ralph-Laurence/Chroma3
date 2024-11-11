#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class SaveReserializer : MonoBehaviour
{
    private string jsonPath;
    private UserDataHelper usm;

    void Awake()
    {
        usm = UserDataHelper.Instance;
        jsonPath = $"{Application.persistentDataPath}/user.sav.json";
    }

    void Start()
    {
        Deserialize();
    }

    private void Deserialize()
    {
        if (!File.Exists(jsonPath))
        {
            Debug.LogWarning("JSON DOES NOT EXIST");
            return;
        }

        // Deserialize it
        try
        {
            var file = File.ReadAllText(jsonPath);
            var data = JsonUtility.FromJson<UserData>(file);
            Debug.Log("Data successfully deserialized");

            StartCoroutine(Reserialize(data));
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Reading failed... -> {ex.Message}");
        }
    }

    private IEnumerator Reserialize(UserData userData)
    {
        Debug.Log("Reserializing back to Binary....");
        yield return new WaitForSeconds(2.0F);

        yield return usm.SaveUserData(userData, (d) => Debug.Log("Success!"));
    }
}
#endif