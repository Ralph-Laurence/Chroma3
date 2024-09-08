using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This will be used while in-game
/// </summary>
public class PowerupsEffectManager : MonoBehaviour
{
    [SerializeField] private InGameHotBar hotbar;
    [SerializeField] private PowerupsIO   powerupsIO;
    
    private Sprite[] countIndicatorSubSprites;
    private GameSessionManager gsm;

    void Awake()
    {
        gsm = GameSessionManager.Instance;
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
            IELoadActivePowerups,
            hotbar.RePopulateHotbar
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
                var dataSource  = equipped;
                var keys        = dataSource.Keys.ToList();

                for (var i = 0; i < keys.Count; i++)
                {
                    var key  = keys[i];
                    var edit = dataSource[key];

                    edit.AmountIcon = edit.CurrentAmount.MapAmountToSprite
                    (
                        countIndicatorSubSprites,
                        edit.ItemType
                    );

                    dataSource[key] = edit;
                }

                hotbar.SetItemQueueSource(dataSource);
            })
        );        
    }
}