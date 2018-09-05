using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static int CalcScore(Vector3 p1, Vector3 p2)
	{
		float distance = Vector3.Distance(p1, p2);

		if (distance <0.1)
		{
			return 40;
		}
		else if (distance <0.2)
		{
			return 20;
		}
		else if (distance <0.35)
		{
			return 10;
		}
		else
		{
			return 5;
		}
	}
}
