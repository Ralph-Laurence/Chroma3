using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{   
    public Camera cam;
    public GameObject hero;
    public Transform targetStage;
    public float heroHeight = 10.0F;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
            SpawnHero();
    }

    // void SpawnHero()
    // {
    //     // Target position
    //     var targetPos = targetStage.transform.position;
    //     targetPos.y = targetStage.transform.position.y + heroHeight;

    //     // Bottom-Left = 0,0
    //     var startPos = cam.ViewportToWorldPoint(Vector3.forward * cam.nearClipPlane);

    //     // Coming from bottom-left corner
    //     startPos.y = targetPos.y;

    //     hero.transform.position = startPos;

    //     // Rotate the hero to the heading direction
    //     hero.transform.LookAt(new Vector3(0.0F, targetPos.y));

    //     // Bottom-Right = 0,1
    //     var exitPos = cam.ViewportToWorldPoint(new Vector3(1.0F, 0.0F, cam.nearClipPlane));
    //     exitPos.y = targetPos.y;

    //     var rollingBegan = false;

    //     // Move to the direction
    //     LeanTween.move(hero, targetPos, 2.0F)
    //              .setEase(LeanTweenType.easeOutQuad)
    //              .setOnUpdate((float time) => {

    //                 if (time >= 0.85F && !rollingBegan)
    //                 {
    //                     rollingBegan = true;

    //                     var rollHeight = 5.0F;
    //                     var rollDuration = 2.0F;
    //                     var rollAxis = transform.right * -1.0F; // (-1, 0, 0) like transform.left
    //                     var rollAmount = 270.0F;

    //                     // Do a loop-ramp-roll
    //                     LeanTween.rotateAroundLocal(hero, rollAxis, rollAmount, rollDuration)
    //                              .setEase(LeanTweenType.easeInOutQuad)
    //                              .setOnComplete(() => {

    //                                  // Calculate the direction to the target
    //                                  Vector3 direction = exitPos - hero.transform.position;
    //                                  // Create the rotation to look at the target
    //                                  Quaternion lookRotation = Quaternion.LookRotation(direction);

    //                                  LeanTween.rotateY(hero, lookRotation.eulerAngles.y, rollDuration / 3.0F)
    //                                           .setEase(LeanTweenType.easeOutQuad)
    //                                           .setOnComplete(() => {

    //                                               var heading = lookRotation.eulerAngles;
    //                                               heading.x = 0.0F;
    //                                               heading.z = 0.0F;

    //                                               // Use LeanTween to rotate the object smoothly
    //                                               LeanTween.rotate(hero, heading, 0.35F)
    //                                                        .setEase(LeanTweenType.easeOutQuad)
    //                                                        .setOnUpdate((float t) => {

    //                                                             if (t >= 0.85F)
    //                                                                 LeanTween.move(hero, exitPos, 3.0F)
    //                                                                          .setEase(LeanTweenType.easeOutQuad);

    //                                                        });
    //                                           });
    //                              });

    //                      //Move the hero up and down
    //                      LeanTween.moveY(hero, startPos.y + rollHeight, rollDuration / 2)
    //                               .setEase(LeanTweenType.easeInOutQuad)
    //                               .setOnComplete(() =>
    //                               {
    //                                   LeanTween.moveY(hero, startPos.y, rollDuration / 2)
    //                                            .setEase(LeanTweenType.easeOutQuad);
    //                               });
    //                  }
    //              });
    // }

    // void SpawnHero()
    // {
    //     // Target position
    //     var targetPos = targetStage.transform.position;
    //     targetPos.y = targetStage.transform.position.y + heroHeight;

    //     // Bottom-Left = 0,0
    //     var startPos = cam.ViewportToWorldPoint(Vector3.forward * cam.nearClipPlane);
    //     startPos.y = targetPos.y;

    //     hero.transform.position = startPos;

    //     // Rotate the hero to the heading direction
    //     hero.transform.LookAt(new Vector3(0.0F, targetPos.y));

    //     // Bottom-Right = 0,1
    //     var exitPos = cam.ViewportToWorldPoint(new Vector3(1.0F, 0.0F, cam.nearClipPlane));
    //     exitPos.y = targetPos.y;

    //     var rollingBegan = false;

    //     // Move to the direction
    //     LeanTween.move(hero, targetPos, 2.0F)
    //              .setEase(LeanTweenType.easeOutQuad)
    //              .setOnUpdate((float time) =>
    //              {

    //                  if (time >= 0.85F && !rollingBegan)
    //                  {
    //                      rollingBegan = true;

    //                      var rollHeight = 5.0F;
    //                      var rollDuration = 2.0F;
    //                      var rollAxis = transform.right * -1.0F; // (-1, 0, 0) like transform.left
    //                      var rollAmount = 270.0F;

    //                      // Do a loop-ramp-roll
    //                      LeanTween.rotateAroundLocal(hero, rollAxis, rollAmount, 1.5F)
    //                           .setEase(LeanTweenType.easeInOutQuad)
    //                           .setOnComplete(() =>
    //                              {

    //                                  // Calculate the direction to the target
    //                                  Vector3 direction = exitPos - hero.transform.position;
    //                                  // Create the rotation to look at the target
    //                                  Quaternion lookRotation = Quaternion.LookRotation(direction);

    //                                  // Blend the final heading rotation sooner
    //                                  LeanTween.rotateY(hero, lookRotation.eulerAngles.y, 1.0F)
    //                                           .setEase(LeanTweenType.easeOutQuad)
    //                                           .setOnComplete(() =>
    //                                           {

    //                                               var heading = lookRotation.eulerAngles;
    //                                               heading.x = 0.0F;
    //                                               heading.z = 0.0F;

    //                                               // Rotate smoothly towards the exit
    //                                               LeanTween.rotate(hero, heading, 0.25F)
    //                                                        .setEase(LeanTweenType.easeOutQuad)
    //                                                        .setOnUpdate((float t) =>
    //                                                        {

    //                                                            if (t >= 0.85F)
    //                                                                LeanTween.move(hero, exitPos, 3.0F)
    //                                                                     .setEase(LeanTweenType.easeOutQuad);
    //                                                        });
    //                                           });
    //                              });

    //                      //Move the hero up and down
    //                      LeanTween.moveY(hero, startPos.y + rollHeight, rollDuration / 2)
    //                               .setEase(LeanTweenType.easeInOutQuad)
    //                               .setOnComplete(() =>
    //                               {
    //                                   LeanTween.moveY(hero, startPos.y, 1.25F)
    //                                            .setEase(LeanTweenType.easeOutQuad);
    //                               });
    //                  }
    //              });
    // }

        void SpawnHero()
{
    // Set up positions and heights
    Vector3 startPos = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
    startPos.y = heroHeight;

    Vector3 loopPos = new Vector3(0, heroHeight, 0); // The (0, 0) position where the loop happens
    Vector3 exitPos = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
    exitPos.y = heroHeight;

    // Place the hero at the start position and look towards the loop point
    hero.transform.position = startPos;
    hero.transform.LookAt(loopPos);

    var rollBegan = false;

    // Smooth movement to the loop position
    LeanTween.move(hero, loopPos, 2.0f).setEase(LeanTweenType.linear)
             .setOnUpdate((float time) =>
             {
                if (time >= 0.75F && !rollBegan)
                {
                    rollBegan = true;

                    // Perform the loop and roll without interruption
                    PerformLoopAndRoll(loopPos, exitPos);
                }
             });
    }

void PerformLoopAndRoll(Vector3 loopPos, Vector3 exitPos)
{
    // Roll variables
    float rollHeight = 5.0f;
    float rollDuration = 2.0f;
    float rollAmount = 270.0f; // Full roll
    var rollAxis = hero.transform.right * -1.0F;

    // Move up and down while doing the roll without stopping
    LeanTween.moveY(hero, loopPos.y + rollHeight, rollDuration / 2)
        .setEase(LeanTweenType.easeOutQuad) // Smooth easing for up and down
        .setLoopPingPong(1); // Move up then back down

    // Perform the roll while moving up and down, keeping it continuous
    LeanTween.rotateAroundLocal(hero, rollAxis, rollAmount, rollDuration)
        .setEase(LeanTweenType.easeInOutQuad) // Use smooth easing for the roll
        .setOnComplete(() => {
            // Transition immediately to the next stage (no delay)
            SmoothHeadingTransition(exitPos);
        });
}

void SmoothHeadingTransition(Vector3 exitPos)
{
    // Calculate the direction to the exit point
    Vector3 direction = exitPos - hero.transform.position;
    Quaternion targetRotation = Quaternion.LookRotation(direction);

    // Rotate smoothly to face the exit point
    LeanTween.rotate(hero, targetRotation.eulerAngles, 0.5f).setEase(LeanTweenType.linear);

    // Immediately move the hero towards the exit without delay
    LeanTween.move(hero, exitPos, 3.0f).setEase(LeanTweenType.linear);
}


}