using System.Collections;
using UnityEngine;
using Narrate;

public class StoryTracker : MonoBehaviour {

    [Header("Global Variables")]
    public bool key;
    public bool hat;

    [Header("Lambert")]
    public bool lambertInitiated;
    public GameObject[] pushPullObjects;
    public GameObject[] interactiveObjects;
    public bool rugCollected;
    public bool wigSnatched;
    public OnEnableNarrationTrigger wigNarration;
    public GameObject lambertCamerino;
    public GameObject lambertCamerinoDoor;
    public CamerinoDoorBehaviour lambertCamerinoDoorBehaviour;
    public GameObject[] principalDoors = new GameObject[2];
    public GameObject lambertPrincipal;
    public Transform lambertPrincipalPos;
    public GameObject lambertWig;
    public bool lambertQuestComplete;

    [Header("Twins")]
    public GameObject twinsCamerinoDoor;
    public CamerinoDoorBehaviour twinsCamerinoDoorBehaviour;
    public GameObject twinsDoorTrigger;

    public void LambertInitaited() {
        // bool
        lambertInitiated = true;
        // activate pushpulls
        foreach (GameObject pushPullObject in pushPullObjects) {
            pushPullObject.tag = "PushPull";
            pushPullObject.layer = 9;
        }
        // activate interactives
        foreach (GameObject interactiveObject in interactiveObjects) {
            interactiveObject.layer = 9;
        }
    }

    public void Wig() {
        // bool
        wigSnatched = true;
        // narration
        wigNarration.enabled = true;
        // reset camerino door
        lambertCamerinoDoorBehaviour.TriggerLight();
        lambertCamerinoDoorBehaviour.doorSwitch = false;
        lambertCamerinoDoorBehaviour.TriggerDoor();
        lambertCamerino.layer = 0;
        // open principal doors
        foreach (GameObject principalDoor in principalDoors) {
            principalDoor.tag = "Untagged";
            principalDoor.GetComponent<DoorBehaviour>().TriggerDoor();
        }
        // place lambert in principal
        // Instantiate(lambertPrefab, lambertPrincipalPos.position, lambertPrincipalPos.rotation);
    }

    public void LambertQuestComplete() {
        lambertWig.SetActive(true);
        twinsCamerinoDoor.layer = 9;
        twinsDoorTrigger.SetActive(true);
    }
}