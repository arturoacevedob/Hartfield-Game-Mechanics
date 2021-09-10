using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]

public class PushPullBehaviour : MonoBehaviour {

    public bool pushPullSwitch = false;
    public Camera playerCamera;
    RaycastSystem RaycastSystem;

    private void Awake() {
        RaycastSystem = playerCamera.GetComponent<RaycastSystem>();
    }

    public void TriggerMove() {
        if (!pushPullSwitch) {
            RaycastSystem.FindOrientation();
        }
        pushPullSwitch = !pushPullSwitch;
    }
}