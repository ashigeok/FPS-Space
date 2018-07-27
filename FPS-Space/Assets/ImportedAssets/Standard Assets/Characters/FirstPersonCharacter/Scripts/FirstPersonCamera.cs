using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

namespace UnityStandardAssets.Characters.FirstPerson
{
	public class FirstPersonCamera : MonoBehaviour
	{
		
		[SerializeField] private MouseLook m_MouseLook;

		private static Camera m_Camera;
		public static Vector3 m_HitPosition;
		private static Vector3 m_OriginalCameraLocalPosition;
		

		// Use this for initialization
		void Start()
		{
			m_Camera = Camera.main;
			m_MouseLook.Init(transform , m_Camera.transform);
			m_OriginalCameraLocalPosition = m_Camera.transform.localPosition;
			

		}

		// Update is called once per frame
		void Update()
		{
			RotateView();
			Ray();
		}
		
		private static void Ray()
		{
			var ray = new Ray (m_Camera.transform.position, m_Camera.transform.forward);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit))
			{
				m_HitPosition = hit.point;
			}
			else
			{
				m_HitPosition = ray.GetPoint(100f);
			}
		}
		
		private void RotateView()
		{
			m_MouseLook.LookRotation (transform, m_Camera.transform);
		}
		
		public static void UpdateCameraPosition(float speed, bool useHeadBob, CharacterController characterController,
			CurveControlledBob headBob, LerpControlledBob jumpBob, float runstepLenghten, float crouchHeadDistance)
		{
			Vector3 newCameraPosition;
            
			if(FirstPersonController.m_IsCrouching)
			{
				newCameraPosition = m_Camera.transform.localPosition;
				newCameraPosition.y = m_OriginalCameraLocalPosition.y - crouchHeadDistance;
			}
			else if (useHeadBob && characterController.velocity.magnitude > 0 && characterController.isGrounded)
			{
				m_Camera.transform.localPosition =
					headBob.DoHeadBob(characterController.velocity.magnitude +
					                    (speed*(FirstPersonController.m_IsWalking ? 1f : runstepLenghten)));
				newCameraPosition = m_Camera.transform.localPosition;
				newCameraPosition.y = m_Camera.transform.localPosition.y - jumpBob.Offset();
			}
			else
			{
				newCameraPosition = m_Camera.transform.localPosition;
				newCameraPosition.y = m_OriginalCameraLocalPosition.y - jumpBob.Offset();
			}
			m_Camera.transform.localPosition = newCameraPosition;
		}
	}
}
