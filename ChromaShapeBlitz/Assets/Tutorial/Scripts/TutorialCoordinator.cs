using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialCoordinator : MonoBehaviour
{
    [SerializeField] private GameObject tutorialConsentDialog;
    [SerializeField] private List<GameObject> tutorialDrivers;
    [SerializeField] private int inventoryTutorialIndexContinuation;

    // Will be invoked when all tutorials are completed just before we delete this gameobject
    [SerializeField] private UnityEvent OnAllTutorialsComplete;

    private TutorialDriver tutorialDriver;
    private GameSessionManager gsm;
    
    void Awake()
    {
        gsm = GameSessionManager.Instance;

        // Load the saved state of the tutorial
        var data = GameSessionManager.Instance.UserSessionData;
        Debug.Log($"Current Step -> {data.CurrentTutorialStep}");

        if (gsm.UserSessionData.IsTutorialCompleted)
        {
            HandleTutorialsCompleted();
            return;
        }
        //
        // The 6th tutorial step is a bit different from the rest of the tutorials as it involves two different shop sections.
        // The user might have closed the game while the inventory tutorial is ongoing. We will handle that differently.
        //
        var currentTutStep           = gsm.UserSessionData.CurrentTutorialStep;
        var isInventoryTutorialBegan = currentTutStep == TutorialSteps.STEP7_EQUIP_INVENTORY;
        //
        //
        //
        if (data == null || (data != null && data.IsTutorialCompleted))
            return;
        
        for (var i = 0; i < tutorialDrivers.Count; i++)
        {
            tutorialDrivers[i].TryGetComponent(out TutorialDriver driver);

            if (driver == null) // Driver is null
                continue;

            if (isInventoryTutorialBegan && driver.StepIdentifier == TutorialSteps.STEP6_POWERUP_PURCHASE)
            {
                tutorialDriver = driver;
                tutorialDriver.JumpToContentIndex(inventoryTutorialIndexContinuation);
            }

            // Find the needed tutorial driver by step
            else if (driver.StepIdentifier == data.CurrentTutorialStep)
                tutorialDriver = driver;

            else
                Destroy(driver.gameObject); // Remove other drivers
        }

        tutorialDrivers?.Clear();

        if (!gsm.UserSessionData.IsTutorialCompleted && data.CurrentTutorialStep == TutorialSteps.STEP1_BASICS)
        {
            tutorialConsentDialog.SetActive(true);
        }

        if (tutorialDriver != null)
            tutorialDriver.gameObject.SetActive(true);
    }

    private void HandleTutorialsCompleted()
    {
        for (var i = 0; i < tutorialDrivers.Count; i++)
        {
            var driver = tutorialDrivers[i];
            Destroy(driver);
        }

        tutorialDrivers?.Clear();

        OnAllTutorialsComplete?.Invoke();

        Destroy(tutorialConsentDialog);
        Destroy(gameObject);
    }

    public void SkipTutorial()
    {
        var userData = gsm.UserSessionData;

        userData.IsTutorialCompleted = true;

        userData.CurrentTutorialStep  = TutorialSteps.TUTORIALS_COMPLETE;
        userData.CurrentTutorialStage = TutorialDriver.MAX_STAGES;
        userData.MainMenuTheme        = MainMenuThemeIdentifier.Auto;

        StartCoroutine(UserDataHelper.Instance.SaveUserData(userData, (u) =>
        {
            gsm.UserSessionData = u;
            HandleTutorialsCompleted();
        }));
    }

    public void OnTutorialConsentGranted()
    {
        Destroy(tutorialConsentDialog);
    }
}
