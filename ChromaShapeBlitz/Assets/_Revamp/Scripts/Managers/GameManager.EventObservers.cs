using UnityEngine;

namespace Revamp
{
    public partial class GameManager : MonoBehaviour
    {
        //--------------------------------------
        // Register event observers / listeners
        //--------------------------------------
        private void AttachEventObservers()
        {
            OnStageCreated.BindEvent(ObserveStageCreated);
            OnStageCompleted.BindEvent(ObserveStageComplete);
        }
        //--------------------------------------
        // Unregister event observer / listeners
        //--------------------------------------
        private void DetachEventObservers()
        {
            OnStageCreated.UnbindEvent(ObserveStageCreated);
            OnStageCompleted.UnbindEvent(ObserveStageComplete);
        }
        //--------------------------------------
        // Subscribe to events 
        //--------------------------------------

        // After a stage has been created
        private void ObserveStageCreated(StageCreatedEventArgs e)
        {
            bgm.Stop();
            bgm.SetClip(e.StageBgm);

            bgm.Play();

            stageTimer.Prepare(e.TotalStageTime, e.StagePattern);
            stageTimer.Begin();
        }

        // When a stage has been completed
        private void ObserveStageComplete(StageCompletionType stageCompletionType)
        {
            stageTimer.Stop();
            bgm.Stop();

            switch (stageCompletionType)
            {
                case StageCompletionType.Success:
                    OnGameSuccess();
                    break;

                case StageCompletionType.Failed:
                    sfx.PlayOnce(incorrectBlocksSfx);
                    OnGameFailed();
                    break;
            }
        }
    }
}