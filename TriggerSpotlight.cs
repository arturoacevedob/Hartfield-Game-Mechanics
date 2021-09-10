using System.Collections;
using UnityEngine;
using Narrate;

public class TriggerSpotlight : MonoBehaviour {

    public bool turnOnOffOnTrigger;
    public SpotlightBehaviour spotlight;
    public OnEnableNarrationTrigger narration;

    /*public void Awake() {
        spotlight.InitialState(!turnOnOffOnTrigger);
    }*/

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            spotlight.TriggerLight(turnOnOffOnTrigger);
            StartCoroutine(Wait(0.5f));
        }
    }

    public IEnumerator Wait(float seconds) {
        yield return new WaitForSeconds(seconds);
        narration.enabled = true;
        Object.Destroy(this);
    }
}