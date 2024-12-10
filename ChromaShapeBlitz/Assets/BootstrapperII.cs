using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootstrapperII : MonoBehaviour
{
    [Header("Startup Animation")]
    [SerializeField] private GameObject logo;
    [SerializeField] private GameObject rays;

    [Header("Main UI")]
    [SerializeField] private GameObject meterbar;
    [SerializeField] private Slider     progressBar;
    [SerializeField] private Text       progressText;
    [SerializeField] private Text       progressCaption;

    [Space(10)]
    [SerializeField] private List<BlockSkinsAssetGroup> blockSkinGroups;

    [Space(10)]
    [Header("Cache default mats used by powerups.")]
    [SerializeField] private Material lightBlockMat;
    [SerializeField] private Material darkBlockMat;

    private GameSessionManager gsm;


    void Awake()
    {
        gsm = GameSessionManager.Instance;

        if (lightBlockMat != null && darkBlockMat != null)
        {
            gsm.CacheInitialBlockMats(lightBlockMat, darkBlockMat);
        }
    }

    private void Start()
    {
        // We immediately begin the data loading
        // after the animation finishes
        AnimateStartup(onFinished: () =>
        {
            StartCoroutine(Initialize());
        });
    }
    //
    //
    #region BOOTSTRAPPER_UI
    //
    //
    /// <summary>
    /// In the old bootstrapper, the animation is driven by Unity Animator.
    /// However, in this newer version, we animate the logo using LeanTween.
    /// </summary>
    private void AnimateStartup(Action onFinished)
    {
        var raysShown = false;

        // Make the logo gradually bigger
        LeanTween.scale(logo, Vector3.one, 0.55F)
                 .setEase(LeanTweenType.easeInOutQuad);

        // Spin the logo as it scales up
        LeanTween.rotateZ(logo, 360.0F * 2.0F, 0.55F)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnUpdate((float time) =>
                 {
                     if (time >= 0.5F && !raysShown)
                     {
                         LeanTween.scale(rays, Vector3.one, 0.65F);
                         raysShown = true;
                     }
                 })
                 .setOnComplete(onFinished);
    }

    /// <summary>
    /// Update the progress of the meter bar smoothly instead of 'jumping'.
    /// </summary>
    /// <param name="progress">The progress value</param>
    /// <param name="caption">The message of the progress</param>
    private IEnumerator UpdateLoadingProgress(float targetValue, string caption)
    {
        progressCaption.text = caption;

        LeanTween.value(progressBar.gameObject, progressBar.value, targetValue, 1.0F)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnUpdate((float value) => { progressBar.value = value; })
                 .setOnComplete(() =>
                 {
                     // Ensure the slider value is exactly the target value at the end
                     progressBar.value = targetValue;
                     progressText.text = $"{progressBar.value} %";
                 });

        yield return new WaitForSeconds(1.0F);
    }
    //
    //
    //
    #endregion BOOTSTRAPPER_UI
    //
    //
    //
    #region BOOTSTRAPPER_BEHAVIOURS
    //
    //
    //

    /// <summary>
    /// The starting point of data loading
    /// </summary>
    /// <returns>CoRoutine</returns>
    private IEnumerator Initialize()
    {
        // Disable screen dimming
        //Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // Show the progress of data loading
        meterbar.SetActive(true);

        yield return StartCoroutine(UpdateLoadingProgress(10, "Initializing..."));
        yield return new WaitForSeconds(0.5F);

        var tasks = new List<Func<IEnumerator>>()
        {
            () => UpdateLoadingProgress(25, "Loading user data..."),
            LoadUserData,
            () => UpdateLoadingProgress(50, "Loading user data..."),
            CacheAppliedSkins,
            () => UpdateLoadingProgress(75, "Preparing audio..."),
            LoadUserAudioSettings,
            () => UpdateLoadingProgress(90, "Authenticating user..."),
            LoadAuthCache,
            () => UpdateLoadingProgress(100, "Starting..."),
            LaunchMainMenu
        };

        for (var i = 0; i < tasks.Count; i++)
        {
            var task = tasks[i];
            yield return StartCoroutine(task());
        }

        yield return null;
    }

    /// <summary>
    /// Redirect to the main after the userdata has been fully loaded.
    /// </summary>
    /// <returns>CoRoutine</returns>
    private IEnumerator LaunchMainMenu()
    {
        // Start loading the scene
        var asyncLoad = SceneManager.LoadSceneAsync(Constants.Scenes.MainMenu, LoadSceneMode.Single);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Wait a frame so every Awake and Start method is called
        yield return new WaitForEndOfFrame();
    }

    #endregion BOOTSTRAPPER_BEHAVIOURS
    //
    //
    //
    #region DATA_LOADING
    //
    //
    //

    /// <summary>
    /// Deserialize the user save data from the binary "user.sav" file.
    /// When the data doesn't exist, UserDataHelper seeds a new user data.
    /// </summary>
    /// <returns>CoRoutine</returns>
    private IEnumerator LoadUserData()
    {
        yield return StartCoroutine(UserDataHelper.Instance.LoadUserData((userData) =>
        {
            gsm.UserSessionData = userData;
        }));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadAuthCache()
    {
        gsm.AuthData = PlayerAuthManager.Instance.LoadAuthData();
        yield return new WaitForSeconds(0.25F);
    }

    /// <summary>
    /// These are the currently active block skins. We need to cache the
    /// active skins into a session, for use later.
    /// </summary>
    private IEnumerator CacheAppliedSkins() // (UserData userData)
    {
        var userData = gsm.UserSessionData;

        var activeSkins = userData.ActiveBlockSkins.ToList();

        foreach (var groups in blockSkinGroups)
        {
            foreach (var skin in groups.SkinGroup)
            {
                if (!activeSkins.Contains(skin.Id))
                    continue;

                gsm.SetActiveBlockSkinMaterial(skin.ColorCategory, skin.Material);
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.25F);
    }

    /// <summary>
    /// Load user audio preferences from PlayerPrefs then apply them.
    /// </summary>
    /// <returns>CoRoutine</returns>
    private IEnumerator LoadUserAudioSettings()
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

        yield return new WaitForSeconds(0.25F);
    }
    //
    //
    //
    #endregion DATA_LOADING
    //
    //
    //
}
