using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CommonEventObserver))]
public class CommonActionObserverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Update the serialized object's representation.
        serializedObject.Update();

        // Draw the first help box
        EditorGUILayout.HelpBox("Execute common actions when notified", MessageType.Info);

        // Draw the actions field
        SerializedProperty actionsProperty = serializedObject.FindProperty("actions");
        EditorGUILayout.PropertyField(actionsProperty);

        // Add some space
        EditorGUILayout.Space(10);

        // Draw the second help box
        var helpNotifierTag = "This tag will be used by the event observer by comparing its current tag vs the " 
                            + "tag sent by notifier. If the tags match, the assigned actions will be invoked.";
                            
        EditorGUILayout.HelpBox(helpNotifierTag, MessageType.Info);

        // Draw the notifierTag field
        SerializedProperty notifierTagProperty = serializedObject.FindProperty("notifierTag");
        EditorGUILayout.PropertyField(notifierTagProperty);

        // Apply changes to the serialized object
        serializedObject.ApplyModifiedProperties();
    }
}