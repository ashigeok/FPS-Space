using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Characters.FirstPerson
{
	public class GunManager : MonoBehaviour
	{
		[SerializeField] private GameObject m_muzzle;
		[SerializeField] private GameObject m_sparkle;
		[SerializeField] private AudioClip m_audioClip;
		[SerializeField] private FirstPersonCamera m_firstPersonCamera;
		
		private Vector3 m_hitPosition;
		private bool m_isHit;
		private bool m_isCoolTime;
		private const float m_coolTime = 0.5f;
		private const float m_offset = 0.1f;
		private GameObject m_muzzleFlash;
		private GameObject m_hitSparkle;
		private AudioSource m_audioSource;
		
		
		// Use this for initialization
		private void Start()
		{
			m_audioSource = GetComponent<AudioSource>();
		}

		private Vector3 GetFiringDirection()
		{
			// ターゲットオブジェクトとの差分を求め
			Vector3 temp =  m_firstPersonCamera.GetHitPosition() - m_muzzle.transform.position;
			// 正規化して方向ベクトルを求める
			Vector3 normal = temp.normalized;

			return normal;
		}

		// Update is called once per frame
		private void Update () {
			if (Input.GetMouseButton(0) && !m_isCoolTime)
			{
				RaycastHit hit;
				
				if (Physics.Raycast (m_muzzle.transform.position, GetFiringDirection(), out hit))
				{
					m_isHit = true;
					m_hitPosition = hit.point;
				}
				else
				{
					m_isHit = false;
				}
				
				Fire();
			}
			
		}


		private void Fire()
		{
			m_isCoolTime = true;
			FireSound();
			FireEffect();
			Invoke("SetCoolTime", m_coolTime);
		}
		
		private void FireSound()
		{
			m_audioSource.PlayOneShot(m_audioClip);
		}

		private void FireEffect ()
		{
			m_muzzleFlash = Instantiate(m_sparkle, m_muzzle.transform.position, m_muzzle.transform.rotation, m_muzzle.transform) as GameObject;
			Invoke("DestroyMuzzleFlash", 0.1f);
			
			if (m_isHit)
			{
				HitSparkleOffset();
				m_hitSparkle = Instantiate(m_sparkle, m_hitPosition, m_muzzle.transform.rotation) as GameObject;
				Invoke("DestroyHitSparkle", 0.1f);
			}
		}

		private void SetCoolTime()
		{
			m_isCoolTime = false;
		}

		private void DestroyMuzzleFlash()
		{
			Destroy(m_muzzleFlash);
		}
		
		private void DestroyHitSparkle()
		{
			Destroy(m_hitSparkle);
		}

		private void HitSparkleOffset()
		{
			if (m_hitPosition.x < m_muzzle.transform.position.x)
			{
				m_hitPosition.x += m_offset;
			}
			else
			{
				m_hitPosition.x -= m_offset;
			}
			if (m_hitPosition.y < m_muzzle.transform.position.y)
			{
				m_hitPosition.y +=m_offset;
			}
			else
			{
				m_hitPosition.y -= m_offset;
			}
			if (m_hitPosition.z < m_muzzle.transform.position.z)
			{
				m_hitPosition.z += m_offset;
			}
			else
			{
				m_hitPosition.z -= m_offset;
			}
		}
		

	}
}
