using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="MedievalFishing/Player/CastMeter")]
public class CastStats : ScriptableObject
{
	[Header("Temporarily values")]
	public float startVelocity;
	public float startAccuracy;
	

	public float currentVelocity;
	public float currentAccuracy;
	public float currentGravity = 0;
	
	public void Reset() {
		currentVelocity = startVelocity;
		currentAccuracy = startAccuracy;
	}

}
