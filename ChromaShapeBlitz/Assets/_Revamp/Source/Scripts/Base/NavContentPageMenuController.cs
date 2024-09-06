using UnityEngine;

public class NavContentPageMenuController : MonoBehaviour
{
    public int PageID { get; set; }

    /// <summary>
    /// Invoked after LeanTween has done transitioning to show this page
    /// </summary>
    /// <param name="pageId"></param>
    public virtual void OnBecameFullyVisible(int pageId)
    {
        // Override this method
    }
}