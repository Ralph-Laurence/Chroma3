using UnityEngine;
using UnityEngine.UI;

public class NavContentPage : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;

    /// <summary>
    /// Resets the scrollview position to top
    /// </summary>
    public void ResetScrollTop() => scrollRect.verticalNormalizedPosition = 1.0F;
}