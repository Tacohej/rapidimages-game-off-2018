using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="ScriptableObjects/AI/FishStats")]
public class FishStats : ScriptableObject
{
	[Header("Swim speeds")]
	public float swimSpeed = 0.5f;
	public float fleeSpeed = 10;
	public float chaseSpeed = 5;
	[Space]
	public float swimAreaRadius = 3;

	public float sightRadius = 2;

	[Header("Decides how long the fish will be in idle state before swimming")]
	public float minIdleDuration = 1;
	public float maxIdleDuration = 3;

	[Header("Debugging")]
	public bool showSwimArea = true;
	public bool showSightArea = true;
}
