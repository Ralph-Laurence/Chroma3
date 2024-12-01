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
    Permanent,

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
    ConsumableSingle,

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
    ConsumableStackable
}

public enum PowerupCategories
{ 
    GenericItem,        // Unassigned
    StageSolver,        // Idea, Wizard, GrandMaster
    TimerPause,         // Adrenaline, Flux
    TimerIncrease,      // Continuity, Stretch, Endurance
    PatternReveal,      // Glance, Peek, Recon
    SpecialVision,      // Visor, XRay
    FillRatePerk,       // Flash, Rush, Sprint, Turbo, Blitz
    CurrencyPerk        // Looter, Buccaneer, 
}

public enum PowerupActivation
{
    /// <summary>
    /// Powerup gets activated immediately upon purchase
    /// </summary>
    Instant,

    /// <summary>
    /// Requires the user to activate the powerup while in game
    /// </summary>
    Manual
}

public enum PowerupItemCardColor
{
    Blue,
    Green,
    Pink,
    Orange,
    Purple
}

public enum InventoryItemType
{
    Unset,
    ReadOnly,
    Consumable
}

public enum AssetLoaderReleaseBehaviours
{
    OnSceneUnload,
    ScriptDriven
}

public enum TutorialSteps
{
    STEP1_BASICS                = 1,
    STEP2_BLOCK_SKIN_PURCHASE   = 2,
    STEP3_BLOCK_SKIN_USAGE      = 3,
    STEP4_BACKGROUND_PURCHASE   = 4,
    STEP5_BACKGROUND_USAGE      = 5,
    STEP6_POWERUP_PURCHASE      = 6,
    STEP7_EQUIP_INVENTORY       = 7,
    STEP8_USE_POWERUP_IN_GAME   = 8,
    STEP9_GAMEPLAY_MECHANICS    = 9,
    TUTORIALS_COMPLETE          = 100
}

public enum DailyGiftTypes
{
    Coin,
    Gem,
    RandomPowerup,
    MegaPack
}