using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TargetController : MonoBehaviour
{

	[SerializeField] private float RespawnTime;

	private Animator m_animator;
	private Health m_health;
	private Collider[] m_colliders;

	// Use this for initialization
	void Start ()
	{
		m_animator = GetComponent<Animator>();
		m_health = GetComponent<Health>();
		m_colliders = GetComponentsInChildren<Collider>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (m_health.IsDead())
		{
			StartCoroutine (((Func<IEnumerator>)Respawn).Method.Name);
		}	
	}
	
	private IEnumerator Respawn()
	{
		m_animator.SetBool("IsDead", m_health.IsDead());
		SetCollidersEnabled(m_colliders, false);
		m_health.HealFullHealth();
		
		yield return new WaitForSeconds(RespawnTime);
		m_animator.SetBool("IsDead", m_health.IsDead());
		SetCollidersEnabled(m_colliders, true);
	}

	private void SetCollidersEnabled(Collider[] colliders, bool tf)
	{
		foreach(var col in colliders)
		{
			col.enabled = tf;
		}
	}
}
