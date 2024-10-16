using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private SoundEffects sfx;
    private bool animationBegan;

    void Awake()
    {
        sfx = SoundEffects.Instance;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (orthoCam == null && autoFindCamera)
            orthoCam = Camera.main;

        UpdatePointerScale();
    }

    // Update is called once per frame
    void Update() 
    {
        UpdatePointerScale();    
    }

    public void SetTargets(List<GameObject> targets) => this.targets = targets;

    /// <summary>
    /// Resize the pointer so that it looks the same across resolutions
    /// </summary>
    private void UpdatePointerScale()
    {
        var height = pointer.bounds.size.y;
        var scale = pointerScale / height;

        pointer.transform.localScale = new (scale, scale, 1.0F);
    }

    private IEnumerator IEAnimateClick()
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
    }

    public void MatchStageRotation(Vector3 eulerAngles) => pointer.transform.eulerAngles = eulerAngles;

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
            // Wait for step duration before moving to next target
            yield return step;

            var target = targets[i];

            pointer.transform.position = target.transform.position;

            yield return StartCoroutine(IEAnimateClick());
        }
    }
}
