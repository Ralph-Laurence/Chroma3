using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public GameObject easyTrophy;
    public GameObject normalTrophy;
    public GameObject hardTrophy;
    public RectTransform canvas;

    void Update()
    {
	switch(Input.inputString)
	{
	    case "1":
		Instantiate(easyTrophy, canvas);
		break;
	    case "2":
		Instantiate(normalTrophy, canvas);
		break;
	    case "3":
		Instantiate(hardTrophy, canvas);
		break;
	}
    }
}