using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]

public class SecretDoorBehaviour : MonoBehaviour {

    Animator _animation;
    public bool doorSwitch = true;
    public AudioClip open;
    public AudioClip close;
    public AudioClip locked;
    AudioSource _audioSource;
    public CameraSwitch CameraSwitch;

    void Awake() {
        _animation = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void TriggerDoor(bool flag) {
        if (flag) OpenDoor();
        else CloseDoor();
    }

    public void OpenDoor() {
        _animation.SetBool("Open", doorSwitch);
        CameraSwitch.CaptureOriginalCamera();
        CameraSwitch.Enable();
        doorSwitch = !doorSwitch;
    }

    public void CloseDoor() {
        _animation.SetBool("Open", doorSwitch);
        CameraSwitch.Disable();
        doorSwitch = !doorSwitch;
    }

    public void PlayOpen() {
        _audioSource.PlayOneShot(open);
    }

    public void PlayClose() {
        _audioSource.PlayOneShot(close);
    }

    public void PlayLocked() {
        _audioSource.PlayOneShot(locked);
    }
}