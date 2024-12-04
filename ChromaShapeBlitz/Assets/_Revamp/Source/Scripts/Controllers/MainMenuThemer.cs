using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuThemer : MonoBehaviour
{
    // [SerializeField] private MainMenuThemeAsset themeAsset;
    [SerializeField] private MainMenuThemeAssetLoader themeAssetLoader;

    [Space(10)]
    [SerializeField] private Image logoImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private RawImage foregroundImage;

    [Space(10)]
    [SerializeField] private MainMenuSideFab slotMachineSideFab;
    [SerializeField] private MainMenuSideFab dailyGiftSideFab;
    [SerializeField] private MainMenuSideFab decorSideFab;

    [Space(10)]
    [SerializeField] private ThemableButton playButton;
    [SerializeField] private ThemableButton shopButton;
    [SerializeField] private ThemableButton quitButton;

    [Space(10)]
    [SerializeField] private ThemableButton aboutButton;
    [SerializeField] private ThemableButton creditButton;

    [Space(10)]
    [SerializeField] private ThemableToggle sfxToggle;
    [SerializeField] private ThemableToggle bgmToggle;

    private BackgroundMusic bgmManager;
    private GameSessionManager gsm;
    private string StockThemeName => MainMenuThemeIdentifier.Stock.ToString();

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        bgmManager = BackgroundMusic.Instance;
        gsm = GameSessionManager.Instance;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        string themeAddress;
        var themeToUse = gsm.UserSessionData.MainMenuTheme;

        // When theme identifier is set to "Auto", the
        // game decides automatically which theme to use
        // according to the current month's festive holiday.
        // When the current month has no festivity, the game
        // loads the theme from user saved data.
        // As a fallback, the stock (default) theme is used instead.
        if (themeToUse == MainMenuThemeIdentifier.Auto)
        {
            // Load the correct theme by current season
            var currentMonth = DateTime.Now.Month;

            themeAddress = currentMonth switch
            {
                11 => Constants.PackedAssetAddresses.HalloweenTheme.ToString(),
                12 => Constants.PackedAssetAddresses.ChristmasTheme.ToString(),
                _ => StockThemeName
            };
        }
        // The addressable theme asset has a suffix "Theme".
        // Except for "Auto" and "Stock"
        else
        {
            themeAddress = (!themeToUse.ToString().Equals(StockThemeName))
                         ? $"{themeToUse}Theme"
                         : themeToUse.ToString();
        }

        Debug.Log($"Uses theme -> {themeToUse}");
        
        // Use the default background music when there is no theme loaded
        if (themeAddress.Equals(StockThemeName))
        {    
            bgmManager.PlayMainBgm();
            return;
        }

        // Use the theme
        StartCoroutine(IELoadTheme(themeAddress));
    }

    private IEnumerator IELoadTheme(string themeAddress)
    {
        MainMenuThemeAsset themeAsset = default;

        yield return StartCoroutine(themeAssetLoader.IELoadThemeAsset
        (
            address: themeAddress,
            onResult: (result) => themeAsset = result
        ));

        // Load the bare minimum when we wont use a theme.
        if (themeAsset == null)
        {
            // Use the default background music when there is no theme given
            bgmManager.PlayMainBgm();
            Debug.Log("Theme Not Found!");
            yield break;
        }

        // Apply the theme when available
        ApplyTheme(themeAsset);
    }

    private void ApplyTheme(MainMenuThemeAsset themeAsset)
    {
        logoImage.sprite        = themeAsset.Logo;
        backgroundImage.sprite  = themeAsset.Background;
        foregroundImage.texture = themeAsset.Foreground;

        // Set the opacity of foreground
        var fgColor = foregroundImage.color;
        
        // Convert normalized (rgb 0-255) colors to rgb 0-1        
        fgColor.a = themeAsset.ForegroundOpacity / 255.0F;

        foregroundImage.color = fgColor;

        // Change side fabs (floating action button = fab)
        ChangeFABTheme(slotMachineSideFab,  themeAsset.SlotMachineFabTheme);
        ChangeFABTheme(dailyGiftSideFab,    themeAsset.DailyGiftFabTheme);
        ChangeFABTheme(decorSideFab,        themeAsset.DecorsFabTheme);

        // Change the main control buttons' appearances
        ChangeButtonTheme(playButton,       themeAsset.PlayButtonTheme);
        ChangeButtonTheme(shopButton,       themeAsset.ShopButtonTheme);
        ChangeButtonTheme(quitButton,       themeAsset.QuitButtonTheme);
        ChangeButtonTheme(aboutButton,      themeAsset.AboutButtonTheme);
        ChangeButtonTheme(creditButton,     themeAsset.CreditsButtonTheme);

        // In some themes, the sound toggles can be optional.
        if (themeAsset.SfxToggleTheme != null)
            ChangeToggleTheme(sfxToggle, themeAsset.SfxToggleTheme);

        if (themeAsset.BgmToggleTheme != null)
            ChangeToggleTheme(bgmToggle, themeAsset.BgmToggleTheme);

        // This is only applicable on Christmas theme
        HandleOnChristmasThemeExtras(themeAsset);

        // Play the theme's background music
        PlayThemeBgm(themeAsset);
    }

    private void ChangeFABTheme(MainMenuSideFab fab, ButtonThemeAsset buttonTheme)
    {
        fab.ChangeFabIcon(buttonTheme.BaseSprite);
        fab.SetScale(buttonTheme.BaseScale);
    }

    private void ChangeButtonTheme(ThemableButton btn, ButtonThemeAsset buttonTheme)
    {
        var normal = buttonTheme.NormalColor;
        var hover  = buttonTheme.HoverColor;
        var click  = buttonTheme.ClickColor;

        btn.SetColor(normal, hover, click);
        btn.SetIcon(buttonTheme.Icon);
        btn.ScaleIcon(buttonTheme.IconScale);
    }

    private void ChangeToggleTheme(ThemableToggle toggle, ToggleThemeAsset toggleTheme)
    {
        var normal = toggleTheme.NormalColor;
        var hover  = toggleTheme.HoverColor;
        var click  = toggleTheme.ClickColor;

        toggle.SetColor(normal, hover, click);
        toggle.SetBackground(toggleTheme.BaseSprite);
    }

    private void AddSnowCaps(ThemableButton[] buttons, MainMenuThemeAsset themeAsset)
    {
        for (var i = 0; i < buttons.Length; i++)
        {
            // Reference to each button
            var button = buttons[i];

            // Create a snow cap gameobject
            var snowCapObj = new GameObject("SnowCap");

            // Apply scalings and anchorings.
            // We will stretch the snowcap on top of the button
            var rect = snowCapObj.AddComponent(typeof(RectTransform)) as RectTransform;

            // Make the snowcap object a child of "button"
            snowCapObj.transform.SetParent(button.transform, true);

            rect.SetAsFirstSibling();
            rect.anchorMin  = new Vector2(0.0F, 1.0F);
            rect.anchorMax  = Vector2.one;
            rect.pivot      = new Vector2(0.5F, 1.0F);
            rect.localScale = Vector2.one;
            rect.offsetMin  = Vector2.zero;
            rect.offsetMax  = Vector2.zero;

            // Stretch the snowcap on top of its parent button
            var pos = rect.anchoredPosition;
            pos.y = 4.0F;
            pos.x = 0.0F;
            rect.anchoredPosition = pos;

            // Snowcaps are 40px in height
            var rectSize = rect.sizeDelta;
            rectSize.y = 40.0F;
            rect.sizeDelta = rectSize;

            // Select a random snow cap
            var snowCapArr = themeAsset.ButtonSnowCaps;
            var snowCapIdx = UnityEngine.Random.Range(0, snowCapArr.Length);
            var snowCapSpt = snowCapArr[snowCapIdx];
            var snowCapImg = snowCapObj.AddComponent(typeof(Image)) as Image;

            snowCapImg.sprite = snowCapSpt;
        }
    }

    private void HandleOnChristmasThemeExtras(MainMenuThemeAsset themeAsset)
    {
        if (themeAsset.ThemeIdentifier != MainMenuThemeIdentifier.Christmas)
            return;

        // Make sure to add snow caps only when using the Christmas theme
        var buttons = new ThemableButton[]
        {
            playButton,
            shopButton,
            quitButton,
            aboutButton,
            creditButton
        };

        AddSnowCaps(buttons, themeAsset);
    }

    private void PlayThemeBgm(MainMenuThemeAsset themeAsset)
    {
        if (bgmManager == null)
            return;

        // Select random bgm
        if (themeAsset.BgmThemes.Length > 1)
        {
            var randomIdx = UnityEngine.Random.Range(0, themeAsset.BgmThemes.Length);
            var randomBgm = themeAsset.BgmThemes[randomIdx];
            bgmManager.SetClip(randomBgm);
        }
        else
        {
            // Use the single bgm from the theme instead
            bgmManager.SetClip(themeAsset.BgmThemes[0]);
        }

        bgmManager.Play();
    }
}
