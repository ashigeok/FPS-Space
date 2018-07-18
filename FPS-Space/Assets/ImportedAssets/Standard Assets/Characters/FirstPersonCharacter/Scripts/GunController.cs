using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Characters.FirstPerson
{
	public class GunController : MonoBehaviour
	{
		[SerializeField] private GameObject muzzle;
		[SerializeField] private GameObject sparkle;
		[SerializeField] private AudioClip audioClip;
		private Vector3 hitPosition;
		private bool isHit;
		private bool m_IsCoolTime = false;
		private const float coolTime = 0.5f;
		private const float offset = 0.1f;
		private GameObject muzzleFlash;
		private GameObject hitSparkle;
		private AudioSource audioSource;
		
		
		// Use this for initialization
		private void Start()
		{
			audioSource = GetComponent<AudioSource>();
		}

		private Vector3 GetFiringDirection()
		{
			// ターゲットオブジェクトとの差分を求め
			Vector3 temp =  FirstPersonController.m_HitPosition - muzzle.transform.position;
			// 正規化して方向ベクトルを求める
			Vector3 normal = temp.normalized;

			return normal;
		}

		// Update is called once per frame
		private void Update () {

			RaycastHit hit;

				if (Physics.Raycast (muzzle.transform.position, GetFiringDirection(), out hit))
				{
					isHit = true;
					hitPosition = hit.point;
				}
				else
				{
					isHit = false;
				}
			
			if (Input.GetMouseButton(0) && !m_IsCoolTime)
			{
				Fire();
			}
			
		}


		private void Fire()
		{
			m_IsCoolTime = true;
			FireSound();
			FireEffect();
			Invoke("SetCoolTime", coolTime);
		}
		
		private void FireSound()
		{
			audioSource.PlayOneShot(audioClip);
		}

		private void FireEffect ()
		{
			muzzleFlash = Instantiate(sparkle, muzzle.transform.position, muzzle.transform.rotation, muzzle.transform) as GameObject;
			Invoke("DestroyMuzzleFlash", 0.1f);
			
			if (isHit)
			{
				HitSparkleOffset();
				hitSparkle = Instantiate(sparkle, hitPosition, muzzle.transform.rotation) as GameObject;
				Invoke("DestroyHitSparkle", 0.1f);
			}
		}

		private void SetCoolTime()
		{
			m_IsCoolTime = false;
		}

		private void DestroyMuzzleFlash()
		{
			Destroy(muzzleFlash);
		}
		
		private void DestroyHitSparkle()
		{
			Destroy(hitSparkle);
		}

		private void HitSparkleOffset()
		{
			if (hitPosition.x < muzzle.transform.position.x)
			{
				hitPosition.x += offset;
			}
			else
			{
				hitPosition.x -= offset;
			}
			if (hitPosition.y < muzzle.transform.position.y)
			{
				hitPosition.y += offset;
			}
			else
			{
				hitPosition.y -= offset;
			}
			if (hitPosition.z < muzzle.transform.position.z)
			{
				hitPosition.z += offset;
			}
			else
			{
				hitPosition.z -= offset;
			}
		}
		

	}
}
