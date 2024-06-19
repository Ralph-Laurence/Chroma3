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

        void Awake()
        {
            bgmManager          = BackgroundMusic.Instance;
            levelMenuController = LevelMenuController.Instance;
        }

        void Start()
        {
            bgmManager.PlayMainBgm();
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
    }

}