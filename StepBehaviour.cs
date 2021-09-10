using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ladder.Scripts;

public class StepBehaviour : MonoBehaviour {

	public List<GroundType> groundTypes = new List<GroundType> ();
	public string currentGround;
	public CustomFirstPersonController Player;

	void Start ()
	{
		setGroundType (groundTypes [0]);
	}

	public void OnControllerColliderHit (ControllerColliderHit hit)
	{
		if (hit.transform.tag == "Wood")
			setGroundType (groundTypes [1]);
		else if (hit.transform.tag == "Dirt")
			setGroundType (groundTypes [2]);
        else if (hit.transform.tag == "Gravel")
            setGroundType(groundTypes[3]);
        else
			setGroundType (groundTypes [0]);
	}

	public void setGroundType(GroundType ground)
	{
		if (currentGround != ground.name) {
			Player.m_FootstepSounds = ground.footstepSounds;
			Player.m_WalkSpeed = ground.walkSpeed;
			Player.m_RunSpeed = ground.runSpeed;
			currentGround = ground.name;
		}
	}
}

[System.Serializable]
public class GroundType {
	public string name;
	public AudioClip [] footstepSounds;
	public float walkSpeed = 5;
	public float runSpeed = 10;
}