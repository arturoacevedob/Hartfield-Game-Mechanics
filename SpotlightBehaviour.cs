using UnityEngine;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(VolumetricLight))]
[RequireComponent(typeof(AudioSource))]

public class SpotlightBehaviour : MonoBehaviour {
    public AudioClip turnOn;
    public AudioClip turnOff;
    private AudioClip _audioClip;
    private Light _light;
    private VolumetricLight _volumetricLight;
    private AudioSource _audioSource;

    public void Awake() {
        _light = GetComponent<Light>();
        _volumetricLight = GetComponent<VolumetricLight>();
        _audioSource = GetComponent<AudioSource>();

        _light.enabled = false;
        _volumetricLight.enabled = false;
    }

    public void InitialState(bool OnOff) {
        _light.enabled = OnOff;
        _volumetricLight.enabled = OnOff;
    }

    public void TriggerLight(bool OnOff) {
        _light.enabled = OnOff;
        _volumetricLight.enabled = OnOff;
        _audioClip = OnOff ? turnOff : turnOn;
        _audioSource.PlayOneShot(_audioClip);
    }
}
