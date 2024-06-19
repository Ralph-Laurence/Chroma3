using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class StageFabricator : MonoBehaviour
{
    private bool _showOnlyTriggers;

    //
    //========================================================
    // :::: ALL  UI EVENT METHODS ARE PREFIXED WITH "Ev_" ::::
    //========================================================
    //
    public void Ev_ResetMechanics()
    {
        levelDifficultySelect.value = 0;
        levelDifficultySelect.RefreshShownValue();

        levelNumberSelect.value = 0;
        levelNumberSelect.RefreshShownValue();

        levelVariantSelect.value = 0;
        levelVariantSelect.RefreshShownValue();

        inputTotalStageTime.text = string.Empty;
        inputMinStageTime.text = string.Empty;
        inputMaxStageTime.text = string.Empty;

        inputTotalReward.text = string.Empty;
        rewardTypeSelect.value = 0;
        rewardTypeSelect.RefreshShownValue();
    }

    public void Ev_ResetLevelDesign()
    {
        gridDimensionsSelect.value = 0;
        gridDimensionsSelect.RefreshShownValue();
           
        bgmSelect.value = 0;
        bgmSelect.RefreshShownValue();

        inputStagePosX      .text = "0";
        inputStagePosY      .text = "0";
        inputMinOrtho       .text = "5";
        inputMaxOrtho       .text = "7.55";
        inputIsoCamTiltX    .text = "50";
        inputStageTiltY     .text = "0";
        inputStageViewLeft  .text = "-60";
        inputStageViewRight .text = "60";
    }

    public void Ev_ResetParams()
    {
        ResetStageTemplate();
        ResetFabricatorUI();
        ClearFillTriggers();
    }

    public void Ev_PlayBgmPreview()
    {
        Ev_StopBgmPreview();
        audioPlayer.clip = GetSelectedBgm();
        audioPlayer.Play();
    }

    public void Ev_StopBgmPreview()
    {
        if (audioPlayer.isPlaying)
            audioPlayer.Stop();

        audioPlayer.clip = null;
    }
    
    public void Ev_GenerateGrid()
    {
        var selectedDimension = gridDimensionsSelect.options[gridDimensionsSelect.value].text;
        var gridDimension     = gridDimensions[selectedDimension];

        GenerateGrid(gridDimension);
    }

    public void Ev_ShowOnlyTriggers()
    {
        _showOnlyTriggers = !_showOnlyTriggers;

        if (_showOnlyTriggers)
        {
            blockGridsParent.gameObject.SetActive(false);
            showOnlyTriggersButton.image.color = ColorSwatches.Orange.ToUnityColor();
        }
        else
        {
            blockGridsParent.gameObject.SetActive(true);
            showOnlyTriggersButton.image.color = ColorSwatches.Blue.ToUnityColor();
        }
    }

    public void Ev_SaveAsPrefab() => SaveToPrefab();

    public void Ev_OnSelectBgm(TMP_Dropdown sender) => Ev_StopBgmPreview();
    public void Ev_ApplyLevelDesignParams() => ApplyStageTemplateParams();

    public void Ev_NewFabrication() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void Ev_BeginBlockPaintClicked() => HandleBeginBlockPaintButtonClicked();
    public void Ev_BeginEraseButtonClicked() => HandleEraseBlockButtonClicked();
    public void Ev_CloseBlockPaintClicked() => HandleEndBlockPaintButtonClicked();
}