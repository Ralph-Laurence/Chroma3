using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectController : MonoBehaviour
{
    [SerializeField] private GameObject stageSelectButtonPrefab;
    [SerializeField] private Sprite buttonAppearanceEasy;
    [SerializeField] private Sprite buttonAppearanceNormal;
    [SerializeField] private Sprite buttonAppearanceHard;

    [SerializeField] private ScrollRect scrollRect;

    [Header("RectTransform of scrollview content")]
    [SerializeField] private RectTransform scrollViewContent;

    private List<StageSelectButton> buttons;
    private Dictionary<LevelDifficulties, Sprite> buttonAppearances;

    void Start()
    {
        buttons = new List<StageSelectButton>();
        buttonAppearances = new Dictionary<LevelDifficulties, Sprite>
        {
            { LevelDifficulties.Easy,   buttonAppearanceEasy    },
            { LevelDifficulties.Normal, buttonAppearanceNormal  },
            { LevelDifficulties.Hard,   buttonAppearanceHard    },
        };

        var totalStages = Revamp.GameManager.TotalEasyStages;

        // Create those buttons
        for (var i = 0; i < totalStages; i++)
        {
            var btn = Instantiate(stageSelectButtonPrefab, scrollViewContent);
            btn.TryGetComponent(out StageSelectButton button);
            buttons.Add(button);
        }

        // The default level is easy, which is also the required pool number for buttons.
        SetDifficulty(LevelDifficulties.Easy, totalStages);
    }

    public void SetDifficulty(LevelDifficulties difficulty, int visibleButtons)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            if (i < visibleButtons)
            {
                buttons[i].SetTargetStage(difficulty, i + 1);
                buttons[i].SetAppearance(buttonAppearances[difficulty]);
                buttons[i].Show();
            }
            else
            {
                buttons[i].Hide();
            }
        }

        // Reset the scroll position
        scrollViewContent.anchoredPosition = Vector2.zero;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            SetDifficulty(LevelDifficulties.Easy,  Revamp.GameManager.TotalEasyStages);

        else if (Input.GetKeyDown(KeyCode.N))
            SetDifficulty(LevelDifficulties.Normal, Revamp.GameManager.TotalNormalStages);

        else if (Input.GetKeyDown(KeyCode.H))
            SetDifficulty(LevelDifficulties.Hard, Revamp.GameManager.TotalHardStages);
    }
}