using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavBar : MonoBehaviour
{
    [SerializeField] private List<GameObject> navPagesObj;
    [SerializeField] private List<NavButton> navButtons;
    private readonly List<NavContentPage> navPages = new();

    private NavContentPage tweenTargetContentPage;
    private bool navContentPageTweenBegan;
    private int lastSelectedPageIndex = 0;

    void Awake()
    {
        navButtons.ForEach(nav => nav.ButtonComponent.onClick.AddListener
        (
            delegate {
                var targetPage = nav.TargetPageIndex;
                HandleNavSelect(targetPage);
            }
        ));

        for (var i = 0; i < navPagesObj.Count; i++)
        {
            var page = navPagesObj[i];
            page.TryGetComponent(out NavContentPage contentPage);

            if (i > 0)
            {
                page.TryGetComponent(out RectTransform rect);
                rect.localScale = Vector2.zero;
            }

            navPages.Add(contentPage);
        }
    }

    void Update()
    {
        // Prevent the scrollview of the selected nav content page
        // from scrolling to the bottom as it scales up
        if (navContentPageTweenBegan && tweenTargetContentPage != null)
            tweenTargetContentPage.ResetScrollTop();
    }

    private void HandleNavSelect(int pageIdx)
    {
        if (pageIdx >= navPages.Count)
        {
            Debug.LogWarning($"No page at index: {pageIdx}");
            return;
        }

        if (lastSelectedPageIndex == pageIdx)
        {
            Debug.LogWarning("We are on the same page.");
            return;
        }

        var lastSelectedPageObj = navPages[lastSelectedPageIndex].gameObject;
        lastSelectedPageObj.TryGetComponent(out RectTransform lastSelectedPageRect);

        // Scale down the currently open page, then hide it
        LeanTween.scale(lastSelectedPageRect, Vector2.zero, 0.25F)
                 .setOnComplete(() => {
                    
                    // Mark the nav button as active
                    NavButtonSelectedNotifier.NotifyObserver(pageIdx);

                    lastSelectedPageObj.SetActive(false);

                    var currentPageObj = navPages[pageIdx].gameObject;
                    currentPageObj.TryGetComponent(out RectTransform currentPageRect);

                    // Show the selected page then scale it up.
                    currentPageObj.SetActive(true);

                    // Prevent the scrollview of the selected nav content page
                    // from scrolling to the bottom as it scales up
                    tweenTargetContentPage = navPages[pageIdx];
                    navContentPageTweenBegan = true;

                    // Update the last page index after scaling
                    LeanTween.scale(currentPageRect, Vector2.one, 0.25F)
                             .setOnComplete(() => 
                             {
                                lastSelectedPageIndex = pageIdx;
                                navContentPageTweenBegan = false;
                                tweenTargetContentPage = null;
                             });
                 });
    }
}
