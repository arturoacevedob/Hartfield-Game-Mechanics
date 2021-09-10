using UnityEngine;
using Ladder.Scripts;
using System.Collections;
using UnityStandardAssets.Utility;

public class CameraSwitch : MonoBehaviour {

    public GameObject playerCamera;
    public Transform playerCameraTransform;
    public Transform targetCamera;
    private Vector3 originalCameraPosition;
    private Quaternion originalCameraRotation;
    CustomFirstPersonController Player;

    public bool cameraSwitch;
    private bool scriptAdded;

    SimpleMouseRotator FreeLookCamera;

    public AnimationCurve MoveCurve;

    private void Awake() {
        Player = GetComponent<CustomFirstPersonController>();
        FreeLookCamera = playerCamera.GetComponent<SimpleMouseRotator>();
    }

    public void CaptureOriginalCamera() {
        originalCameraPosition = playerCameraTransform.position;
        originalCameraRotation = playerCameraTransform.rotation;
    }

    public void Enable() {
        StartCoroutine(LerpOpen());
    }

    public void Disable() {
        StartCoroutine(LerpClose());
    }

    IEnumerator LerpOpen() {
        Player.enabled = false;
        float lerpFraction = 0;
        while (lerpFraction < 1) {
            lerpFraction += Time.deltaTime / 1.5f;
            playerCameraTransform.position = Vector3.Lerp(playerCameraTransform.position, targetCamera.position, MoveCurve.Evaluate(lerpFraction));
            playerCameraTransform.rotation = Quaternion.Lerp(playerCameraTransform.rotation, targetCamera.rotation, MoveCurve.Evaluate(lerpFraction));
            yield return null;
        }
        FreeLookCamera.enabled = true;
    }

    IEnumerator LerpClose() {
        FreeLookCamera.enabled = false;
        float lerpFraction = 0;
        while (lerpFraction < 1) {
            lerpFraction += Time.deltaTime / 0.5f;
            playerCameraTransform.position = Vector3.Lerp(targetCamera.position, originalCameraPosition, MoveCurve.Evaluate(lerpFraction));
            playerCameraTransform.rotation = Quaternion.Lerp(targetCamera.rotation, originalCameraRotation, MoveCurve.Evaluate(lerpFraction));
            yield return null;
        }
        Player.enabled = true;
    }
}