using UnityEngine;

public enum MainMenuThemeIdentifier
{
    // Used in physical assets
    Stock,
    Halloween,
    Christmas,

    // Used internally as state identifier.
    // Auto applies the relevant theme by season.
    Auto
}

[CreateAssetMenu(fileName = "MainMenuThemeAsset", menuName = "Scriptable Objects/MainMenuThemeAsset")]
public class MainMenuThemeAsset : ScriptableObject
{
    public MainMenuThemeIdentifier ThemeIdentifier;

    #region ROOT_LAYOUT
    //
    //
    [Header("Root Layout")]
    public Sprite Logo;
    public Sprite Background;
    public Texture2D Foreground;
    public float ForegroundOpacity = 22.0F;
    //
    //
    #endregion ROOT_LAYOUT
    //
    //
    //
    #region MAIN_CONTROL_BUTTONS
    //
    //
    [Header("Main Controls")]
    public ButtonThemeAsset AboutButtonTheme;
    public ButtonThemeAsset CreditsButtonTheme;
    public ButtonThemeAsset PlayButtonTheme;
    public ButtonThemeAsset QuitButtonTheme;
    public ButtonThemeAsset ShopButtonTheme;
    public ToggleThemeAsset SfxToggleTheme;
    public ToggleThemeAsset BgmToggleTheme;
    //
    //
    #endregion MAIN_CONTROL_BUTTONS
    //
    //
    //
    #region SIDE_FABS
    //
    //
    public ButtonThemeAsset SlotMachineFabTheme;
    public ButtonThemeAsset DailyGiftFabTheme;
    public ButtonThemeAsset DecorsFabTheme;
    //
    //
    //
    #endregion SIDE_FABS
    //
    //
    //
    [Space(10)]
    [Header("Random bgm selection")]
    public AudioClip[] BgmThemes;
    //
    //
    //
    [Space(10)]
    [Header("Only for Christmas theme")]
    public Sprite[] ButtonSnowCaps;
}
