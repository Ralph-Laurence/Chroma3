using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SlotMachineKnob : MonoBehaviour
{
    [SerializeField] private GameObject knob;
    [SerializeField] private AudioClip knobPulledSfx;
    [SerializeField] private UnityEvent OnKnobPulled;
    [SerializeField] private UnityEvent OnKnobRetract;
    
    private LTDescr tween;
    private Button button;
    private SoundEffects sfx;
    private RectTransform knobRect;

    private void Awake()
    {
        sfx = SoundEffects.Instance;

        TryGetComponent(out button);
        knob.TryGetComponent(out knobRect);

        button.onClick.AddListener(Pulldown);
    }

    private void Pulldown()
    {
        sfx.PlayOnce(knobPulledSfx);
        button.enabled = false;

        LeanTween.cancel(knob);

        // Store the initial angle
        var initialAngle = knobRect.localEulerAngles.x;

        // Define the callback for updating the rotation
        var callback = new Action<float>((v) =>
        {
            knobRect.localEulerAngles = Vector3.left * v;
        });

        tween?.reset();

        // Start the tween from the initial angle to 0 smoothly over 0.3 seconds
        tween = LeanTween.value(gameObject, initialAngle, 180.0F, 0.3F)
                 .setOnUpdate(callback)
                 .setOnComplete(() =>
                 {
                     OnKnobPulled?.Invoke();
                     knobRect.localEulerAngles = Vector3.left * 180.0F;
                 });
    }

    public void Retract()
    {
        LeanTween.cancel(gameObject);

        knob.TryGetComponent(out RectTransform knobRect);

        // Capture the initial x-axis rotation
        float initialXRotation = 180.0F;

        // Define the callback to update only the x-axis rotation
        var callback = new Action<float>((v) =>
        {
            knobRect.localEulerAngles = Vector3.left * v;
        });

        tween?.reset();

        // Tween the x-axis from the initial rotation to 0 degrees over 0.3 seconds
        tween = LeanTween.value(gameObject, initialXRotation, 0f, 0.3f)
                 .setOnUpdate(callback)
                 .setOnComplete(() =>
                 {
                     // Explicitly set the x rotation to 0 to ensure it ends correctly
                     knobRect.localEulerAngles = Vector3.zero;
                     OnKnobRetract?.Invoke();

                     button.enabled = true;
                 });
    }

}
