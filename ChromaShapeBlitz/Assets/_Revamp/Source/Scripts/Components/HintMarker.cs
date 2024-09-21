using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private float yOffset = 0.5F;

    [SerializeField] private Camera orthoCam;
    [SerializeField] private bool autoFindCamera;

    [SerializeField] private List<GameObject> targets;
    [SerializeField] private float hintStep = 0.25F;

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

    public IEnumerator ShowHints(Action onAnimationBegan = null)
    {
        if (animationBegan)
            yield break;

		onAnimationBegan?.Invoke();
		
        var step = new WaitForSeconds(hintStep);

        for (var i = 0; i < targets.Count; i++)
        {
            var target = targets[i];

            transform.position = target.transform.position;
            yield return StartCoroutine(IEAnimateClick());

            // Wait for step duration before moving to next target
            yield return step;
        }
    }
}
