using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SoundToggle : MonoBehaviour
{
    [SerializeField] private SoundToggleType soundToggleType;

    private SettingsManager settingsManager;
    private BackgroundMusic bgmManager;
    private SoundEffects sfxManager;
    private UISound uixManager;
    private Toggle toggle;

    private bool isInitialized;

    void Start() => Initialize();
    void OnEnable() => Initialize();

    private void Initialize()
    {
        if (isInitialized)
            return;

        uixManager      = UISound.Instance;
        sfxManager      = SoundEffects.Instance;
        bgmManager      = BackgroundMusic.Instance;
        settingsManager = SettingsManager.Instance;

        TryGetComponent(out toggle);

        var settings = settingsManager.GetSettingsData();

        switch (soundToggleType)
        {
            case SoundToggleType.SFX:
                toggle.isOn = !settings.SfxEnabled;
                break;

            case SoundToggleType.BGM:
                toggle.isOn = !settings.BgmEnabled;
                break;
        }

        toggle.onValueChanged.AddListener(HandleToggled);
        isInitialized = true;
    }

    public void HandleToggled(bool isOn)
    {
        Initialize();

        if (isOn)
        {
            HandleSoundDisable();
            return;
        }

        HandleSoundEnable();
    }

    private void HandleSoundEnable()
    {
        switch (soundToggleType)
        {
            case SoundToggleType.SFX:
                sfxManager.UnMute();
                uixManager.UnMute();
                settingsManager.EnableSfx();
                break;

            case SoundToggleType.BGM:
                bgmManager.UnMute();
                settingsManager.EnableBgm();
                break;
        }
    }

    private void HandleSoundDisable()
    {
        switch (soundToggleType)
        {
            case SoundToggleType.SFX:
                sfxManager.Mute();
                uixManager.Mute();
                settingsManager.DisableSfx();
                break;

            case SoundToggleType.BGM:
                bgmManager.Mute();
                settingsManager.DisableBgm();
                break;
        }
    }
}