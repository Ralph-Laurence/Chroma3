using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public struct LevelSelectPageData
{
    public LevelDifficulties Difficulty;
    public LevelSelectPage EventSender;
    public int TotalStages;
}

[RequireComponent(typeof(Button))]
public class LevelSelectPage : MonoBehaviour
{
    [SerializeField] private LevelDifficulties difficulty;
    [SerializeField] private float scaleDownRate = 0.25F;

    private Button button;
    private UISound uiSound;
    private RectTransform rect;

    void OnEnable()
    {
        OnStageSelectMenuCloseNotifier.BindEvent(ObserveStageSelectMenuClosed);    
    }

    void OnDisable()
    {
        OnStageSelectMenuCloseNotifier.UnbindEvent(ObserveStageSelectMenuClosed);
    }

    void Awake()
    {
        uiSound = UISound.Instance;
        TryGetComponent(out rect);
        TryGetComponent(out button);
        
        button.onClick.AddListener(Ev_HandleClicked);
    }

    private void Ev_HandleClicked()
    {   
        if (uiSound != null)
            uiSound.PlayUxPositiveClick();

        LevelMenuNotifier.NotifyLevelPageClicked(new LevelSelectPageData
        {
            Difficulty  = difficulty,
            TotalStages = Revamp.GameManager.GetTotalStages(difficulty),
            EventSender = this
        });

        Downscale();
    }

    private void Upscale() //(RectTransform rect, Action callback = null)
    {
        LeanTween.scale(rect, Vector3.one, scaleDownRate);//.setOnComplete(() => callback?.Invoke());
    }

    private void Downscale()
    {
        LeanTween.scale(rect, Vector3.zero, scaleDownRate);//.setOnComplete(() => callback?.Invoke());
    }

    private void ObserveStageSelectMenuClosed(LevelSelectPage sender)
    {
        if (sender != this)
            return;

        Upscale();
    }
}
