using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

namespace UnityStandardAssets.Characters.FirstPerson
{
	public class FirstPersonCamera : MonoBehaviour
	{
		
		[SerializeField] private MouseLook m_mouseLook;
		[SerializeField] private bool m_useFovKick;
		[SerializeField] private FOVKick m_fovKick = new FOVKick();
		[SerializeField] private bool m_useHeadBob;
		[SerializeField] private CurveControlledBob m_headBob = new CurveControlledBob();
		[SerializeField] private LerpControlledBob m_jumpBob = new LerpControlledBob();

		private Camera m_camera;
		private Vector3 m_originalCameraLocalPosition;


		private void Awake()
		{
			m_camera = Camera.main;
		}

		// Use this for initialization
		void Start()
		{
			m_mouseLook.Init(transform , m_camera.transform);
			m_originalCameraLocalPosition = m_camera.transform.localPosition;
			m_fovKick.Setup(m_camera);
		}


		// Update is called once per frame
		void Update()
		{
			RotateView();
		}

		
		public void SetHeadBob(float stepInterval)
		{
			m_headBob.Setup(m_camera, stepInterval);
		}

		
		private void RotateView()
		{
			m_mouseLook.LookRotation (transform, m_camera.transform);
		}


		public Vector3 GetHitPosition()
		{
			Vector3 hitPosition = new Vector3();
			var ray = new Ray (m_camera.transform.position, m_camera.transform.forward);
			RaycastHit hit;

			hitPosition = Physics.Raycast(ray, out hit) ? hit.point : ray.GetPoint(100f);

			return hitPosition;
		}
		
		
		public void UpdateCrouchCameraPosition(float crouchHeadDistance)
		{
			Vector3 newCameraPosition = m_camera.transform.localPosition;
			newCameraPosition.y = m_originalCameraLocalPosition.y - crouchHeadDistance;
		
			m_camera.transform.localPosition = newCameraPosition;
		}
		
		
		public void UpdateHeadBobCameraPosition(float speed, CharacterController characterController, float runstepLenghten)
		{
			Vector3 newCameraPosition;
			if (m_useHeadBob && characterController.velocity.magnitude > 0 && characterController.isGrounded)
			{
				m_camera.transform.localPosition =
					m_headBob.DoHeadBob(characterController.velocity.magnitude +
					                    (speed*(FirstPersonController.m_isWalking ? 1f : runstepLenghten)));
				newCameraPosition = m_camera.transform.localPosition;
				newCameraPosition.y = m_camera.transform.localPosition.y - m_jumpBob.Offset();
			}
			else
			{
				newCameraPosition = m_camera.transform.localPosition;
				newCameraPosition.y = m_originalCameraLocalPosition.y - m_jumpBob.Offset();
			}
			m_camera.transform.localPosition = newCameraPosition;
		}

		public void DoFoveKick()
		{
			if (!m_useFovKick)
			{
				return;
			}
			StopAllCoroutines();
			StartCoroutine(!FirstPersonController.m_isWalking ? m_fovKick.FOVKickUp() : m_fovKick.FOVKickDown());
		}

		public void DoJumpBob()
		{
			StartCoroutine(m_jumpBob.DoBobCycle());
		}
	}
}
