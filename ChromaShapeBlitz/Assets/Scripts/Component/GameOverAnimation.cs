using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverAnimatableValues
{
    public GameResults GameResult    { get; set; }
    public RewardType  RewardType    { get; set; }
    public bool RewardAlreadyClaimed { get; set; }
    public int  RewardAmount         { get; set; }
    public int  TotalStars           { get; set; }
    public int  TotalPlayTime        { get; set; }
}

public class GameOverAnimation : MonoBehaviour
{
    [SerializeField] private GameObject confettiObj;
    [SerializeField] private GameObject playTimeText;
    [SerializeField] private GameObject rewardsText;

    private TextMeshProUGUI             txtTotalPlayTime;
    private TextMeshProUGUI             txtRewards;
    private GameOverAnimatableValues    animatableValues;

    private ParticleSystem confetti;
    private Animator       animator;

    private void Start() => InitializeComponents();

    private void OnEnable() => InitializeComponents();

    private void InitializeComponents()
    {
        TryGetComponent(out animator);

        confettiObj.TryGetComponent(out confetti);
        playTimeText.TryGetComponent(out txtTotalPlayTime);

        rewardsText.TryGetComponent(out txtRewards);
    }

    public void SetProperties(GameOverAnimatableValues values) => animatableValues = values;

    public void BeginAnimation()
    {
        if (animatableValues.GameResult == GameResults.Passed)
        {
            string coinReward()
            {
                var rewardClaimed = animatableValues.RewardAlreadyClaimed;

                if (rewardClaimed || (!rewardClaimed && animatableValues.TotalStars != 3))
                    return $"<style=\"Reward Silver Coin\">{animatableValues.RewardAmount}";

                return $"<style=\"Reward Gold Coin\">{animatableValues.RewardAmount}";
            }

            var rewardText = new Dictionary<RewardType, string>
            {
                { RewardType.Coin, coinReward() },
                { RewardType.Gem , $"<style=\"Reward Gems\">{animatableValues.RewardAmount}" }
            };

            txtRewards.text = rewardText[animatableValues.RewardType];

            txtTotalPlayTime.text = $"Level Completed in <style=\"Play Time\">{animatableValues.TotalPlayTime} secs";

            // Determine how many stars should we animate
            switch (animatableValues.TotalStars)
            {
                case 1:
                    animator.Play(Constants.GameOverAnimationStates.SuccessOneStar);
                    break;

                case 2:
                    animator.Play(Constants.GameOverAnimationStates.SuccessTwoStar);
                    break;

                case 3:
                    animator.Play(Constants.GameOverAnimationStates.SuccessFullStar);
                    break;
            }
        }
        else
        {
            animator.Play(Constants.GameOverAnimationStates.Failure);
        }
    }

    // Animation Events
    public void PlaySuccessSfx() => SfxManager.Instance.PlaySuccessSfx();
    public void PlayStarFillSfx() => SfxManager.Instance.PlayStarFill();
    public void PlayFullStarFillSfx() => SfxManager.Instance.PlayFullStar();

    public void PlayConfetti()
    {
        confetti.Play();
        SfxManager.Instance.PlayConfettiSfx();
    }

    public void OnRewardDialogShown() => SfxManager.Instance.PlayRewardsDialogShown();

    public void PlayFailSfx()       => SfxManager.Instance.PlayFailureSfx();
    public void PlaySkipLevelPop()  => SfxManager.Instance.PlaySkipLevelPop();

    // The entire animation has finished
    public void OnAnimationCompleted() { }
}
