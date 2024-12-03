using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuThemer : MonoBehaviour
{
    [SerializeField] private MainMenuThemeAsset themeAsset;
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

    private BackgroundMusic bgmManager;

    // Awake is called when the script instance is being loaded.
    void Awake()
    {
        bgmManager = BackgroundMusic.Instance;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logoImage.sprite = themeAsset.Logo;

        backgroundImage.sprite  = themeAsset.Background;
        foregroundImage.texture = themeAsset.Foreground;

        // Set the opacity of foreground
        var fgColor = foregroundImage.color;
        
        // Convert normalized (rgb 0-255) colors to rgb 0-1        
        fgColor.a = themeAsset.ForegroundOpacity / 255.0F;

        foregroundImage.color = fgColor;

        // Change side fabs (floating action button = fab)
        ChangeFABTheme(slotMachineSideFab, themeAsset.SlotMachineFabTheme);
        ChangeFABTheme(dailyGiftSideFab, themeAsset.DailyGiftFabTheme);
        ChangeFABTheme(decorSideFab, themeAsset.DecorsFabTheme);

        // Change the main control buttons' appearances
        ChangeButtonTheme(playButton, themeAsset.PlayButtonTheme);
        ChangeButtonTheme(shopButton, themeAsset.ShopButtonTheme);
        ChangeButtonTheme(quitButton, themeAsset.QuitButtonTheme);

        ChangeButtonTheme(aboutButton, themeAsset.AboutButtonTheme);
        ChangeButtonTheme(creditButton, themeAsset.CreditsButtonTheme);

        // Play the background music
        if (bgmManager != null)
        {
            // If no bgm was given from the theme, use the default
            if (themeAsset.Background == null)
            {
                bgmManager.PlayMainBgm();
                return;
            }
            
            // Select random bgm
            if (themeAsset.BgmThemes.Length > 1)
            {
                var randomIdx = Random.Range(0, themeAsset.BgmThemes.Length);
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
}
