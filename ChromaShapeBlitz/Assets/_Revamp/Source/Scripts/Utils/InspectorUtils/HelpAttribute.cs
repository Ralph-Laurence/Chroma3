using UnityEngine;

namespace InspectorUtils
{
    public class HelpAttribute : PropertyAttribute
    {
        public string HelpText { get; private set; }
        public HelpBoxMessageTypes MessageType { get; private set; }
        
        public HelpAttribute(string helpText, HelpBoxMessageTypes messageType = HelpBoxMessageTypes.Info)
        {
            HelpText = helpText;
            MessageType = messageType;
        }
    }
}
