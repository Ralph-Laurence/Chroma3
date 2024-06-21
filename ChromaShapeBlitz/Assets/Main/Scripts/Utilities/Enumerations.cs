using UnityEngine;

public enum ColorSwatches
{
    None,
    Yellow,
    Blue,
    Magenta,
    Purple,
    Green,
    Orange
}

public enum CurrencyType
{
    Coin,
    Gem
}

public enum RewardType
{
    Coin,
    Gem,
    // Medal,
    // Throphy
}

public enum GameResults
{
    Passed,
    Failed
}

public enum BackgroundParallaxConstraints
{
    Normal,
    Reversed,
    ReverseX,
    ReverseY
}

public enum CustomizeTabIdentifiers
{
    //None,
    Skins,
    Themes
}

public enum BackgroundTypes
{
    Parallax,
    Gradient,
    Static
}

public enum MoveDirections
{
    None,
    Forward,
    Backward,
    Left,
    Right
}

public enum IsoCamViewingAngles
{
    Middle,
    Left,
    Right
}

public enum LinearPropEntityDirections
{
    X,
    Z
}

public enum LinearBounds
{
    ControlledByScript,
    XAxis,
    ZAxis
}

public enum GameLevels
{
    Easy,
    Normal,
    Hard
}

//=============================================================//
// REVAMPED
//=============================================================//

public enum RewardTypes
{
    Coins,
    Gems
}

public enum LevelDifficulties
{
    Easy,
    Normal,
    Hard
}

public enum SoundToggleType
{
    BGM,
    SFX
}

public enum UXButtonClickTypes
{
    Positive,
    Negative
}

public enum GameOverTypes
{
    Success,
    Fail
}

/// <summary>
/// Used to identify common GameManager actions
/// </summary>
public enum GameManagerActionEvents
{
    None,
    Pause,
    Resume,
    Retry,
    ExitToMenu,
    NextStage
}

/// <summary>
/// Used when a stage variant was done validating its sequence.
/// 
/// <list type="bullet">
///     <item>
///         <term>Success</term>
///         <description>Block sequences are all correct</description>
///     </item>
///     <item>
///         <term>Failed</term>
///         <description>There are incorrect sequences</description>
///     </item>
/// </list>
/// </summary>
public enum StageCompletionType
{
    Success,
    Failed
}

/// <summary>
/// <para><b>These are mainly used by Event Notifier</b></para>
/// 
/// Game Manager states may only have two values:
/// 
/// <list type="bullet">
///     <item>
///         <term>Active</term>
///         <description>The game manager is not stopped</description>
///     </item>
///     <item>
///         <term>Stopped</term>
///         <description>The game manager is terminated</description>
///     </item>
/// </list>
/// </summary>
public enum GameManagerStates
{
    Active,
    Stopped
}