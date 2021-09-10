using UnityEngine;

public class TriggerDoorLight : MonoBehaviour {

    public GameObject camerinoDoor;
    CamerinoDoorBehaviour doorBehaviour;

    void Awake() {
        doorBehaviour = camerinoDoor.GetComponent<CamerinoDoorBehaviour>();
    }

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            doorBehaviour.TriggerLight();
            Object.Destroy(this);
        }
    }
}