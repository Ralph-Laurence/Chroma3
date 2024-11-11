using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlotMachineSpriteMapping", menuName = "Scriptable Objects/SlotMachineSpriteMapping")]
public class SlotMachineItemSpriteMapping : ScriptableObject
{
    [Space(10)]
    [Header("Common Items")]
    public Sprite None1;
    public Sprite CoinsPile;
    public Sprite Shit;
    public Sprite None2;

    [Space(10)]
    [Header("Less Common Items")]
    public Sprite Adrenaline;
    public Sprite Endurance;

    [Space(10)]
    [Header("Rare Items")]
    public Sprite GemsPile;
    public Sprite CoinsBag;
    public Sprite Idea;

    [Space(10)]
    [Header("Epic Items")]
    public Sprite CoinsBucket;
    public Sprite GemsBag;

    [Space(10)]
    [Header("Legendary Items")]
    public Sprite Wizard;
    public Sprite Recon;
    public Sprite GemsBucket;

    // For Randomization
    public Dictionary<string, Sprite> SpriteMap { get; private set; }

    public void Initialize()
    {
        SpriteMap = new Dictionary<string, Sprite>
        {
            { "None1",      None1       },
            { "None2",      None2       },
            { "CoinsPile",  CoinsPile   },
            { "Shit",       Shit        },
            { "Adrenaline", Adrenaline  },
            { "Endurance",  Endurance   },
            { "GemsPile",   GemsPile    },
            { "CoinsBag",   CoinsBag    },
            { "Idea",       Idea        },
            { "CoinsBucket", CoinsBucket },
            { "GemsBag",    GemsBag     },
            { "Wizard",     Wizard      },
            { "Recon",      Recon       },
            { "GemsBucket", GemsBucket  },
        };
    }

    public Sprite Select(string spriteKey) => SpriteMap[spriteKey];

    /// <summary>
    /// Select three random sprites
    /// </summary>
    public Sprite[] SelectRandom()
    {
        var availableSprites = new List<Sprite>(SpriteMap.Values);
        availableSprites.RemoveAll(sprite => sprite == null);

        // Generate the first two random sprites
        Sprite firstSprite = availableSprites[Random.Range(0, availableSprites.Count)];
        Sprite secondSprite = firstSprite;

        // Ensure the second sprite can be either the same or different
        while (secondSprite == firstSprite)
        {
            secondSprite = availableSprites[Random.Range(0, availableSprites.Count)];
        }

        // Generate the third sprite, allowing for up to two repeats of the same sprite
        Sprite thirdSprite = availableSprites[Random.Range(0, availableSprites.Count)];

        // If all three are the same, select a unique sprite for the third one
        if (firstSprite == secondSprite && secondSprite == thirdSprite)
        {
            // Find a sprite that is unique compared to the first two
            do
            {
                thirdSprite = availableSprites[Random.Range(0, availableSprites.Count)];
            } while (thirdSprite == firstSprite);
        }

        return new Sprite[] { firstSprite, secondSprite, thirdSprite };
    }

}
