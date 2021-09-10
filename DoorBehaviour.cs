using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]

public class DoorBehaviour : MonoBehaviour {

    Animator _animation;
    public bool doorSwitch = true;
    public AudioClip open;
    public AudioClip close;
    public AudioClip locked;
    AudioSource _audioSource;

    void Awake() {
        _animation = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void TriggerDoor() {
        _animation.SetBool("Open", doorSwitch);
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