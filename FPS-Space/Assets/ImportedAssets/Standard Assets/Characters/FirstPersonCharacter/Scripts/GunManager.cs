using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace UnityStandardAssets.Characters.FirstPerson
{
	public class GunManager : MonoBehaviour
	{
		[SerializeField] private GameObject m_muzzle;
		[SerializeField] private GameObject m_sparkle;
		[SerializeField] private float m_coolTime;
		[SerializeField] private float m_sparkleLifeTime;
		[SerializeField] private float m_offset;
		[SerializeField] private int m_ammoLimit;
		[SerializeField] private int m_magazineSize;
		[SerializeField] private AudioClip m_fireSound;
		[SerializeField] private AudioClip m_reloadSound;
		[SerializeField] private FirstPersonCamera m_firstPersonCamera;

		private Vector3 m_hitPosition;
		private bool m_isHit;
		private bool m_isCoolTime;
		private GameObject m_muzzleFlash;
		private GameObject m_hitSparkle;
		private AudioSource m_audioSource;
		private int m_currentAmmo;
		private int m_bulletsInMagazine;
		
		// Use this for initialization
		private void Start()
		{
			m_audioSource = GetComponent<AudioSource>();
			m_currentAmmo = m_ammoLimit;
			m_bulletsInMagazine = m_magazineSize;
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
			Fire();
			Reload();
		}


		private void Fire()
		{
			if (!Input.GetMouseButton(0) || m_isCoolTime || m_bulletsInMagazine < 1) return;
			
			m_bulletsInMagazine -= 1;
			
			
			
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

			m_isCoolTime = true;
			FireSound();
			FireEffect();
			StartCoroutine (((Func<IEnumerator>)SetCoolTime).Method.Name);

		}
		
		private void FireSound()
		{
			m_audioSource.PlayOneShot(m_fireSound);
		}

		private void FireEffect ()
		{
			m_muzzleFlash = Instantiate(m_sparkle, m_muzzle.transform.position, m_muzzle.transform.rotation, m_muzzle.transform) as GameObject;
			StartCoroutine (((Func<IEnumerator>)DestroyMuzzleFlash).Method.Name);
			
			if (m_isHit)
			{
				HitSparkleOffset();
				m_hitSparkle = Instantiate(m_sparkle, m_hitPosition, m_muzzle.transform.rotation) as GameObject;
				StartCoroutine (((Func<IEnumerator>)DestroyHitSparkle).Method.Name);
			}
		}

		private IEnumerator SetCoolTime()
		{
			yield return new WaitForSeconds(m_coolTime);
			m_isCoolTime = false;
		}

		private IEnumerator DestroyMuzzleFlash()
		{
			yield return new WaitForSeconds(m_sparkleLifeTime);
			Destroy(m_muzzleFlash);
		}
		
		private IEnumerator DestroyHitSparkle()
		{
			yield return new WaitForSeconds(m_sparkleLifeTime);
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

		private void Reload()
		{
			if (!Input.GetKeyDown(KeyCode.R) || m_currentAmmo < 1
			                                 || m_bulletsInMagazine == m_magazineSize) return;
			
			m_audioSource.PlayOneShot(m_reloadSound);

			int i = (m_magazineSize - m_bulletsInMagazine);
			
			if (i < m_currentAmmo)
			{
				m_bulletsInMagazine += i;
				m_currentAmmo -= i;
			}
			else
			{
				m_bulletsInMagazine += m_currentAmmo;
				m_currentAmmo = 0;
			}

		}

	}
}
