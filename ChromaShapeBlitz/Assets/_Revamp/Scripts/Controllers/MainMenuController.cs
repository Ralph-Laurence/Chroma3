using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Revamp
{
    public class MainMenuController : MonoBehaviour
    {
        private BackgroundMusic bgmManager;
        private LevelMenuController levelMenuController;
        [SerializeField] private Rv_BlockSkinGroupSO[] blockSkinGroups;

        [Space(10)]
        [SerializeField] private GameObject skinItemCard;
        [SerializeField] private RectTransform skinsScrollViewContent;

        private Dictionary<int, Rv_BlockSkinSO> skinsLookupTable;
        private GameSessionManager gsm;
        private UserDataHelper userDataHelper;
        private UserData userData;

        void Awake()
        {
            bgmManager          = BackgroundMusic.Instance;
            levelMenuController = LevelMenuController.Instance;
            
            gsm = GameSessionManager.Instance;
            userDataHelper = UserDataHelper.Instance;
            userData = userDataHelper.GetLoadedData();
        }

        void Start()
        {
            bgmManager.PlayMainBgm();

            PlayerCurrencyNotifier.NotifyObserver(new PlayerCurrencyEventArgs
            {
                Amount = userData.TotalCoins,
                Currency = CurrencyType.Coin,
            });

            PlayerCurrencyNotifier.NotifyObserver(new PlayerCurrencyEventArgs
            {
                Amount = userData.TotalGems,
                Currency = CurrencyType.Gem,
            });

            BuildSkinShopMenu();
        }

        public void Ev_Play()
        {
            if (levelMenuController == null)
                return;

            levelMenuController.Show();
        }

        public void Ev_Quit()
        {
            Application.Quit();
        }

        private void BuildSkinShopMenu()
        {
            skinsLookupTable = new Dictionary<int, Rv_BlockSkinSO>();
            
            foreach (var group in blockSkinGroups)
            {
                group.AddTo(skinsLookupTable);
            }

            foreach (var kvp in skinsLookupTable)
            {
                var skinId = kvp.Key;
                var skinData = kvp.Value.SkinInfo;

                Instantiate(skinItemCard, skinsScrollViewContent)
                    .TryGetComponent(out SkinShopItemCard card);

                var data = new SkinItemData
                {
                    CostCurrency = skinData.Cost,
                    ID           = skinData.Id,
                    Name         = skinData.Name,
                    PreviewImage = skinData.PreviewImage,
                    Price        = skinData.Price,
                };

                card.SetItemData(data);
            }
        }
    }

}