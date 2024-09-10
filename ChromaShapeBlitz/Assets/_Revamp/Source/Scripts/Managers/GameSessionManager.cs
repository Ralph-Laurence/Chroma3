using System.Collections.Generic;
using UnityEngine;

public struct BlockSkinsSession
{
    public Material BlueSkinMat;
    public Material GreenSkinMat;
    public Material MagentaSkinMat;
    public Material OrangeSkinMat;
    public Material PurpleSkinMat;
    public Material YellowSkinMat;
}

public class GameSessionManager
{
    #region SINGLETON

    public static GameSessionManager instance;

    public static GameSessionManager Instance
    {
        get
        {
            instance ??= new GameSessionManager();
            return instance;
        }
    }
    #endregion SINGLETON

    #region GAME_LOGIC
    public BlockSkinsSession BlockSkinMaterialsInUse;

    public LevelDifficulties SelectedDifficulty;
    public int SelectedStageNumber;
    public int LastRandomSpawnIndex;
    public int SelectedStageMaxTime;
    public int SelectedStageMinTime;

    public UserData UserSessionData;

    public bool IsVisitShopOnGameOver { get; set; }

    private bool m_inventoryPageNeedsReload;

    public void ClearSession()
    {
        SelectedDifficulty   = default;
        SelectedStageNumber  = default;
        LastRandomSpawnIndex = default;
        SelectedStageMaxTime = default;
        SelectedStageMinTime = default;
    }

    /// <summary>
    /// Updates the in-session materials. Any changes aren't saved to disk.
    /// </summary>
    public void SetActiveBlockSkinMaterial(ColorSwatches color, Material material)
    {
        switch (color)
        {
            case ColorSwatches.Blue:    BlockSkinMaterialsInUse.BlueSkinMat         = material; break;
            case ColorSwatches.Green:   BlockSkinMaterialsInUse.GreenSkinMat        = material; break;
            case ColorSwatches.Magenta: BlockSkinMaterialsInUse.MagentaSkinMat      = material; break;
            case ColorSwatches.Orange:  BlockSkinMaterialsInUse.OrangeSkinMat       = material; break;
            case ColorSwatches.Purple:  BlockSkinMaterialsInUse.PurpleSkinMat       = material; break;
            case ColorSwatches.Yellow:  BlockSkinMaterialsInUse.YellowSkinMat       = material; break;
        }
    }

    /// <summary>
    /// Should the inventory page needs reload ... ?
    /// This should be used only every after purchasing a purchase.
    /// </summary>
    /// <param name="needsReload"></param>
    public void SetInventoryPageNeedsReload(bool needsReload) => m_inventoryPageNeedsReload = needsReload;
    public bool IsInventoryPageNeedsReload => m_inventoryPageNeedsReload;

    #endregion GAME_LOGIC

    #region FOR_POWERUP_EFFECTS

    public Material BLOCK_MAT_LIGHT {private set; get;}
    public Material BLOCK_MAT_DARK  {private set; get;}

    /// <summary>
    /// We need to save references to these default block materials,
    /// as they will be used for powerup effects such as Block Reveal.
    /// 
    /// Call this once from the Bootstrapper
    /// </summary>
    public void CacheInitialBlockMats(Material light, Material dark)
    {
        BLOCK_MAT_LIGHT = light;
        BLOCK_MAT_DARK  = dark;
    }

    #endregion FOR_POWERUP_EFFECTS
}