using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public partial class StageFabricator : MonoBehaviour
{
    private string SaveFolderBase => $"Assets/Resources/Stages";

    [Space(10)] [Header("Filler Pad Visuals")]
    [SerializeField] private GameObject sequenceTriggerPadTemplate;
    [SerializeField] private GameObject fillerPadVisualBlue;
    [SerializeField] private GameObject fillerPadVisualGreen;
    [SerializeField] private GameObject fillerPadVisualMagenta;
    [SerializeField] private GameObject fillerPadVisualOrange;
    [SerializeField] private GameObject fillerPadVisualPurple;
    [SerializeField] private GameObject fillerPadVisualYellow;

    [Space(10)] [Header("Save Prompt")]
    [SerializeField] private GameObject promptOverlay;
    [SerializeField] private GameObject savePromptDialog;

    [Space(10)] [Header("Overwrite Protection")]
    [SerializeField] private GameObject overwritePromptDialog;
    [SerializeField] private Text overwritePromptText;

    private void SaveToPrefab()
    {
        stageTemplate.TryGetComponent(out StageVariant srcVariantData);

        var objName  = $"{srcVariantData.VariantDifficulty}_{srcVariantData.VariantNumber}_{srcVariantData.VariantName}";
        var savePath = $"{SaveFolderBase}/{srcVariantData.VariantDifficulty}/Stage_{srcVariantData.VariantNumber}";

        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        var fileName = $"{savePath}/{objName}.prefab";

        // Prevent accidental overwrite
        if (File.Exists(fileName))
        {
            WarnOverwrite(objName);
            return;
        }

        savePromptDialog.SetActive(true);

        ConfirmSave(objName, srcVariantData, fileName);
    }

    public void ConfirmSave(string objName, StageVariant srcVariantData, string fileName)
    {
        var blockClonesMap = new Dictionary<string, Block>();

        var newStageVariantRoot  = new GameObject(objName);
        var blockGridRootClone   = new GameObject("Block Grid");
        var triggersRootClone    = new GameObject("Sequence Triggers");

        blockGridRootClone.transform.SetParent(newStageVariantRoot.transform);
        triggersRootClone.transform.SetParent(newStageVariantRoot.transform);
        //
        //=======================================
        // Copy the root's component data
        //=======================================
        //
        var newVariantData = newStageVariantRoot.AddComponent(typeof(StageVariant)) as StageVariant;
        
        newVariantData.IncorrectBlockMaterial = IncorrectBlockMat;

        // Level Design
        newVariantData.AutoFindCamera       = srcVariantData.AutoFindCamera;
        newVariantData.StageOffset          = srcVariantData.StageOffset;
        newVariantData.ViewAngleLeft        = srcVariantData.ViewAngleLeft;
        newVariantData.ViewAngleRight       = srcVariantData.ViewAngleRight;
        newVariantData.MinCameraOrthoSize   = srcVariantData.MinCameraOrthoSize;
        newVariantData.MaxCameraOrthoSize   = srcVariantData.MaxCameraOrthoSize;
        newVariantData.BgmClip              = srcVariantData.BgmClip;
        
        // Mechanics
        newVariantData.PatternObjective     = srcVariantData.PatternObjective;
        newVariantData.TotalStageTime       = srcVariantData.TotalStageTime;
        newVariantData.MinStageTime         = srcVariantData.MinStageTime;
        newVariantData.MaxStageTime         = srcVariantData.MaxStageTime;
        newVariantData.RewardType           = srcVariantData.RewardType;
        newVariantData.TotalReward          = srcVariantData.TotalReward;

        newVariantData.VariantName          = srcVariantData.VariantName;
        newVariantData.VariantNumber        = srcVariantData.VariantNumber;
        newVariantData.VariantDifficulty    = srcVariantData.VariantDifficulty;
        //
        //=======================================
        // Copy the block grid's rows and blocks
        //=======================================
        //
        foreach (Transform row in stageTemplate.transform.Find("Block Grid"))
        {
            // Skip empty rows
            if (row.childCount == 0)
                continue;

            var rowRoot = new GameObject(row.name);

            foreach (Transform col in row)
            {
                col.TryGetComponent(out Block block);

                var blockType = block.DarkerFill ? darkBlock : lightBlock;
                var blockObj = Instantiate
                (
                    blockType, 
                    block.transform.localPosition, 
                    block.transform.localRotation, 
                    rowRoot.transform
                );

                blockObj.TryGetComponent(out Block blockNew);
                
                blockNew.name               = block.name;
                blockNew.tag                = block.tag;

                blockNew.DarkerFill         = block.DarkerFill;
                blockNew.IsDestinationBlock = block.IsDestinationBlock;
                blockNew.SetColor(ColorSwatches.None);
                blockNew.SetRequiredColor(block.RequiredColor);

                blockClonesMap.Add(block.name, blockNew);
            }

            rowRoot.transform.SetParent(blockGridRootClone.transform, false);
        }
        //
        //=======================================
        // Copy the sequence filler triggers
        //=======================================
        //
        foreach (Transform trigger in stageTemplate.transform.Find("Sequence Triggers"))
        {
            trigger.TryGetComponent(out BlockSequenceController srcController);
            GameObject fillerPadVisuals = null;

            switch (srcController.FillColorValue)
            {
                case ColorSwatches.Blue:    fillerPadVisuals = fillerPadVisualBlue;    break;
                case ColorSwatches.Green:   fillerPadVisuals = fillerPadVisualGreen;   break;
                case ColorSwatches.Magenta: fillerPadVisuals = fillerPadVisualMagenta; break;
                case ColorSwatches.Orange:  fillerPadVisuals = fillerPadVisualOrange;  break;
                case ColorSwatches.Purple:  fillerPadVisuals = fillerPadVisualPurple;  break;
                case ColorSwatches.Yellow:  fillerPadVisuals = fillerPadVisualYellow;  break;
            }

            var newSequenceTriggerPad = Instantiate
            (
                sequenceTriggerPadTemplate,
                srcController.transform.localPosition,
                srcController.transform.localRotation,
                triggersRootClone.transform
            );

            newSequenceTriggerPad.name = srcController.name;

            // The sequence trigger's visuals, a child of the sequence trigger's root object
            var newFillerVisuals = Instantiate(fillerPadVisuals);
            newFillerVisuals.transform.SetParent(newSequenceTriggerPad.transform, false);
            newFillerVisuals.name = "Visual";
            
            // Attach a new block sequence controller then copy the original sequences
            newSequenceTriggerPad.TryGetComponent(out BlockSequenceController newController);
            
            newController.FillColorValue = srcController.FillColorValue;
            newController.FillMaterial   = srcController.FillMaterial;
            newController.Destination    = blockClonesMap[srcController.Destination.name];
            newController.PadVisuals     = newFillerVisuals;
            
            srcController.BlockSequence.ForEach(src => newController.BlockSequence.Add( blockClonesMap[src.name] ));

            newVariantData.SequenceSet.Add(newController);
        }

        newStageVariantRoot.transform.SetPositionAndRotation
        (
            stageTemplate.transform.position, 
            stageTemplate.transform.rotation
        );
 #if UNITY_EDITOR
        // Write the prefab into disk
        PrefabUtility.SaveAsPrefabAssetAndConnect(newStageVariantRoot, fileName, InteractionMode.AutomatedAction);
        DestroyImmediate(newStageVariantRoot);

        // Load the prefab asset then disable edit mode on its sequence controllers.
        // Also, reset its Y position

        // Get the prefab asset
        var prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(fileName);

        // Modify the prefab asset
        prefabAsset.TryGetComponent(out StageVariant stageVariant);
        stageVariant.SequenceSet.ForEach(sq => sq.IsEditMode = false);

        var prefabAssetPos = stageVariant.transform.position;
        prefabAssetPos.y = 0.0F;
        prefabAsset.transform.position = prefabAssetPos;

        // Save the changes
        PrefabUtility.SavePrefabAsset(prefabAsset);
#endif
        promptOverlay.SetActive(false);
    }

    private void WarnOverwrite(string objName)
    {
        overwritePromptText.text = $"Save failed on {objName} to prevent accidental overwrite as the prefab already exists.";
        overwritePromptDialog.SetActive(true);
        promptOverlay.SetActive(true);
    }
}