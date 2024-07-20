using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class TestReadFolder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Specify the path to the folder containing the audio clips
        string folderPath = "Assets/_Revamp/Art/Sounds";

        // Find all assets in the specified folder
        string[] guids = AssetDatabase.FindAssets("t:AudioClip", new[] { folderPath });

        // Loop through each asset and load it
        foreach (string guid in guids)
        {
            // Get the path of the asset
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Load the audio clip
            AudioClip audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);

            if (audioClip != null)
            {
                // Output the name of the audio clip to the console
                Debug.Log("Loaded Audio Clip: " + audioClip.name);
            }
            else
            {
                Debug.LogError("Failed to load audio clip at path: " + assetPath);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
#endif