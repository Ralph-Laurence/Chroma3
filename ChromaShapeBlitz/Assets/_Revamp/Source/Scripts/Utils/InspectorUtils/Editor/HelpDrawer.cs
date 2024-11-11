using UnityEditor;
using UnityEngine;

namespace InspectorUtils
{
    [CustomPropertyDrawer(typeof(HelpAttribute))]
    public class HelpDrawer : PropertyDrawer
    {
        private MessageType ToMessageType(HelpBoxMessageTypes messageType)
        {
            return messageType switch
            {
                HelpBoxMessageTypes.Info => MessageType.Info,
                HelpBoxMessageTypes.Warning => MessageType.Warning,
                HelpBoxMessageTypes.Error => MessageType.Error,
                _ => MessageType.Info
            };
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Calculate the height for the help box
            float helpHeight = EditorGUIUtility.singleLineHeight * 2;
            Rect helpPosition = new Rect(position.x, position.y, position.width, helpHeight);
            Rect fieldPosition = new Rect(position.x, position.y + helpHeight + 2, position.width, position.height - helpHeight - 2);

            var msgType = ToMessageType(((HelpAttribute)attribute).MessageType);

            // Draw the help box
            EditorGUI.HelpBox(helpPosition, ((HelpAttribute)attribute).HelpText, msgType);

            // Draw the property field below the help box
            EditorGUI.PropertyField(fieldPosition, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Adjust the height to fit both the help box and the property field
            return EditorGUIUtility.singleLineHeight * 3 + 4; // Adjust as needed for spacing
        }
    }
}