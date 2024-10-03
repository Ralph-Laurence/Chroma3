using UnityEngine;
using UnityEngine.SceneManagement;

namespace Revamp
{
    public partial class GameManager : MonoBehaviour
    {
        private void OnCutSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name.Equals(Constants.Scenes.CutSceneEMPAttack))
                bgm.Pause();
        }

        private void OnCutSceneUnLoaded(Scene scene)
        {
            if (scene.name.Equals(Constants.Scenes.CutSceneEMPAttack))
                bgm.Resume();
        }
    }
}