using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum TutorialSteps
{
    MainMenu,
    GamePlay,
    SkinShop,
    BackgroundShop,
    PowerupsAndInventory
}

public class TutorialController : MonoBehaviour
{
    [SerializeField] private RectTransform targets;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }


    /* private void RenderCloner()
    {
        //..........................................................//
        // # Position the cloner exactly at the target's position # //
        //..........................................................//
        
        // Step 1: Get the target's world position
        var worldPosition = target.TransformPoint(Vector3.zero);

        // Step 2: Calculate the center position offset of the target in local space
        var targetCenterOffset = new Vector2
        (
            (0.5F - target.pivot.x) * target.rect.width,
            (0.5F - target.pivot.y) * target.rect.height
        );

        // Step 3: Convert the target's center to world space
        var targetCenterWorldPosition = worldPosition + target.TransformVector(targetCenterOffset);

        // Step 4: Convert the world position of the center to the canvas local space
        RectTransformUtility.ScreenPointToLocalPointInRectangle
        (
            canvasRectTransform,
            RectTransformUtility.WorldToScreenPoint(null, targetCenterWorldPosition),
            null,
            out Vector2 localPoint
        );

        // Step 5: Assign this position to the cursor's anchored position
        cloner.anchoredPosition = localPoint;
    }*/
}
