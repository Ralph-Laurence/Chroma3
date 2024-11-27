using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct HelpTopicItem
{
    public Sprite PreviewImage;

    [TextArea(minLines: 5, maxLines: 10)]
    public string Description;
}

[CreateAssetMenu(fileName = "HelpItem", menuName = "Scriptable Objects/HelpItem")]
public class HelpTopic : ScriptableObject
{
    public List<HelpTopicItem> TopicItems;
}
