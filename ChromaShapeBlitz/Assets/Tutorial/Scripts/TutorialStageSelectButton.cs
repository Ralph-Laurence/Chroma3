using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialStageSelectButton : MonoBehaviour
{
    [SerializeField] private int stageNumber;
    [SerializeField] private Sprite bgLocked;
    [SerializeField] private Sprite bgUnlocked;
    public TextMeshProUGUI stageNumberLabel;
    public GameObject LockIndicator;
    
    [Header("Stars in array must be in order of display")]
    public Image[] StarImage;

    private Image buttonBackground;
    private Button button;

    void Start()
    {
        TryGetComponent(out buttonBackground);
        TryGetComponent(out button);

        button.onClick.AddListener(() => SceneManager.LoadScene( $"{Constants.Scenes.TutorialStagePrefix}{stageNumber}" ));
    }

    public void SetStars(int starCount, Sprite starFilled, Sprite starUnfilled)
    { 
       for (var i = 0; i < StarImage.Length; i++)
       {
            if (i < starCount)
                StarImage[i].sprite = starFilled;
            else
                StarImage[i].sprite = starUnfilled;
       }
    }

    public void Unlock()
    {
        buttonBackground.sprite = bgUnlocked;

        stageNumberLabel.gameObject.SetActive(true);
        LockIndicator.SetActive(false);
        button.interactable = true;
    }
    
    public void Lock()
    {
        buttonBackground.sprite = bgLocked;

        stageNumberLabel.gameObject.SetActive(false);
        LockIndicator.SetActive(true);
        button.interactable = false;
    }
}
