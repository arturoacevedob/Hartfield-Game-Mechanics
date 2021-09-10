using System.Collections;
using Ladder.Scripts;
using Narrate;
using UnityEngine;
using UnityEngine.UI;

public class RaycastSystem : MonoBehaviour {

    // Input Bools
    public bool isAPressed;
    public bool isBPressed;

    bool doorSelected;
    bool camerinoDoorSelected;
    bool keyDoorSelected;
    bool secretDoorSelected;
    bool collectableSelected;
    bool pickUpSelected;
    bool pushPullSelected;
    bool lambertSelected;

    // Raycast setup — NEEDS INTERACTIVE LAYER
    public int rayLenght = 10;
    private GameObject hitObject;
    private RaycastHit hitRaycast;
    private readonly int layerMask = 1 << 9 | 1 << 11;

    // PushPull setup;
    public float frontBackIndex;
    public float rightLeftIndex;
    GameObject PushPullObject;
    PushPullBehaviour PushPullObjectBehaviour;
    public GameObject Player;
    private float pushPullLenght;
    private float pushPullPlayerDest;
    private Transform currentPushPull;
    private string currentOrientation;

    // Instructions UI Setup
    public Text Instructions;
    int caseSwitch;

    // Pickup setup — NEEDS PICKUP TAG
    private Transform currentPickup;
    private float pickupLenght;

    // Switches
    private bool pickupSwitch;
    private bool SecretDoorSwitch = true;

    // LERP Vars
    private float lerpSpeed = 0.8f;
    private float lerpFraction;

    // External Scripts
    public StoryTracker storyTracker;
    Collectable collectable;
    CustomFirstPersonController PlayerMovement;

    GameObject lastHighlightedObject;

    private void Awake() {
        PlayerMovement = Player.GetComponent<CustomFirstPersonController>();
    }

    // Highlight System
    private void HighlightObject(GameObject item) {
        if (lastHighlightedObject != item) {
            ClearHighlighted();
            item.GetComponent<Renderer>().material.color = new Color32(220, 220, 220, 255);
            lastHighlightedObject = item;
        }
    }
    private void ClearHighlighted() {
        if (lastHighlightedObject != null) {
            lastHighlightedObject.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
            lastHighlightedObject = null;
            hitObject = null;
        }
    }

    private void Update() {
        // Cursor State
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Input Management
        isAPressed = Input.GetButtonDown("A");
        isBPressed = Input.GetButtonDown("B");

        if (hitObject != null) {
            if (doorSelected) {
                DoorBehaviour door = hitObject.GetComponent<DoorBehaviour>();
                caseSwitch = door.doorSwitch ? 1 : 2;
                InstructionSystem(caseSwitch);
                if (isAPressed) {
                    door.TriggerDoor();
                }
            }

            if (camerinoDoorSelected) {
                CamerinoDoorBehaviour door = hitObject.GetComponent<CamerinoDoorBehaviour>();
                caseSwitch = door.doorSwitch ? 1 : 2;
                InstructionSystem(caseSwitch);
                if (isAPressed) {
                    if (hitObject.GetComponent<OnEnableNarrationTrigger>())
                        hitObject.GetComponent<OnEnableNarrationTrigger>().enabled = true;
                    hitObject.GetComponent<StoryTrackerEntry>().Set();
                    door.TriggerDoor();
                }
            }

            if (keyDoorSelected) {
                DoorBehaviour door = hitObject.GetComponent<DoorBehaviour>();
                if (storyTracker.key) {
                    caseSwitch = door.doorSwitch ? 1 : 2;
                    InstructionSystem(caseSwitch);
                    if (isAPressed && storyTracker.hat) {
                        door.TriggerDoor();
                        storyTracker.key = false;
                        hitObject.tag = "Door";
                    } else if (isAPressed) {
                        hitObject.GetComponent<OnEnableNarrationTrigger>().enabled = true;
                    }
                } else if (isAPressed) {
                    door.PlayLocked();
                } else InstructionSystem(6);
            }

            if (secretDoorSelected) {
                SecretDoorBehaviour secretDoor = hitObject.GetComponent<SecretDoorBehaviour>();
                if (storyTracker.rugCollected) {
                    caseSwitch = secretDoor.doorSwitch ? 1 : 2;
                    InstructionSystem(caseSwitch);
                    if (isAPressed) {
                        secretDoor.TriggerDoor(SecretDoorSwitch);
                        SecretDoorSwitch = !SecretDoorSwitch;
                    }
                } else InstructionSystem(default);
            }

            if (collectableSelected) {
                InstructionSystem(5);
                if (isAPressed) {
                    if (hitObject.name == "Hat") {
                        hitObject.GetComponent<StoryTrackerEntry>().Set();
                        GameObject.Find("HatVoiceOver").GetComponent<OnEnableNarrationTrigger>().enabled = true;
                    }
                    hitObject.GetComponent<StoryTrackerEntry>().Set();
                    hitObject.GetComponent<MeshRenderer>().enabled = false;
                    collectable = hitObject.GetComponent<Collectable>();
                    Object.Destroy(hitObject, collectable.Behaviour());
                    Instructions.text = "";
                }
            }

            if (pickUpSelected) {
                caseSwitch = !pickupSwitch ? 3 : 4;
                InstructionSystem(caseSwitch);

                // Pickup Move
                if (isAPressed && !pickupSwitch) {
                    SetPickup(hitObject.transform);
                    if (hitObject.GetComponent<SFXSystem>())
                        hitObject.GetComponent<SFXSystem>().PlayPickup();
                    pickupSwitch = true;
                }
            }

            if (pushPullSelected) {
                PushPullObjectBehaviour = hitObject.GetComponent<PushPullBehaviour>();
                caseSwitch = !PushPullObjectBehaviour.pushPullSwitch ? 7 : 8;
                InstructionSystem(caseSwitch);

                // PushPull Set
                if (isAPressed && !PushPullObjectBehaviour.pushPullSwitch) {
                    PushPullObject = hitObject;
                    SetPushPull(PushPullObject.transform, FindOrientation());
                    if (hitObject.GetComponent<SFXSystem>())
                        hitObject.GetComponent<SFXSystem>().PlayPickup();
                    PushPullObjectBehaviour.pushPullSwitch = true;
                }
            }

            if (lambertSelected) {
                InstructionSystem(9);
                if (isAPressed) {
                    hitObject.GetComponent<OnEnableNarrationTrigger>().enabled = true;
                    hitObject.tag = "Untagged";
                    hitObject.layer = 0;
                    storyTracker.LambertQuestComplete();
                }
            }

        } else Instructions.text = "";



        // Finds camera orientation
        FindOrientation();

        // PushPull Move
        if (currentPushPull) MovePushPull(currentOrientation);
        // PushPull Drop
        if (currentPushPull) {
            if (isBPressed) {
                DropPushPull();
                StartCoroutine(ResetFPS());
                PushPullObjectBehaviour.pushPullSwitch = false;
            }
        }

        // Pickup Move
        if (currentPickup) MovePickup();
        // Pickup Drop
        if (pickupSwitch) {
            if (isBPressed) {
                DropPickup();
                pickupSwitch = false;
            }
        }
    }

    private void FixedUpdate() {

        // Raycast System
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitRaycast, rayLenght, layerMask)) {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hitRaycast.distance, Color.red);

            // Memory for selected object and visual indicator
            hitObject = hitRaycast.collider.gameObject;
            HighlightObject(hitObject);

            doorSelected = hitRaycast.collider.gameObject.tag == "Door";
            camerinoDoorSelected = hitRaycast.collider.gameObject.tag == "CamerinoDoor";
            keyDoorSelected = hitRaycast.collider.gameObject.tag == "KeyDoor";
            secretDoorSelected = hitRaycast.collider.gameObject.tag == "SecretDoor";
            collectableSelected = hitRaycast.collider.gameObject.tag == "Collectable";
            pickUpSelected = hitRaycast.collider.gameObject.tag == "PickUp";
            pushPullSelected = hitRaycast.collider.gameObject.tag == "PushPull" && FindOrientation() != "";
            lambertSelected = hitRaycast.collider.gameObject.tag == "Lambert";
        } else {
            // Deletes memory for selected object and returns its color to normal
            ClearHighlighted();
        }

    }

    // Pickup Subsystems
    private void SetPickup(Transform newPickup) {
        if (currentPickup) return;
        currentPickup = newPickup;
        pickupLenght = Vector3.Distance(transform.position, currentPickup.position);
        currentPickup.GetComponent<Rigidbody>().useGravity = false;
    }
    private void MovePickup() {
        if (lerpFraction < 1) {
            lerpFraction += Time.deltaTime * lerpSpeed;
            currentPickup.position = Vector3.Lerp(currentPickup.position, transform.position + transform.forward * pickupLenght, lerpFraction);
        } else {
            currentPickup.position = transform.position + transform.forward * pickupLenght;
        }
    }
    private void DropPickup() {
        if (!currentPickup) return;
        Rigidbody currentPickupRB = currentPickup.GetComponent<Rigidbody>();
        currentPickupRB.useGravity = true;
        currentPickupRB.velocity = Vector3.zero;
        currentPickup.parent = null;
        currentPickup = null;
        lerpFraction = 0;
    }

    // Instruction System Switch
    public void InstructionSystem(int caseSwitch) {
        switch (caseSwitch) {
            case 1:
                Instructions.text = "Press A to open";
                break;
            case 2:
                Instructions.text = "Press A to close";
                break;
            case 3:
                Instructions.text = "Press A to pick up";
                break;
            case 4:
                Instructions.text = "Press B to drop";
                break;
            case 5:
                Instructions.text = "Press A to collect";
                break;
            case 6:
                Instructions.text = "Door is locked";
                break;
            case 7:
                Instructions.text = "Press A to move";
                break;
            case 8:
                Instructions.text = "Press B to release";
                break;
            case 9:
                Instructions.text = "Press A to place wig";
                break;
            default:
                Instructions.text = "";
                break;
        }
    }

    // Pickup Subsystems
    public string FindOrientation() {
        frontBackIndex = Vector3.Dot(transform.forward, Vector3.forward);
        rightLeftIndex = Vector3.Dot(transform.forward, Vector3.right);
        if (Mathf.Abs(frontBackIndex) > 0.05f) {
            if (Mathf.Abs(frontBackIndex) > Mathf.Abs(rightLeftIndex)) {
                if (frontBackIndex > 0) return "z+";
                else return "z-";
            }
        }
        if (Mathf.Abs(rightLeftIndex) > 0.05f) {
            if (Mathf.Abs(frontBackIndex) < Mathf.Abs(rightLeftIndex)) {
                if (rightLeftIndex > 0) return "x+";
                else return "x-";
            }
        }
        return "";
    }
    public void SetPushPull(Transform newTransform, string Orientation) {
        if (currentPushPull) return;
        currentPushPull = newTransform;
        currentOrientation = Orientation;
        if (currentOrientation == "z+" || currentOrientation == "z-")
            pushPullLenght = Mathf.Abs(currentPushPull.transform.position.z - Player.transform.position.z);
        else pushPullLenght = Mathf.Abs(currentPushPull.transform.position.x - Player.transform.position.x);
        Player.transform.position = new Vector3(currentPushPull.position.x, Player.transform.position.y, Player.transform.position.z);
    }
    public void MovePushPull(string currentOrientation) {
        switch (currentOrientation) {
            case "z+":
                PlayerMovement.m_WalkSpeed = 2;
                Player.transform.position = new Vector3(currentPushPull.position.x, Player.transform.position.y, Player.transform.position.z);
                currentPushPull.position = new Vector3(currentPushPull.position.x,
                                                       currentPushPull.position.y,
                                                       Player.transform.position.z + pushPullLenght);
                break;
            case "z-":
                PlayerMovement.m_WalkSpeed = 2;
                Player.transform.position = new Vector3(currentPushPull.position.x, Player.transform.position.y, Player.transform.position.z);
                currentPushPull.position = new Vector3(currentPushPull.position.x,
                                                       currentPushPull.position.y,
                                                       Player.transform.position.z - pushPullLenght);
                break;
            case "x+":
                PlayerMovement.m_WalkSpeed = 2;
                Player.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, currentPushPull.position.z);
                currentPushPull.position = new Vector3(Player.transform.position.x + pushPullLenght,
                                                       currentPushPull.position.y,
                                                       currentPushPull.position.z);
                break;
            case "x-":
                PlayerMovement.m_WalkSpeed = 2;
                Player.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y, currentPushPull.position.z);
                currentPushPull.position = new Vector3(Player.transform.position.x - pushPullLenght,
                                                       currentPushPull.position.y,
                                                       currentPushPull.position.z);
                break;
        }
    }
    public void DropPushPull() {
        if (!currentPushPull) return;
        PushPullObjectBehaviour.TriggerMove();
        PlayerMovement.enabled = false;
        currentPushPull = null;
        currentOrientation = null;
    }
    IEnumerator ResetFPS() {
        yield return 0;
        PlayerMovement.enabled = true;
        PlayerMovement.m_WalkSpeed = 5;
    }
}