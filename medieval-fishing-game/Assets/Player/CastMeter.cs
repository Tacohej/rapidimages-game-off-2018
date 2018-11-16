using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="MedievalFishing/Player/CastMeter")]
public class CastMeter : ScriptableObject
{
	[Header("Temporarily values")]
	public float velocity;
	public float accuracy;
	
	public bool velocityIsSet;
	public bool accuracyIsSet;
	
	private float timer;

	public void StartMeter ()
	{
		timer = 0;
		velocityIsSet = false;
		accuracyIsSet = false;
	}

	public void UpdateMeter (float dt)
	{
		timer += dt;

		if (!velocityIsSet) {
			velocity = Mathf.Sin(timer);
		}
		accuracy = Mathf.Cos(timer);
	}

	void OnEnable () {
		Debug.Log("Test");
	}

}
