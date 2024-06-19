using TMPro;
using UnityEngine;

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

        [SerializeField] private AudioClip incorrectBlocksSfx;
        [SerializeField] private StageFactory stageFactory;
        
        [Space(10)] [Header("Pause Menu")]
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private TextMeshProUGUI pauseMenuStageTitle;
        [SerializeField] private AudioClip pauseSound;
        [SerializeField] private AudioClip resumeSound;
        [SerializeField] GameObject mainMenuSceneLoader;
    }
}