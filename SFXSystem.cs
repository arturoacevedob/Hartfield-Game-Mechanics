using UnityEngine;

[RequireComponent (typeof (AudioSource))]

public class SFXSystem : MonoBehaviour {
	AudioSource audioSource;
	public AudioClip pickup;
	public AudioClip drop;

	private void Awake ()
	{
		audioSource = GetComponent<AudioSource> ();
	}

	public void PlayPickup ()
	{
		if (pickup) audioSource.PlayOneShot (pickup);
	}

	public void PlayDrop ()
	{
		if (drop) audioSource.PlayOneShot (drop);
	}

	private void OnCollisionEnter (Collision collision)
	{
		if (collision.relativeVelocity.magnitude > 3) {
			audioSource.PlayOneShot (drop);
		} // Terrain AMO
	}
}