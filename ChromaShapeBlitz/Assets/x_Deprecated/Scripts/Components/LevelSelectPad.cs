using UnityEngine;
using UnityEngine.UI;

[System.Obsolete]
public class LevelSelectPad : MonoBehaviour
{
    [SerializeField] private Sprite filledStar;
    [SerializeField] private Sprite unfilledStar;

    [SerializeField] private Image[] stars;

    public void FillStars(int starCount)
    {
        for(var i = 0; i < stars.Length; i++)
        {
            if ( i <= starCount - 1)
                stars[i].sprite = filledStar;
        }
    }
}
