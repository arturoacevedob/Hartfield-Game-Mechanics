using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace Ladder.Scripts {
    /// <summary>
    /// FPS player controller that has the ability to climb ladders
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    public class CustomFirstPersonController : MonoBehaviour {
        [SerializeField]
        public bool m_IsWalking;
        [SerializeField]
        public float m_WalkSpeed;
        [SerializeField]
        public float m_RunSpeed;
        [SerializeField]
        [Range(0f, 1f)]
        public float m_RunstepLenghten;
        [SerializeField]
        public float m_JumpSpeed;
        [SerializeField]
        public float m_StickToGroundForce;
        [SerializeField]
        public float m_GravityMultiplier;
        [SerializeField]
        public MouseLook m_MouseLook;
        [SerializeField]
        public bool m_UseFovKick;
        [SerializeField]
        public FOVKick m_FovKick = new FOVKick();
        [SerializeField]
        public bool m_UseHeadBob;
        [SerializeField]
        public CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField]
        public LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField]
        public float m_StepInterval;
        [SerializeField]
        public AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField]
        public AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField]
        public AudioClip m_LandSound;           // the sound played when character touches back on ground.

        public Camera m_Camera;
        public bool m_Jump;
        public float m_YRotation;
        public Vector2 m_Input;
        public Vector3 m_MoveDir = Vector3.zero;
        public float dynamicInput;
        public CharacterController m_CharacterController;
        public CollisionFlags m_CollisionFlags;
        public bool m_PreviouslyGrounded;
        public Vector3 m_OriginalCameraPosition;
        public float m_StepCycle;
        public float m_NextStep;
        public bool m_Jumping;
        public AudioSource m_AudioSource;

        public float m_ClimbSpeed; // How fast does the player climb the ladder

        public bool m_isClimbing = false; // Are we currently climbing?

        public Transform m_ladderTrigger; // The current trigger we hit for the ladder

        // Use this for initialization
        public void Start() {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
            m_ClimbSpeed = 3.0f;
            m_AudioSource = GetComponent<AudioSource>();
            m_MouseLook.Init(transform, m_Camera.transform);
            m_MouseLook.SwitchInputs();
        }


        // Update is called once per frame
        public void Update() {
            RotateView();
            ControllerSwitch();

            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump) {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded && m_isClimbing) {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded) {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }

        public void ControllerSwitch()
        {
            dynamicInput = m_MouseLook.xboxController ? m_Input.sqrMagnitude : 1f;
        }

        public void PlayLandingSound() {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }

        /// <summary>
        /// transition state for ladder climbing
        /// </summary>
        public enum TransitionState {
            None = 0,
            ToLadder1 = 1,
            ToLadder2 = 2,
            ToLadder3 = 3
        }

        public TransitionState _climbingTransition = TransitionState.None;

        public void FixedUpdate() {
            float speed;
            GetInput(out speed);

            Vector3 desiredMove = Vector3.zero;

            // Ladder climbing logic
            if (m_isClimbing) {
                Transform trLadder = m_ladderTrigger.parent;

                if (_climbingTransition != TransitionState.None) {
                    // Get the next point to which we have to move while we are climbing the ladder
                    transform.position = trLadder.Find(_climbingTransition.ToString()).position;
                    _climbingTransition = TransitionState.None;
                } else {

                    // Attach the player to the ladder with the rotation angle of the ladder transform
                    desiredMove = trLadder.rotation * Vector3.forward * m_Input.y;

                    m_MoveDir.y = desiredMove.y * m_ClimbSpeed;
                    m_MoveDir.x = desiredMove.x * m_ClimbSpeed;
                    m_MoveDir.z = desiredMove.z * m_ClimbSpeed;

                    m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
                }
            } else {
                // always move along the camera forward as it is the direction that it being aimed at
                desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

                // get a normal for the surface that is being touched to move along it
                RaycastHit hitInfo;
                Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                                   m_CharacterController.height / 2f);
                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

                m_MoveDir.x = desiredMove.x * dynamicInput * speed;
                m_MoveDir.z = desiredMove.z * dynamicInput * speed;


                if (m_CharacterController.isGrounded) {
                    m_MoveDir.y = -m_StickToGroundForce;

                    if (m_Jump) {
                        m_MoveDir.y = m_JumpSpeed;
                        PlayJumpSound();
                        m_Jump = false;
                        m_Jumping = true;
                    }
                } else {
                    m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
                }

                m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

                ProgressStepCycle(speed);
                UpdateCameraPosition(speed);
            }
        }

        public void PlayJumpSound() {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        public void ProgressStepCycle(float speed) {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0)) {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? dynamicInput : m_RunstepLenghten))) *
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep)) {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        public void PlayFootStepAudio() {
            if (!m_CharacterController.isGrounded) {
                return;
            }

            if (m_FootstepSounds.Length == 0)
                return;

            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        public void UpdateCameraPosition(float speed) {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob) {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded) {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed * (m_IsWalking ? dynamicInput : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            } else {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        public void GetInput(out float speed) {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            /*// normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1) {
                m_Input.Normalize();
            }*/

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0) {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }


        public void RotateView() {

            m_MouseLook.LookRotation(transform, m_Camera.transform, null);

        }

        void OnTriggerEnter(Collider other) {
            if (other.tag == "Ladder_Bottom") {
                m_ladderTrigger = other.transform;

                if (!m_isClimbing) {
                    _climbingTransition = TransitionState.ToLadder1;
                    ToggleClimbing();
                } else {
                    ToggleClimbing();
                    _climbingTransition = TransitionState.None;
                }
            } else if (other.tag == "Ladder_Top") {
                m_ladderTrigger = other.transform;

                // We hit the top trigger and come from the ladder
                if (m_isClimbing) {
                    // move to the upper point and exit the ladder
                    _climbingTransition = TransitionState.ToLadder3;
                } else {
                    // We seem to come from above, so let's move to tha ladder (point 2) again
                    _climbingTransition = TransitionState.ToLadder2;
                }

                ToggleClimbing();
            }
        }

        public void ToggleClimbing() {
            m_isClimbing = !m_isClimbing;
        }

        public void OnControllerColliderHit(ControllerColliderHit hit) {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below) {
                return;
            }

            if (body == null || body.isKinematic) {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
