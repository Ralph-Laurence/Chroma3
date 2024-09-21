using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Revamp;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// This will be used while in-game
/// </summary>
public class PowerupsEffectManager : MonoBehaviour
{
    [SerializeField] private InGameHotBar hotbar;
    [SerializeField] private float hotbarPosX = 12.0F;
    [SerializeField] private float hotbarSlideInDuration = 0.25F;
    [SerializeField] private PowerupsIO   powerupsIO;

    [SerializeField] private HintMarker hintMarker;
    [SerializeField] private StageCamera stageCamera;

    private RectTransform hotbarRect;
    private Sprite[] countIndicatorSubSprites;
    private GameSessionManager gsm;

    private Vector2 hotbarInitialPosition;
    private Dictionary<int, InventoryItemData> activePowerups = new();
    private Dictionary<int, PowerupEffectData> powerupEffectMap = new();

    void Awake()
    {
        gsm = GameSessionManager.Instance;

        hotbar.TryGetComponent(out hotbarRect);
        
        if (hotbarRect != null)
        {
            hotbarInitialPosition = new Vector2
            (
                -(hotbarRect.rect.width + 15.0F),
                hotbarRect.anchoredPosition.y
            );
            
            hotbarRect.anchoredPosition = hotbarInitialPosition;
        }
    }

    void Start()
    {
        StartCoroutine(IEBeginLoadData());
    }

    private IEnumerator IEBeginLoadData()
    {
        var tasks = new List<Func<IEnumerator>>
        {
            IELoadSubSprites,
            IELoadPowerupsMap,
            IELoadActivePowerups,
            IEFillHotbar
        };

        for (var i = 0; i < tasks.Count; i++)
        {
            var coroutine = tasks[i];

            // yield return ensures that each coroutine fully completes before moving on to the next one. 
            // This is different from running them concurrently (in parallel) as it waits for each to finish.
            yield return StartCoroutine(coroutine());
        }
    }

    /// <summary>
    /// Load Subsprites from Addressables
    /// </summary>
    private IEnumerator IELoadSubSprites()
    {
        yield return StartCoroutine
        (
            powerupsIO.LoadCountIndicatorSubSpritesAsync((sprites) =>
            {
                countIndicatorSubSprites = sprites;
                hotbar.SetCountIndicatorsSource(countIndicatorSubSprites);

                Debug.LogWarning($"Sprites loaded with count : {countIndicatorSubSprites.Length}"); 
            })
        );
    }

    /// <summary>
    /// Load user-equipped powerups from disk
    /// </summary>
    private IEnumerator IELoadActivePowerups()
    {
        yield return StartCoroutine
        (
            powerupsIO.LoadOwnedPowerupsAssetsAsync((owned, equipped) =>
            {
                activePowerups  = equipped;
                var keys        = activePowerups.Keys.ToList();

                for (var i = 0; i < keys.Count; i++)
                {
                    var key  = keys[i];
                    var edit = activePowerups[key];

                    edit.AmountIcon = edit.CurrentAmount.MapAmountToSprite
                    (
                        countIndicatorSubSprites,
                        edit.ItemType
                    );

                    activePowerups[key] = edit;
                }

                hotbar.SetItemQueueSource(activePowerups);
            })
        );        
    }

    private IEnumerator IEFillHotbar()
    {
        if (activePowerups.Count <= 0)
            yield break;

        yield return StartCoroutine(hotbar.RePopulateHotbar());

        // Slide in the hotbar
        LeanTween.moveX(hotbarRect, hotbarPosX, hotbarSlideInDuration);
        yield return null;

        // Debug.Log("Item Queue: ");
        
        // foreach (var item in hotbar.GetItemQueue())
        // {
        //     Debug.LogWarning(item);
        // }

        // yield return null;
    }

    private IEnumerator IELoadPowerupsMap()
    {
        var powerupsLutAddress = Constants.PackedAssetAddresses.PowerupsLUT;
        var handle = Addressables.LoadAssetAsync<PowerupsAssetGroup>(powerupsLutAddress);

        yield return handle;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogWarning("Failed to load powerups");
            yield break;
        }

        var powerupAssetsList = handle.Result.Powerups;

        for (var i = 0; i < powerupAssetsList.Count; i++)
        {
            var data = powerupAssetsList[i];
            powerupEffectMap.Add(data.Id, new PowerupEffectData
            {
                PowerupId   = data.Id,
                Category    = data.PowerupCategory,
                EffectValue = data.EffectValue
            });
        }
    }

    #region EVENT_OBSERVERS

    private void ObserveGameManagerActionEvents(GameManagerActionEvents action)
    {
        if (action == GameManagerActionEvents.Retry)
        {
            Debug.LogWarning("Retryed");
            hotbarRect.anchoredPosition = hotbarInitialPosition;
        }
    }

    private void ObserveStageCreated(StageCreatedEventArgs e)
    {
        LeanTween.moveX(hotbarRect, hotbarPosX, hotbarSlideInDuration);
    }

    private void ObserveHotbarSlotSelected(int slotIndex)
    {
        var powerupID  = hotbar.GetItemIDAtSlot(slotIndex);
        var effectData = powerupEffectMap[powerupID];
        var effectSlot = hotbar.GetSlot(slotIndex);

        // Apply | Execute the powerup's effect
        HotbarPowerupEffectNotifier.NotifyObserver(effectSlot, effectData);
    }

    void OnEnable()
    {
        GameManagerEventNotifier.BindEvent(ObserveGameManagerActionEvents);
        OnStageCreated.BindEvent(ObserveStageCreated);
        HotbarSlotSelectedNotifier.BindObserver(ObserveHotbarSlotSelected);
        PowerupEffectAppliedNotifier.BindObserver(ObservePowerupApplied);
        HintMarkerNotifier.BindObserver(ObserveHintMarker);
    }
    void OnDisable()
    {
        GameManagerEventNotifier.UnbindEvent(ObserveGameManagerActionEvents);
        OnStageCreated.BindEvent(ObserveStageCreated);
        HotbarSlotSelectedNotifier.UnbindObserver(ObserveHotbarSlotSelected);
        PowerupEffectAppliedNotifier.UnbindObserver(ObservePowerupApplied);
        HintMarkerNotifier.UnbindObserver(ObserveHintMarker);
    }
    #endregion EVENT_OBSERVERS

    private IEnumerator DecreaseQuantity(int slotIndex, PowerupEffectData powerupEffectData)
    {
        var userData = gsm.UserSessionData;
        var owned    = userData.Inventory.OwnedPowerups;
        var equip    = userData.Inventory.EquippedPowerupIds;
        
        // Note: When you modify a struct in C#, you're working with a COPY of the original data.
        // Modifying an item property will not affect the original struct in the list.
        // To apply the modifications, we need to assign the modified struct back to the list.

        for (var i = 0; i < owned.Count; i++)
        {
            var item = owned[i];

            // if the desired item was found, do the processing then leave the loop
            if (item.PowerupID == powerupEffectData.PowerupId)
            {
                item.CurrentAmount--;

                if (item.CurrentAmount > 0)
                {
                    owned[i] = item;
                    hotbar.UpdateSlotCount(slotIndex, item.CurrentAmount);
                }

                // The item has ran out of quantity, dequeue it.
                else if (item.CurrentAmount <= 0)
                {
                    owned.RemoveAt(i);

                    if (equip.Contains(item.PowerupID))
                        equip.Remove(item.PowerupID);
                        // equip.RemoveAll(i => i == item.PowerupID)

                    hotbar.DequeueItem(slotIndex);
                }
                break;
            }

            yield return null;
        }
        
        // Write the changes to file
        yield return StartCoroutine(UserDataHelper.Instance.SaveUserData(userData, null));
    }

    private void ObservePowerupApplied(HotBarSlot sender, PowerupEffectData powerupEffectData)
    {
        // Decrease quantity from the inventory
        StartCoroutine(DecreaseQuantity(sender.SlotIndex, powerupEffectData));
    }

    /// <summary>
    /// We can only show hints when the user havent touched any of the tiles yet
    /// </summary>
    // private IEnumerator IEBeginShowHints()
    // {
    //     // Prevent any interactions while the transition plays
    //     InteractionBlockerNotifier.NotifyObserver();

    //     shouldStickToBottom = false;

    //     stageCamera.ViewFromAbove
    //     (
    //         followStageYRotation: transform.localEulerAngles.y,
    //         onDone: () => StartCoroutine(IEAnimateShowHints())
    //     );

    //     yield return null;
    // }

    private void ObserveHintMarker(StageVariant stageVariant) => StartCoroutine(IEBeginShowHint(stageVariant));

    private IEnumerator IEBeginShowHint(StageVariant stageVariant)
    {
        // Prevent any interactions while the transition plays
        InteractionBlockerNotifier.NotifyObserver(true);

        // Prevent the stage variant from being shown at the bottom screen 
        stageVariant.SetStickToBottom(false);

        var stageRotationY = stageVariant.transform.eulerAngles.y;

        // Rotate and move the camera at top-down position
        yield return StartCoroutine(stageCamera.IEViewFromAbove
        (
            followStageYRotation: stageRotationY
        ));

        Debug.LogWarning($"Stage Rot Y => {stageRotationY}");

        // Make the hint marker visible
        hintMarker.gameObject.SetActive(true);

        // Rotate the pointer hand to be the same rotation as the stage variant
        hintMarker.transform.localEulerAngles = new Vector3(90.0F, stageRotationY);

        // Assign the targets for hint
        var targets = new List<GameObject>();

        for (var i = 0; i < stageVariant.SequenceSet.Count; i++)
        {
            targets.Add(stageVariant.SequenceSet[i].gameObject);
        }

        hintMarker.SetTargets(targets);

        yield return hintMarker.ShowHints();
        yield return new WaitForSeconds(0.5F);

        // Bring back to initial view angle before hints were shown.
        yield return StartCoroutine(stageCamera.IEUnviewFromAbove());
        
        hintMarker.gameObject.SetActive(false);
        stageVariant.SetStickToBottom(true);

        // Allow interactions after the transition plays
        InteractionBlockerNotifier.NotifyObserver(false);
    }
}