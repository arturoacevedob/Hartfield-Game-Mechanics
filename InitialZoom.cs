using System.Collections;
using Ladder.Scripts;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class InitialZoom : MonoBehaviour {

    public Transform playerCamera;
    public Transform InitialZoomStart;
    public Transform InitialZoomEnd;
    public AudioSource radio;
    public PostProcessVolume trainVolume;
    CustomFirstPersonController Player;
    public AnimationCurve MoveCurve;

    void Start() {
        Player = GetComponent<CustomFirstPersonController>();
        Player.enabled = false;
        playerCamera.position = InitialZoomStart.position;
        playerCamera.rotation = InitialZoomStart.rotation;
        StartCoroutine(FadeIn());
        StartCoroutine(IntroMusic());
        StartCoroutine(InitialZoomAnimation());
    }

    public IEnumerator FadeIn() {
        float lerpFraction = -10;
        ColorGrading _colorGrading;
        if (trainVolume.profile.TryGetSettings(out _colorGrading)) {
            while (lerpFraction < 1) {
                lerpFraction += Time.deltaTime;
                _colorGrading.postExposure.value = lerpFraction;
                yield return null;
            }
        }
    }

    public IEnumerator InitialZoomAnimation() {
        yield return new WaitForSeconds(8);
        float lerpFraction = 0;
        while (lerpFraction < 1) {
            lerpFraction += Time.deltaTime * 0.045f;
            playerCamera.position = Vector3.Lerp(InitialZoomStart.position, InitialZoomEnd.position, MoveCurve.Evaluate(lerpFraction));
            playerCamera.rotation = Quaternion.Lerp(InitialZoomStart.rotation, InitialZoomEnd.rotation, MoveCurve.Evaluate(lerpFraction));
            yield return null;
        }
        Player.enabled = true;
    }

    public IEnumerator IntroMusic() {
        float lerpFraction = 0;
        while (lerpFraction < 1) {
            lerpFraction += Time.deltaTime * 0.033f;
            radio.spatialBlend = lerpFraction;
            yield return null;
        }
    }
}