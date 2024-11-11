using UnityEngine;
using UnityEngine.SceneManagement;

namespace Revamp
{
    /// <summary>
    /// The <b>GameManager.EventObservers</b> tied to the GameManager object
    /// will always belong to the GameManager; No other objects can have these.
    /// </summary>
    public partial class GameManager : MonoBehaviour
    {
        //--------------------------------------
        // Register event observers / listeners
        //--------------------------------------
        private void AttachEventObservers()
        {
            OnStageCreated.BindObserver(ObserveStageCreated);
            OnStageCompleted.BindEvent(ObserveStageComplete);
            GameManagerEventNotifier.BindObserver(ObserveGameManagerActionEvents);
            GameManagerStateNotifier.BindEvent(ObserveGameManagerState);
        }
        //--------------------------------------
        // Unregister event observer / listeners
        //--------------------------------------
        private void DetachEventObservers()
        {
            OnStageCreated.UnbindObserver(ObserveStageCreated);
            OnStageCompleted.UnbindEvent(ObserveStageComplete);
            GameManagerEventNotifier.UnbindObserver(ObserveGameManagerActionEvents);
            GameManagerStateNotifier.UnbindEvent(ObserveGameManagerState);
        }
        //--------------------------------------
        // Subscribe to events 
        //--------------------------------------

        private void ObserveGameManagerState(GameManagerStates state) => currentState = state;

        private void ObserveStageCreated(StageCreatedEventArgs e)
        {
            bgm.Stop();
            bgm.SetClip(e.StageBgm);
            bgm.ResetVolume();
            bgm.Play();
            
            var shouldFadeOutPattern = e.StageLevel == LevelDifficulties.Hard;

            stageTimer.Prepare(e.TotalStageTime, e.StagePattern);

            gsm.SelectedStageMinTime = e.MinStageTime;
            gsm.SelectedStageMaxTime = e.MaxStageTime;
            
            var stageTitle  = $"{e.StageLevel} - {e.StageNumber:D2}";
            var rewardText  = e.TotalReward.ToRewardText(e.RewardType);
            var minTimeText = $"Finish under <color=#81FF21>{e.MinStageTime} secs";
            var maxTimeText = $"Finish under <color=#FF9533>{e.MaxStageTime} secs";

            stageTitleTexts.SetTextMultiple(stageTitle);
            rewardsText.SetTextMultiple(rewardText);
            objectiveTextsMinTime.SetTextMultiple(minTimeText);
            objectiveTextsMaxTime.SetTextMultiple(maxTimeText);

            if (gsm.UserSessionData.RemainingEMPUsage > 0)
            {
                stageTimer.SetBrownOut(true);
                
                // Decrease the remaining EMP usages
                gsm.UserSessionData.RemainingEMPUsage--;

                // Write the changes to file
                StartCoroutine(UserDataHelper.Instance.SaveUserData(gsm.UserSessionData, null));

                Debug.Log($"EMP Count : {gsm.UserSessionData.RemainingEMPUsage}");
            }
            else
            {
                stageTimer.SetBrownOut(false);
            }

            stageTimer.Begin(shouldFadeOutPattern);
        }

        private void ObserveStageComplete(StageCompletionType stageCompletionType)
        {
            BreakExecution();

            switch (stageCompletionType)
            {
                case StageCompletionType.Success:
                    OnGameSuccess();
                    break;

                case StageCompletionType.Failed:
                    OnGameFailed();
                    break;
            }
        }

        private void ObserveGameManagerActionEvents(GameManagerActionEvents eventType)
        {
            switch (eventType)
            {
                case GameManagerActionEvents.Pause      : GMEV_Pause();          break;
                case GameManagerActionEvents.Resume     : GMEV_Resume();         break;
                case GameManagerActionEvents.Retry      : GMEV_Retry();          break;
                case GameManagerActionEvents.VisitShop  : GMEV_VisitShop();      break;
                case GameManagerActionEvents.NextStage  : GMAEV_NextStage();     break;
                case GameManagerActionEvents.ExitToMenu : GMEV_ExitToMainMenu(); break;
            }
        }
    }
}