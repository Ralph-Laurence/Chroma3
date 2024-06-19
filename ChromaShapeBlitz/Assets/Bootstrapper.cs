using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bootstrapper : MonoBehaviour
{
    private struct ActionStatusEventData
    {
        public int Progress;
        public int StatusCode;
    }

    [SerializeField] private GameObject loaderView;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text progressText;
    [SerializeField] private Text progressCaption;

    private int totalTasks;
    private int tasksDone;

    private List<Func<IEnumerator>> initializationTasks;

    public void Ev_AnimationEnded() => StartCoroutine( Initialize() );
    private readonly Dictionary<int, string> actionReportMessages = new Dictionary<int, string>
    {
        { 200,  "Starting..."},
        { StatusCodes.BEGIN_WRITE_USER_DATA,  "Creating user data..." },
        { StatusCodes.DONE_WRITE_USER_DATA,   "User data created successfully" },
        { StatusCodes.BEGIN_READ_USER_DATA,   "Reading user data..." },
        { StatusCodes.DONE_READ_USER_DATA,    "User data loaded successfully"  },
        { StatusCodes.BEGIN_BUILD_LEVEL_MENU, "Loading levels..."},
        { StatusCodes.DONE_BUILD_LEVEL_MENU,  "Almost there"},
    };

    // Queue to hold the event status data
    // private Queue<ActionStatusEventData> eventQueue;
    private readonly Queue<int> actionStatusEventQueue = new Queue<int>();
    private bool isProcessingEventQueue;

    void OnEnable()
    {
        ActionStatusNotifier.BindEvent(ObserveActionStatus);
    }

    void OnDisable()
    {
        ActionStatusNotifier.UnbindEvent(ObserveActionStatus);
    }

    void Update()
    {
        // If there is data in the queue and we're not currently displaying anything
        //!IsInvoking(nameof(DisplayEventQueue)))
        if (actionStatusEventQueue.Count > 0 && !isProcessingEventQueue)
        {
            // Start the coroutine to display the next event after a delay
            StartCoroutine(ProcessNotifiedEventQueue());
        }
    }

    private IEnumerator SetProgress(float progress, string caption)
    {
        progressCaption.text = caption;

        // Calculate the target value for the slider
        float targetValue = progressBar.value + progress;

        // The time it takes to fill up the slider
        float duration = 1f; // Change this to whatever you want

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            progressBar.value = Mathf.Lerp(progressBar.value, targetValue, elapsed / duration);
            progressText.text = $"{progressBar.value} %";
            yield return null;
        }

        // Ensure the slider value is exactly the target value at the end
        progressBar.value = targetValue;
        progressText.text = $"{progressBar.value} %";

        isProcessingEventQueue = false;
    }

    private void ObserveActionStatus(int statusCode)
    {
        actionStatusEventQueue.Enqueue(statusCode);
    }

    private IEnumerator ProcessNotifiedEventQueue()
    {
        isProcessingEventQueue = true;

        yield return new WaitForSeconds(1.0F);

        var statusCode = actionStatusEventQueue.Dequeue();
        
        yield return StartCoroutine
        (
            SetProgress(25, actionReportMessages[statusCode])
        );

        tasksDone++;

        if (tasksDone == totalTasks)
        {
            yield return new WaitForSeconds(1.0F);
            yield return StartCoroutine(LaunchMainMenu());
        }
    }

    private IEnumerator Initialize()
    {
        loaderView.SetActive(true);

        initializationTasks = new List<Func<IEnumerator>>
        {
            UserDataHelper.Instance.LoadUserData,
            LevelMenuController.Instance.Initialize,
            ApplyAudioUserSettings
        };

        totalTasks = initializationTasks.Count;

        foreach(var task in initializationTasks)
        {
            yield return StartCoroutine(task());
        };
    }

    private IEnumerator LaunchMainMenu()
    {
        yield return StartCoroutine(SetProgress(100.0F, actionReportMessages[200]));

        // Start loading the scene
        var asyncLoad = SceneManager.LoadSceneAsync("MainMenu");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator ApplyAudioUserSettings()
    {
        if (SettingsManager.Instance == null)
            yield break;

        var settings = SettingsManager.Instance.GetSettingsData();

        var bgm = BackgroundMusic.Instance;
        var sfx = SoundEffects.Instance;
        var uix = UISound.Instance;

        var bgmEnabled = bgm != null && settings.BgmEnabled;
        var sfxEnabled = sfx != null && settings.SfxEnabled;  
        var uixEnabled = uix != null && settings.UISfxEnabled;

        bgm.SetMute(!bgmEnabled);
        sfx.SetMute(!sfxEnabled);
        uix.SetMute(!uixEnabled);

        yield return null;
    }

}
