using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="MedievalFishing/Player/CastMeter")]
public class CastStats : ScriptableObject
{
	public float currentVelocity;
	public float currentAccuracy;
	public float currentGravity;

	public Vector3[] positions;
}
