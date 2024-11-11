using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimeFreezeFx : MonoBehaviour
{
    [SerializeField] private AudioClip sfxThreeSecs;
    [SerializeField] private AudioClip sfxFiveSecs;
    [SerializeField] private float effectSpeed = 4.0F;

    private readonly Color TRANSPARENT = Constants.ColorSwatches.TRANSPARENT;
    private SoundEffects sfx;
    private Image m_image;

    void Awake()
    {
        sfx = SoundEffects.Instance;
        TryGetComponent(out m_image);
        m_image.color = TRANSPARENT;
        m_image.enabled = false;
    }

    public void ExecuteVisualEffect(int freezeSeconds)
    {
        var sound = sfxThreeSecs;

        if (freezeSeconds == 5)
            sound = sfxFiveSecs;

        m_image.enabled = true;
        sfx.PlayOnce(sound);

        var effectSpeed = freezeSeconds / this.effectSpeed;
        var targetColor = Constants.ColorSwatches.WHITE;
        
        SetImageColor(TRANSPARENT);
        
        LeanTween.value(m_image.gameObject, SetImageColor, m_image.color, targetColor, effectSpeed)
                 .setOnComplete(() => {

                    SetImageColor(targetColor);
                    var delay  = freezeSeconds - effectSpeed; // / 2.0F;

                    LeanTween.value(m_image.gameObject, SetImageColor, m_image.color, TRANSPARENT, effectSpeed)
                             .setDelay(delay)
                             
                             .setOnComplete(() => {
                                SetImageColor(TRANSPARENT);
                                m_image.enabled = false;
                             });
                 });
    }

    private void SetImageColor(Color color) => m_image.color = color;
}
