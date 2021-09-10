using UnityEngine;
using System.Collections;
using Narrate;

public class WigPlacer : MonoBehaviour {

    public GameObject lambertPrincipal;
    private ProximityNarrationTrigger narration;

    private void Awake() {
        narration = GetComponent<ProximityNarrationTrigger>();
    }

    private void Update() {
        if (!narration.enabled && narration) {
            StartCoroutine(LambertQuestComplete(24));
        }
    }

    public IEnumerator LambertQuestComplete(int time) {
        yield return new WaitForSeconds(time);
        lambertPrincipal.tag = "Lambert";
        lambertPrincipal.layer = 9;
        Destroy(narration);
    }
}
