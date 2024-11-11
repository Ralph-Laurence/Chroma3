using UnityEngine;

public enum ProgressAnimations
{
    Pingpong,
    Indeterminate
}

public enum IndefiniteProgressBarBehaviours
{
    ManualStart,
    OnEnable
}

public class IndefiniteProgressBar : MonoBehaviour
{
    [SerializeField] private IndefiniteProgressBarBehaviours behaviour;
    [SerializeField] private ProgressAnimations animationType;
    [SerializeField] private float moveDuration = 2.0f;  // Time taken to move from left to right
    [SerializeField] private RectTransform fillRectTransform;

    private bool isStopped = true;
    private float startTime;
    private Vector2 startPos;
    private Vector2 endPos;
    private RectTransform parentRectTransform;

    public void Stop()
    {
        isStopped = true;
        Reset();
    }

    private void Begin()
    {
        Reset();
        isStopped = false;
    }

    void OnEnable()
    {
        if (behaviour == IndefiniteProgressBarBehaviours.OnEnable)
            Begin();
    }

    void OnDisable()
    {
        if (behaviour == IndefiniteProgressBarBehaviours.OnEnable)
            Stop();
    }

    void Start()
    {
        fillRectTransform.parent.TryGetComponent(out parentRectTransform);
        
        switch (animationType)
        {
            case ProgressAnimations.Pingpong:
                // Initial start position (left side of the parent)
                startPos = new Vector2(0, fillRectTransform.anchoredPosition.y);

                // End position (right side of the parent - fill width)
                endPos = new Vector2
                (
                    parentRectTransform.rect.width - fillRectTransform.rect.width,
                    fillRectTransform.anchoredPosition.y
                );
                break;

            case ProgressAnimations.Indeterminate:
                // Initial start position (left side of the parent)
                startPos = new Vector2(-fillRectTransform.rect.width, fillRectTransform.anchoredPosition.y);

                // End position (right side of the parent - half of the fill width)
                endPos = new Vector2(parentRectTransform.rect.width + fillRectTransform.rect.width, fillRectTransform.anchoredPosition.y);

                startTime = Time.time;
                fillRectTransform.anchoredPosition = startPos;
                break;
        }
        
        startTime = Time.time;
    }

    void Update()
    {
        if (isStopped)  
            return;

        switch (animationType)
        {
            case ProgressAnimations.Pingpong:
                AnimateBounce();
                break;

            case ProgressAnimations.Indeterminate:
                AnimateIndeterminate();
                break;
        }
    }

    private void AnimateBounce()
    {
        float t = (Time.time - startTime) / moveDuration;
        t = Mathf.PingPong(t, 1.0f);

        fillRectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
    }

    private void AnimateIndeterminate()
    {
        float elapsedTime = Time.time - startTime;
        float t = elapsedTime / moveDuration;

        // Calculate current position
        if (t >= 1.0f)
        {
            // Reset the start time and position if we've reached the end
            startTime = Time.time;
            fillRectTransform.anchoredPosition = startPos;
        }
        else
        {
            fillRectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
        }
    }

    private void Reset()
    {
        startTime = Time.time;
        fillRectTransform.anchoredPosition = startPos;
    }
}