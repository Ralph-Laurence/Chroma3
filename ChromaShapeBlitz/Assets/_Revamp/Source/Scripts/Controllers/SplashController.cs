using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashController : MonoBehaviour
{
    [SerializeField] private Image fadeScreen;
    [SerializeField] private GameObject roundel;
    [SerializeField] private float roundelDuration = 1.25F;
    [SerializeField] private float roundelShineDuration = 1.25F;
    [SerializeField] private float roundelShineIntensity = 3.0F;

    [SerializeField] private PostProcessVolume postProcess;
    private Bloom bloom;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(BeginTransition());
    }

    private IEnumerator BeginTransition()
    {
        FadeInScreen();

        LeanTween.rotateY(roundel, 360.0F * 2.0F, roundelDuration);
        LeanTween.scale(roundel, Vector3.one, roundelDuration)
                 .setOnComplete(() =>
                 {
                     // Get the Bloom effect from the PostProcessVolume
                     if (postProcess.profile.TryGetSettings(out bloom))
                     {
                         ShineRoundel();
                     }
                 });

        yield return new WaitForSeconds((roundelShineDuration * 2.0F) + 2.0F);

        FadeOutScreen(() =>
        {
            var bootstrapper = Constants.Scenes.Bootstrapper;
            SceneManager.LoadScene(bootstrapper);
        });
    }

    private void ShineRoundel()
    {
        // Start the tween sequence

        // Increase bloom
        LeanTween.value(roundel, 1.0F, roundelShineIntensity, roundelShineDuration)
            .setOnUpdate((float val) => bloom.intensity.value = val)
            .setOnComplete(() =>
            {
                // Decrease bloom
                LeanTween.value(roundel, roundelShineIntensity, 1.0F, roundelShineDuration)
                         .setOnUpdate((float val) => bloom.intensity.value = val);
            });
    }

    private void FadeInScreen()
    {
        var color = fadeScreen.color;
        color.a = 1.0F;

        LeanTween.value(fadeScreen.gameObject, 1.0F, 0.0F, 1.25F)
                 .setOnUpdate((float value) =>
                 {
                     color.a = value;
                     fadeScreen.color = color;
                 });
    }

    private void FadeOutScreen(Action whenDone)
    {
        var color = fadeScreen.color;
        color.a = 0.0F;

        LeanTween.value(fadeScreen.gameObject, 0.0F, 1.0F, 1.0F)
                 .setOnUpdate((float value) =>
                 {
                     color.a = value;
                     fadeScreen.color = color;
                 })
                 .setOnComplete(whenDone);
    }
}
