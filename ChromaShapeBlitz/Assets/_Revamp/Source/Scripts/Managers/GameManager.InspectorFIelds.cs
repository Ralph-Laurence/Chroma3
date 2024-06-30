using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Revamp
{
    /// <summary>
    /// Nothing special here, other than maintaining the order of
    /// fields in the inspector.
    /// </summary>
    public partial class GameManager : MonoBehaviour
    {
        [Space(10)] [Header("Main Behaviour")]
        public StageCamera MainCamera;
        [SerializeField] private PatternTimer stageTimer;

        [SerializeField] private StageFactory stageFactory;
        [SerializeField] private TextMeshProUGUI[] stageTitleTexts;
        
        [Space(10)] [Header("Pause Menu")]
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private AudioClip pauseSound;
        [SerializeField] private AudioClip resumeSound;
        [SerializeField] GameObject mainMenuSceneLoader;
                
        [Space(10)] [Header("Objectives Menu")]
        [SerializeField] private TextMeshProUGUI[] objectiveTextsMinTime;
        [SerializeField] private TextMeshProUGUI[] objectiveTextsMaxTime;
        [SerializeField] private TextMeshProUGUI[] rewardsText;

        [Space(10)] [Header("Game Over Screen")]
        [SerializeField] private GameObject gameOverScreenOverlay;
        [SerializeField] private GameOverScreen gameOverScreenSuccess;
        [SerializeField] private GameOverScreen gameOverScreenFail;
    }
}