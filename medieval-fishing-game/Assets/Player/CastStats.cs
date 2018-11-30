using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="MedievalFishing/Player/CastMeter")]
public class CastStats : ScriptableObject
{
	[System.NonSerialized]
	public float currentVelocity = 0;
	[System.NonSerialized]
	public float currentAccuracy = 0;
	[System.NonSerialized]
	public float currentGravity = 0;
	[System.NonSerialized]
	public Vector3[] positions;
}
