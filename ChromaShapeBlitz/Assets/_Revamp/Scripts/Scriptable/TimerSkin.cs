using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Scriptable Objects/TimerSkin")]
public class TimerSkin : ScriptableObject
{
    public Color MainBackgroundColor;
    public Color RingOutline = new Color(0.0705F, 0.086F, 0.13F, 1.0F);
    public Color FillBackground;
    public Color RingFill;
}