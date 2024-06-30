using UnityEngine;

public struct SettingsData
{
    public bool BgmEnabled;
    public bool SfxEnabled;
    public bool UISfxEnabled; // UI Sfx is dependent on SFX toggle state
}

public class SettingsManager
{
    #region SINGLETON

    public static SettingsManager instance;

    public static SettingsManager Instance
    {
        get
        {
            if (instance == null)
                instance = new SettingsManager();

            return instance;
        }
    }
    #endregion SINGLETON

    private readonly string PREF_KEY_BGM_TOGGLE    = "BGM_TOGGLE";
    private readonly string PREF_KEY_UISFX_TOGGLE  = "UI_SFX_TOGGLE";
    private readonly string PREF_KEY_SFX_TOGGLE    = "SFX_TOGGLE";

    private readonly int ENABLED = 1;
    private readonly int DISABLED = 0;

    public void EnableBgm()
    {
        PlayerPrefs.SetInt(PREF_KEY_BGM_TOGGLE, ENABLED);
        PlayerPrefs.Save();
    }

    public void EnableSfx()
    {
        PlayerPrefs.SetInt(PREF_KEY_SFX_TOGGLE, ENABLED);
        PlayerPrefs.SetInt(PREF_KEY_UISFX_TOGGLE, ENABLED);
        PlayerPrefs.Save();
    }

    public void DisableBgm()
    {
        PlayerPrefs.SetInt(PREF_KEY_BGM_TOGGLE, DISABLED);
        PlayerPrefs.Save();
    }

    public void DisableSfx()
    {
        PlayerPrefs.SetInt(PREF_KEY_SFX_TOGGLE, DISABLED);
        PlayerPrefs.SetInt(PREF_KEY_UISFX_TOGGLE, DISABLED);
        PlayerPrefs.Save();
    }

    public SettingsData GetSettingsData()
    {
        return new SettingsData
        {
            BgmEnabled   = ParseFlag(PREF_KEY_BGM_TOGGLE),
            SfxEnabled   = ParseFlag(PREF_KEY_SFX_TOGGLE),
            UISfxEnabled = ParseFlag(PREF_KEY_SFX_TOGGLE),
        };
    }

    public bool ParseFlag(string flagKey)
    {
        var flag = PlayerPrefs.GetInt(flagKey, DISABLED);

        if (flag == ENABLED)
            return true;

        return false;
    }
}