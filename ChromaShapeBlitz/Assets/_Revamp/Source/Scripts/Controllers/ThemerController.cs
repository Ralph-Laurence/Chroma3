using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemerController : MonoBehaviour
{
    [SerializeField] List<ThemerCard> cards;
    [SerializeField] private GameObject cardMarker;
    [SerializeField] private GameObject fancySceneLoader;

    [Space(10)]
    [SerializeField] private MainMenuThemeAssetLoader themeAssetLoader;
    [SerializeField] private Image background;
    [SerializeField] private RawImage foreground;
    [SerializeField] private Image logo;

    private RectTransform activeCardMarker;
    private GameSessionManager gsm;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        gsm = GameSessionManager.Instance;
        cardMarker.TryGetComponent(out activeCardMarker);
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        var userData = gsm.UserSessionData;
        var currentTheme = userData.MainMenuTheme;

        foreach (var card in cards)
        {
            if (card.themeIdentifier == currentTheme)
            {
                MarkCardAsActive(card);
                break;
            }
        }

        // Use the theme as initial preview of the page
        var themeAddress = new Dictionary<MainMenuThemeIdentifier, string>
        {
            { MainMenuThemeIdentifier.Stock    , Constants.PackedAssetAddresses.StockTheme},
            { MainMenuThemeIdentifier.Halloween, Constants.PackedAssetAddresses.HalloweenTheme},
            { MainMenuThemeIdentifier.Christmas, Constants.PackedAssetAddresses.ChristmasTheme},
        };
        
        if (themeAddress.ContainsKey(currentTheme))
        {
            StartCoroutine(IELoadTheme(themeAddress[currentTheme]));
        }
    }

    private void ApplyTheme(MainMenuThemeIdentifier themeIdentifier)
    {
        var userData = gsm.UserSessionData;
        userData.MainMenuTheme = themeIdentifier;

        StartCoroutine(UserDataHelper.Instance.SaveUserData(userData, (u) => ExitToMenu()));
    }

    private void MarkCardAsActive(ThemerCard card)
    {
        // Lock the card to make sure that it wont
        // be able to recieve clicks.
        card.Lock();

        // Make the marker UI a child of the card
        activeCardMarker.SetParent(card.transform);

        activeCardMarker.anchorMin  = new Vector2(0.0F ,1.0F);
        activeCardMarker.anchorMax  = new Vector2(0.0F ,1.0F);
        activeCardMarker.pivot      = new Vector2(0.0F ,1.0F);
        activeCardMarker.localScale = Vector3.one;

        // Set the Left position of stretched rect transform
        var offsetMin = activeCardMarker.offsetMin;
        offsetMin.x = 0.5F;
        
        activeCardMarker.offsetMin = offsetMin;

        // Set the Y Pos of the stretched rect transform
        var posY = activeCardMarker.anchoredPosition;
        posY.y = -0.5F;

        activeCardMarker.anchoredPosition = posY;
        activeCardMarker.sizeDelta = Vector2.one * 96.0F;

        // Show the timer UI then Run the countdown
        cardMarker.SetActive(true);
    }

    private void ExitToMenu()
    {
        Instantiate(fancySceneLoader).TryGetComponent(out FancySceneLoader loader);
        loader.LoadScene(Constants.Scenes.MainMenu);
    }

    private IEnumerator IELoadTheme(string themeAddress)
    {
        MainMenuThemeAsset themeAsset = default;

        yield return StartCoroutine(themeAssetLoader.IELoadThemeAsset
        (
            address: themeAddress,
            onResult: (result) => themeAsset = result
        ));

        if (themeAsset != null)
        {
            Debug.Log("Not null");
            background.sprite  = themeAsset.Background;
            foreground.texture = themeAsset.Foreground;
            foreground.texture = themeAsset.Foreground;

            // Set the opacity of foreground
            var fgColor = foreground.color;
        
            // Convert normalized (rgb 0-255) colors to rgb 0-1        
            fgColor.a = themeAsset.ForegroundOpacity / 255.0F;

            foreground.color = fgColor;

            logo.sprite = themeAsset.Logo;
        }
    }

    public void Ev_ApplyStockTheme() => ApplyTheme(MainMenuThemeIdentifier.Stock);
    public void Ev_ApplyHalloweenTheme() => ApplyTheme(MainMenuThemeIdentifier.Halloween);
    public void Ev_ApplyChristmasTheme() => ApplyTheme(MainMenuThemeIdentifier.Christmas);
    public void Ev_ApplyAutoTheme() => ApplyTheme(MainMenuThemeIdentifier.Auto);
    public void Ev_ExitToMenu() => ExitToMenu();
}
    