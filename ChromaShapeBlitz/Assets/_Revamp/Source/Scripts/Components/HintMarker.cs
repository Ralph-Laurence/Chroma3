using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HintMarker : MonoBehaviour
{
    // Prevent UI interactions during hint animation
    [SerializeField] private GameObject pointerHand;
    [SerializeField] private Sprite pointerOff;
    [SerializeField] private Sprite pointerOn;
    [SerializeField] private AudioClip sfxClick;

    [SerializeField] private SpriteRenderer pointer;
    [SerializeField] private float pointerScale = 0.1F;
    [SerializeField] private float clickDuration = 0.25F;

    [SerializeField] private Camera orthoCam;
    [SerializeField] private bool autoFindCamera;

    [SerializeField] private List<GameObject> targets;
    [SerializeField] private float hintDurationEasy = 0.25F;
    [SerializeField] private float hintDurationNormal = 0.1F;
    [SerializeField] private float hintDurationHard = 0.01F;
    [SerializeField] private Transform hintStepsContainer;
    [SerializeField] private GameObject hintStepNumber;

    private SoundEffects sfx;
    private bool animationBegan;

    private List<Transform> hintSteps;

    void Awake()
    {
        sfx = SoundEffects.Instance;
        hintSteps = new();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (orthoCam == null && autoFindCamera)
            orthoCam = Camera.main;

        UpdateSpriteRendererScales();
    }

    // Update is called once per frame
    void Update() 
    {
        UpdateSpriteRendererScales();    
    }

    private void OnDisable()
    {
        ClearStepNumbers();
    }

    public void SetTargets(List<GameObject> targets) => this.targets = targets;

    /// <summary>
    /// Resize the sprite renderers so that it looks the same across resolutions
    /// </summary>
    private void UpdateSpriteRendererScales()
    {
        var height = pointer.bounds.size.y;
        var scale = pointerScale / height;

        pointer.transform.localScale = new (scale, scale, 1.0F);

        for (var i = 0; i < hintSteps.Count; i++)
        {
            if (hintSteps[i] == null)
                continue;

            var hintNumberScale = new Vector3(scale - 0.1F, scale - 0.1F, 1.0F);

            hintSteps[i].transform.localScale = hintNumberScale;
        }
    }

    private IEnumerator IEAnimateClick(bool isEndHint)
    {
        animationBegan = true;

        var wait = new WaitForSeconds(clickDuration / 3.0F);

        pointer.sprite = pointerOff;

        yield return wait;

        pointer.sprite = pointerOn;
        sfx.PlayOnce(sfxClick);

        yield return wait;

        pointer.sprite = pointerOff;
        animationBegan = false;

        if (isEndHint)
            pointerHand.SetActive(false);
    }

    private void AddHintStepNumber(Vector3 position, int number)
    {
        var step = Instantiate(hintStepNumber, position, Quaternion.Euler(Vector3.right * 90.0F));

        step.transform.SetParent(hintStepsContainer);

        step.TryGetComponent(out HintStepNumber stepNumber);
        stepNumber.StepNumber   = number;

        hintSteps.Add(step.transform);
    }

    private void ClearStepNumbers()
    {
        for (var i = 0; i < hintSteps.Count; i++)
        {
            if (hintSteps[i] == null)
                continue;

            Destroy(hintSteps[i].gameObject);
        }

        hintSteps.Clear();
    }

    public void MatchStageRotation(Vector3 eulerAngles)
    {
        pointerHand.SetActive(true);
        pointer.transform.eulerAngles = eulerAngles;
    }

    public IEnumerator ShowHints(LevelDifficulties difficulty)
    {
        if (animationBegan)
            yield break;
		
        // Stages under easy level have slower hint duration
        // while hard stages have faster hint duration.
        var hintDuration = difficulty switch
        {
            LevelDifficulties.Easy   => hintDurationEasy,
            LevelDifficulties.Normal => hintDurationNormal,
            LevelDifficulties.Hard   => hintDurationHard,
            _ => 0.3F
        };

        var individualDuration = hintDuration / targets.Count;
        var step = new WaitForSeconds(individualDuration);

        // Initially position the pointer into the first target
        pointer.transform.position = targets[0].transform.position;

        gameObject.SetActive(true);

        for (var i = 0; i < targets.Count; i++)
        {
            //// Wait for step duration before moving to next target
            //yield return step;

            var targetPos = targets[i].transform.position;

            pointer.transform.position = targetPos;

            var isEndHint = i == targets.Count - 1;

            yield return StartCoroutine(IEAnimateClick(isEndHint));

            // Raise the stemp number above the ground a little
            targetPos.y += 0.2F;

            //yield return new WaitForSeconds(0.25F);
            // Wait for step duration before moving to next target
            yield return step;

            AddHintStepNumber(targetPos, i + 1);

            yield return null;
        }
    }

    /// <summary>
    /// Waits for a second before being fully disabled.
    /// This approach allows the step numbers to be seen by the player first by disabling 
    /// the pointer hand first to display the numbers in the span of 1 second
    /// </summary>

    public IEnumerator TurnOff()
    {
        yield return new WaitForSeconds(1.0F);

        gameObject.SetActive(false);
    }
}
