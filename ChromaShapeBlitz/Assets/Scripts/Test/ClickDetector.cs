using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ClickDetector : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent whenClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked: " + eventData.pointerCurrentRaycast.gameObject.name);

        if (whenClicked != null)
            whenClicked.Invoke();
    }
}