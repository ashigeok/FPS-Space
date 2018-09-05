using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	[SerializeField] private float m_maxHealthPoint;
	
	private float m_currentHealthPoint;
	
	// Use this for initialization
	void Start ()
	{
		m_currentHealthPoint = m_maxHealthPoint;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void HealFullHealth()
	{
		m_currentHealthPoint = m_maxHealthPoint;
	}

	public void TakeDamage(float damage)
	{
		m_currentHealthPoint -= damage;
	}

	public bool IsDead()
	{
		return m_currentHealthPoint <= 0;
	}
}
