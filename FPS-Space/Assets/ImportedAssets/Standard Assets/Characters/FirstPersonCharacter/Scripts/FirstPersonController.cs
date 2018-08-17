using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private float m_walkSpeed;
        [SerializeField] private float m_runSpeed;
        [SerializeField] private float m_crouchSpeed;
        [SerializeField] private float m_crouchHeadDistance;
        [SerializeField] [Range(0f, 1f)] private float m_runstepLenghten;
        [SerializeField] private float m_jumpSpeed;
        [SerializeField] private float m_stickToGroundForce;
        [SerializeField] private float m_gravityMultiplier;
        [SerializeField] private float m_stepInterval;
        [SerializeField] private AudioClip[] m_footstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_jumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_landSound;           // the sound played when character touches back on ground.
        [SerializeField] private FirstPersonCamera m_firstPersonCamera;
        
        
        private float m_YRotation;
        private Vector2 m_input;
        private Vector3 m_moveDir = Vector3.zero;
        private CharacterController m_characterController;
        private CollisionFlags m_collisionFlags;
        private bool m_previouslyGrounded;
        private float m_stepCycle;
        private float m_nextStep;
        private bool m_jumping;
        private AudioSource m_audioSource;
        
        private Command m_noCommand, m_buttonW, m_buttonS, m_buttonA, m_buttonD, m_buttonC, m_buttonShift, m_buttonSpace;
        public static float m_horizontal;
        public static float m_vertical;
        public static bool m_isWalking;
        public static bool m_isCrouching;
        public static bool m_jump;

        // Use this for initialization
        private void Start()
        {
            m_characterController = GetComponent<CharacterController>();
            m_stepCycle = 0f;
            m_nextStep = m_stepCycle/2f;
            m_jumping = false;
            m_audioSource = GetComponent<AudioSource>();
            m_firstPersonCamera.SetHeadBob(m_stepInterval);
			
            // Commands
            m_noCommand = new NoCommand();
            m_buttonW = new MoveForward();
            m_buttonS = new MoveReverse();
            m_buttonA = new MoveLeft();
            m_buttonD = new MoveRight();
            m_buttonC = new SetIsCrouching();
            m_buttonShift = new SetIsWalking();
            m_buttonSpace = new SetJump();
        }


        // Update is called once per frame
        private void Update()
        {
            float speed;
            bool waswalking = m_isWalking;
            
            GetInput();
            SetSpeed(out speed);
            
            
            SetMoveDir(speed);
            ProgressStepCycle(speed);
            
            //call FirstPersonCamera.UpdateCameraPosition to update the camera position and do bobs
            if (m_isCrouching)
            {
                m_firstPersonCamera.UpdateCrouchCameraPosition(m_crouchHeadDistance);
            }
            else
            {
                m_firstPersonCamera.UpdateHeadBobCameraPosition(speed, m_characterController, m_runstepLenghten);
            }
            

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (!m_isCrouching && m_isWalking != waswalking && m_characterController.velocity.sqrMagnitude > 0)
            {
                m_firstPersonCamera.DoFoveKick();
            }
            

            if (!m_previouslyGrounded && m_characterController.isGrounded)
            {
                Land();
            }
            
            if (!m_characterController.isGrounded && !m_jumping && m_previouslyGrounded)
            {
                NotJump();
            }

            m_previouslyGrounded = m_characterController.isGrounded;
            
        }
        
        
        private void FixedUpdate()
        {

            if (m_characterController.isGrounded)
            {
                Ground();

                if (m_jump)
                {
                    Jump();
                }
            }
            else
            {
                Fall();
            }

            Move();

        }
        

        private void SetMoveDir(float speed)
        {
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_input.y + transform.right*m_input.x;
            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_characterController.radius, Vector3.down, out hitInfo,
                m_characterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;
            
            m_moveDir.x = desiredMove.x*speed;
            m_moveDir.z = desiredMove.z*speed;
        }
        
        
        private void Move()
        {
            m_collisionFlags = m_characterController.Move(m_moveDir*Time.fixedDeltaTime);
        }
        
        
        private void Land()
        {
            m_firstPersonCamera.DoJumpBob();
            PlayLandingSound();
            m_moveDir.y = 0f;
            m_jumping = false;
        }

        private void PlayLandingSound()
        {
            m_audioSource.clip = m_landSound;
            m_audioSource.Play();
            m_nextStep = m_stepCycle + .5f;
        }
        
        
        private void Fall()
        {
            m_moveDir += Physics.gravity*m_gravityMultiplier*Time.fixedDeltaTime;
        }
        
        
        private void Jump()
        {
            m_moveDir.y = m_jumpSpeed;
            PlayJumpSound();
            m_jump = false;
            m_jumping = true;
        }
        
        private void NotJump()
        {
            m_moveDir.y = 0f;
        }
        
        private void PlayJumpSound()
        {
            m_audioSource.clip = m_jumpSound;
            m_audioSource.Play();
        }
        
        
        private void Ground()
        {
            m_moveDir.y = -m_stickToGroundForce;
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_characterController.velocity.sqrMagnitude > 0 && (m_input.x != 0 || m_input.y != 0))
            {
                m_stepCycle += (m_characterController.velocity.magnitude + (speed*(m_isWalking ? 1f : m_runstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_stepCycle > m_nextStep))
            {
                return;
            }

            m_nextStep = m_stepCycle + m_stepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_characterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_footstepSounds.Length);
            m_audioSource.clip = m_footstepSounds[n];
            m_audioSource.PlayOneShot(m_audioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_footstepSounds[n] = m_footstepSounds[0];
            m_footstepSounds[0] = m_audioSource.clip;
        }



        private void GetInput()
        {
            // Read input
            
            m_horizontal = 0;
            m_vertical = 0;
            
#if !MOBILE_INPUT
            
            if (Input.GetKey(KeyCode.W))
            {
                m_buttonW.Execute();
            }
            if (Input.GetKey(KeyCode.S))
            {
                m_buttonS.Execute();
            }
            if (Input.GetKey(KeyCode.A))
            {
                m_buttonA.Execute();
            }
            if (Input.GetKey(KeyCode.D))
            {
                m_buttonD.Execute();
            }

            if (!m_jump && CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                m_buttonSpace.Execute();
            }

            // On standalone builds, walk/run/crouch speed is modified by a key press.
            // keep track of whether the character is walking, running, or crouching
            //m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
            if (Input.GetKey(KeyCode.C))
            {
                m_buttonC.Execute();
            }
            else
            {
                m_buttonC.Undo();
            }
            
            if (Input.GetKey(KeyCode.LeftShift))
            {
                m_buttonShift.Execute();
            }
            else
            {
                m_buttonShift.Undo();
            }
            
#endif

            m_input = new Vector2(m_horizontal, m_vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_input.sqrMagnitude > 1)
            {
                m_input.Normalize();
            }
        }

        
        // set the desired speed to be walking, running, or crouching
        private void SetSpeed(out float speed)
        {
            if (m_isCrouching)
            {
                speed = m_crouchSpeed;
            }
            else if (m_isWalking)
            {
                speed = m_walkSpeed;
            }
            else
            {
                speed = m_runSpeed;   
            }
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_collisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_characterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
