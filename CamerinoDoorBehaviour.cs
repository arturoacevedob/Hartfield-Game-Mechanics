using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]

public class CamerinoDoorBehaviour : MonoBehaviour {

    Animator _animation;
    public Material _materialLightOff;
    public Material _materialLightOn;
    Material _activeMaterial;
    Material _oldMaterial;
    Material[] newMaterials;
    Material[] oldMaterials;
    public bool doorSwitch = true;
    public bool lightSwitch = false;
    public AudioClip open;
    public AudioClip close;
    public AudioClip locked;
    AudioSource _audioSource;
    public AudioSource _externalAudioSource;
    public AudioClip lightStatic;

    public AudioMixerSnapshot nominal;
    public AudioMixerSnapshot lowpass;

    void Awake() {
        _animation = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        newMaterials = this.GetComponent<MeshRenderer>().materials;
        oldMaterials = this.GetComponent<MeshRenderer>().materials;
        _activeMaterial = newMaterials[1];
        _oldMaterial = _activeMaterial;

    }

    private void Update() {
        if (lightSwitch) {
            _externalAudioSource.PlayOneShot(lightStatic);
        }
        if (!lightSwitch) _externalAudioSource.mute = true;
        else _externalAudioSource.mute = false;
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

    public void TriggerLight() {
        _activeMaterial = lightSwitch == false ? _materialLightOn : _materialLightOff;
        newMaterials[1] = new Material(_activeMaterial);
        StartCoroutine(LightFlicker());
    }

    public void SwitchSnapshots() {
        if (doorSwitch) nominal.TransitionTo(0.5f);
        else lowpass.TransitionTo(0.5f);
    }

    IEnumerator LightFlicker() {
        this.GetComponent<MeshRenderer>().materials = newMaterials;
        lightSwitch = !lightSwitch;
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<MeshRenderer>().materials = oldMaterials;
        lightSwitch = !lightSwitch;
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<MeshRenderer>().materials = newMaterials;
        lightSwitch = !lightSwitch;
        yield return new WaitForSeconds(0.2f);
        this.GetComponent<MeshRenderer>().materials = oldMaterials;
        lightSwitch = !lightSwitch;
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<MeshRenderer>().materials = newMaterials;
        lightSwitch = !lightSwitch;
        yield return new WaitForSeconds(0.5f);
        this.GetComponent<MeshRenderer>().materials = oldMaterials;
        lightSwitch = !lightSwitch;
        yield return new WaitForSeconds(0.05f);
        this.GetComponent<MeshRenderer>().materials = newMaterials;
        lightSwitch = !lightSwitch;
    }
}