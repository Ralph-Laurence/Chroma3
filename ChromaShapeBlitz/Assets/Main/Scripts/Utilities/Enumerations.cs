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

public enum MoveDirections
{
    None,
    Forward,
    Backward,
    Left,
    Right
}


public enum LinearBounds
{
    ControlledByScript,
    XAxis,
    ZAxis
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
    NextStage,
    VisitShop
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

public enum PowerupType
{
    /// <summary>
    /// <list type="bullet">
    ///     <item>
    ///         <description>Once enabled, the effect is permanent</description>
    ///     </item>
    ///     <item>
    ///         <description>Max. 1 item</description>
    ///     </item>
    /// </list>
    /// </summary>
    LIFE_TIME_USE,

    /// <summary>
    /// <list type="bullet">
    ///     <item>
    ///         <description>Single-Use item that gets depleted upon activation.</description>
    ///     </item>
    ///     <item>
    ///         <description>Max. 1 item</description>
    ///     </item>
    /// </list>
    /// </summary>
    CONSUMABLE_SINGLE,

    /// <summary>
    /// <list type="bullet">
    ///     <item>
    ///         <description>Single-Use items that get depleted upon activation.</description>
    ///     </item>
    ///     <item>
    ///         <description>Can add more items</description>
    ///     </item>
    /// </list>
    /// </summary>
    CONSUMABLE_STACKABLE
}

public enum PowerupActivation
{
    /// <summary>
    /// Powerup gets activated immediately upon purchase
    /// </summary>
    INSTANT,

    /// <summary>
    /// Requires the user to activate the powerup while in game
    /// </summary>
    MANUAL
}