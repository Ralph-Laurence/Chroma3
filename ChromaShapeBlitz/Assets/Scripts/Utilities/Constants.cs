using System.IO;
using UnityEngine;

public class Constants
{
    public const string SHADER_MAIN_TEX = "_MainTex";
    public const string SHADER_BASE_MAP = "_BaseMap";

    // Prefixes of scene names
    public const string StageNameEasy    = "Easy_";
    public const string StageNameNormal  = "Normal_";
    public const string StageNameHard    = "Hard_";


    public const int    StartingStage    = 1;

    public const string SplashAnimationState        = "Animate Splash";
    public const string ControlButtonAnimationState = "Animate Control Buttons";

    public struct DataPaths
    {
        public static string PlayerProgress     => Path.Combine(Application.persistentDataPath, "player.io");
        public static string LevelStructure     => Path.Combine(Application.persistentDataPath, "level.io");
        public static string ThemeSkinsData     => Path.Combine(Application.persistentDataPath, "theme.io");
        public static string CustomizationsData => Path.Combine(Application.persistentDataPath, "appearance.io");
    }

    public struct GameOverAnimationStates
    {
        public static readonly string SuccessOneStar    = "Animate One Star";
        public static readonly string SuccessTwoStar    = "Animate Two Stars";
        public static readonly string SuccessFullStar   = "Animate Full Stars";
        public static readonly string Failure           = "Animate Failure";
    }

    public struct PrefKeys
    {
        public static readonly string UnlockedStageEasy      = "Unlocked_Stage_Easy";
        public static readonly string UnlockedStageNormal    = "Unlocked_Stage_Normal";
        public static readonly string UnlockedStageHard      = "Unlocked_Stage_Hard";
    }

    public struct Scenes
    {
        public static readonly string MainMenu = "Main Menu";
    }

    public struct Tags
    {
        public static readonly string GameManager = "Game Manager";
        public static readonly string MainCamera  = "MainCamera";
    }

    public readonly struct ColorSwatches
    {
        public static readonly Color BLUE           = new Color(r: 0.0F, g: 0.3921566F, b: 1.0F, a: 1.0F);
        public static readonly Color GREEN          = new Color(r: 0.0F, g: 0.6792453F, b: 0.30837283F, a: 1.0F);
        public static readonly Color ORANGE         = new Color(r: 1.0F, g: 0.5254902F, b: 0.1254902F, a: 1.0F);
        public static readonly Color PURPLE         = new Color(r: 0.41568628F, g: 0.2F, b: 0.99607843F, a: 1.0F);
        public static readonly Color MAGENTA        = new Color(r: 1.0F, g: 0.0F, b: 0.43137255F, a: 1.0F);
        public static readonly Color YELLOW         = new Color(r: 1.0F, g: 0.6862745F, b: 0.1254902F, a: 1.0F);
        public static readonly Color RED            = new Color(r: 0.99607843F, g: 0.13725491F, b: 0.22745098F, a: 1.0F);
        public static readonly Color DARK_PURPLE    = new Color(r: 0.3254902F, g: 0.0F, b: 1.0F, a: 1.0F);
        public static readonly Color TEAL           = new Color(r: 0.0F, g: 0.7176471F, b: 0.6F, a: 1.0F);
        public static readonly Color DARK_TEAL      = new Color(r: 0.0F, g: 0.49411765F, b: 0.41960785F, a: 1.0F);
        public static readonly Color WHITE          = new Color(r: 1, g: 1, b: 1, a: 1);
    }

    // public readonly struct RewardTypes
    // {
    //     public static readonly int Gem = 1;
    //     public static readonly int Coin = 0;
    // }
}