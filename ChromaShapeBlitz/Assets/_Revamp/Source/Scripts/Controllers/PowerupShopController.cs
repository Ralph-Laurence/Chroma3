using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupShopController : MonoBehaviour
{
    [SerializeField] private List<PowerupsAsset> powerupAssets;
    [SerializeField] private GameObject itemCard;

    [SerializeField] private RectTransform scrollRectContent;
    [SerializeField] private BuyBackgroundPrompt buyConfirmPrompt;
    // [SerializeField] private ScrollRect scrollRect;

    private Dictionary<int, PowerupsAsset> powerupsLookUp;
    private ToggleGroup toggleGroup;

    private GameSessionManager gsm;
    private UserDataHelper userDataHelper;

    private BackgroundShopItemCard m_activeItemCard;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
