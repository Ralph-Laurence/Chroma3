using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpDriver : MonoBehaviour
{
    [SerializeField] private HelpTopic helpTopic;

    [Space(10)]
    [SerializeField] private GameObject topicPreviewWrapper;
    [SerializeField] private Image topicPreview;
    [SerializeField] private TextMeshProUGUI topicDescription;
    [SerializeField] private GameObject btnNext;
    [SerializeField] private GameObject btnPrev;
    [SerializeField] private Image[] paginationIndicators;
    [SerializeField] private Sprite paginationInactive;
    [SerializeField] private Sprite paginationActive; 

    private int currentTopicIndex;

    void Start()
    {
        if (helpTopic == null)
        {
            Debug.LogWarning($"Please assign a help topic to {gameObject.name}!");
            return;
        }

        // Setup the pagination indicators. We will enable each of them according to total topics
        for (var i = 0; i < paginationIndicators.Length; i++)
        {
            var indicator = paginationIndicators[i];

            if (i >= helpTopic.TotalTopics)
                break;

            indicator.gameObject.SetActive(true);
        }

        // Initially set the first help topic to display
        ScrollPage(0);
    }
    //
    // Assign these to the scroller buttons
    //
    public void Ev_ShowNextTopic()
    {
        if (currentTopicIndex < helpTopic.TotalTopics - 1)
        {
            currentTopicIndex++;
            ScrollPage(currentTopicIndex);
        }
    }
    public void Ev_ShowPrevTopic()
    {
        if (currentTopicIndex > 0)
        {
            currentTopicIndex--;
            ScrollPage(currentTopicIndex);
        }
    }

    public void ScrollPage(int pageIndex)
    {
        UpdateHelpTopicButtons(pageIndex);
        UpdateTrackerLocation(pageIndex);

        currentTopicIndex = pageIndex;
        
        var selectedTopic = helpTopic.TopicItems[pageIndex];

        topicDescription.text = selectedTopic.Description;

        if (selectedTopic.PreviewImage != null)
        {
            topicPreviewWrapper.SetActive(true);
            topicPreview.sprite = selectedTopic.PreviewImage;
            return;
        }

        topicPreviewWrapper.SetActive(false);
    }
    //
    // Show or hide the help scroll buttons (i.e. Back | Next)
    // depending on the current scrolled page location
    //
    private void UpdateHelpTopicButtons(int pageIndex)
    {
        if (pageIndex == 0)
        {
            btnPrev.SetActive(false);
            btnNext.SetActive(true);
        }
        else if (pageIndex == helpTopic.TotalTopics - 1)
        {
            btnPrev.SetActive(true);
            btnNext.SetActive(false);
        }
        else
        {
            btnPrev.SetActive(true);
            btnNext.SetActive(true);
        }
    }
    //
    // Page scroll index indicator dots
    //
    private void UpdateTrackerLocation(int pageIndex)
    {
        RectTransform rect;

        foreach (var tracker in paginationIndicators)
        {
            tracker.sprite = paginationInactive;
            tracker.TryGetComponent(out rect);

            rect.sizeDelta = new (12.0F, 12.0F);
        }

        paginationIndicators[pageIndex].sprite = paginationActive;
        paginationIndicators[pageIndex].TryGetComponent(out rect);

        rect.sizeDelta = new (18.0F, 18.0F);
    }
}
