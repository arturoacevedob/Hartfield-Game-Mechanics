using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class Collectable : MonoBehaviour {

    AudioSource _audioSource;
    public AudioClip _audioClip;
    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
    }

    public float Behaviour() {
        if (_audioClip != null) {
            _audioSource.PlayOneShot(_audioClip);
            return _audioClip.length;
        } else {
            return 0f;
        }
    }
}