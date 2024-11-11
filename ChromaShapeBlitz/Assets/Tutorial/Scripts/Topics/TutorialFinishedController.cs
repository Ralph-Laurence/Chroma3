using HighlightPlus;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class TutorialFinishedController : MonoBehaviour
{
    [SerializeField] private TutorialPointerHand pointerHand;
    [SerializeField] private RectTransform chestRenderImage;
    [SerializeField] private GameObject chestObj;
    [SerializeField] private GameObject chestLid;
    [SerializeField] private float      chestOuterGlowMax = 3.0F;
    [SerializeField] private float      chestOuterGlowMin = 1.0F;
    [SerializeField] private GameObject caption;

    [SerializeField] private HighlightEffect highlightEffect;
    public PostProcessVolume postProcessVolume;
    private Bloom bloom;

    private bool readyForClickAction;
    private RawImage chestImage;
    private Button chestButton;

    private bool animationBegan;

    [Space(10)]
    [SerializeField] private List<GameObject> rewardItems;
    [SerializeField] private Vector2[] rewardPositions;

    [SerializeField] private AudioClip sfxOpenChest;
    [SerializeField] private AudioClip sfxPopOutItems;
    [SerializeField] private GameObject tapToContinue;
    [SerializeField] private GameObject fancySceneLoader;

    private SoundEffects sfx;

    private void Awake()
    {
        sfx = SoundEffects.Instance;

        chestRenderImage.TryGetComponent(out chestImage);
        chestRenderImage.TryGetComponent(out chestButton);
        postProcessVolume.profile.TryGetSettings(out bloom);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Begin());
    }

    private IEnumerator Begin()
    {
        yield return new WaitForSeconds(1.75F);

        pointerHand.SetTarget(chestRenderImage);
        pointerHand.Show();
        caption.SetActive(true);
        readyForClickAction = true;
    }

    public void HandleChestClicked()
    {
        if (!readyForClickAction || animationBegan)
            return;

        animationBegan = true;

        pointerHand.Hide();
        caption.SetActive(false);

        // Rotate the chest to face the camera
        LeanTween.rotateY(chestObj, 40.0F, 0.45F)
                 .setOnComplete(AnimateChestOpen);

        sfx.PlayOnce(sfxOpenChest);
    }

    private void AnimateChestOpen()
    {
        // Open the lid
        LeanTween.rotateX(chestLid, 60.0F, 0.3F)
                 .setDelay(0.35F)
                 .setOnComplete(AnimatePopOutItems);

        // Increase global bloom
        LeanTween.value(chestLid, 0.0F, 8.0F, 0.3F)
                     .setDelay(0.35F)
                     .setOnUpdate((float bloomAmount) =>
                     {
                         bloom.intensity.value = bloomAmount;
                     });

        // Decrease highlight
        LeanTween.value(chestLid, chestOuterGlowMax, chestOuterGlowMin, 0.2F)
                     .setDelay(0.35F)
                     .setOnUpdate((float glow) =>
                     {
                         highlightEffect.glow = glow;
                     });
    }

    private void AnimatePopOutItems()
    {
        StartCoroutine(IEAnimatePopOutItems());
    }

    private IEnumerator IEAnimatePopOutItems()
    {
        for (var i = 0; i < rewardItems.Count; i++)
        {
            var item = rewardItems[i];
            var targetPos = rewardPositions[i];

            LeanTween.scale(item, Vector3.one, 0.3F);
            LeanTween.moveLocal(item, targetPos, 0.3F);

            sfx.PlayOnce(sfxPopOutItems);

            yield return new WaitForSeconds(0.3F);
        }

        yield return new WaitForSeconds(0.5F);
        chestImage.raycastTarget = false;
        chestButton.interactable = false;
        tapToContinue.SetActive(true);
    }

    public void ExitToMenu()
    {
        Instantiate(fancySceneLoader).TryGetComponent(out FancySceneLoader loader);
        loader.LoadScene(Constants.Scenes.MainMenu, true);
    }
}
