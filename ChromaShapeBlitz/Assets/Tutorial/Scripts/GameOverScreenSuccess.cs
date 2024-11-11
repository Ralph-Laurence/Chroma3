using TMPro;
using UnityEngine;

public class GameOverScreenSuccess : GameOverScreenBase
{
    [Space(10)]
    [Header("\u25A7\u25A7\u25A7\u25A7\u25A7\u25A7 Total Play Time \u25A8\u25A8\u25A8\u25A8\u25A8\u25A8")]
    [Space(10)]
    [SerializeField] private TutorialPatternTimer timer;
    [SerializeField] private TextMeshProUGUI playTimeText;

    void Start()
    {
        if (timer == null || playTimeText == null)
            return;

        playTimeText.text = $"{timer.ElapsedSeconds} secs";
    }
}
