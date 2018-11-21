using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="MedievalFishing/AI/FishStats")]
public class FishStats : ScriptableObject
{
	[Space]
	public string fishName;

	[Header("Swim speeds")]
	public float swimSpeed = 0.5f;
	public float chaseSpeed = 5;
	[Space]
	public float swimAreaRadius = 3;

	[Header("Sight")]
	public float sightRadius = 2;
	public float stopChasingAtDistance = 10;

	[Header("Decides how long the fish will be in idle state before swimming")]
	public float minIdleDuration = 1;
	public float maxIdleDuration = 3;

	[Header("Set bait types")]
	public List<Bait.Type> attactToList = new List<Bait.Type>();

	[Header("Rewards")]
	public int XP;

	[Space]
	[Header("Debugging")]
	public bool showSwimArea = true;
	public bool showSightArea = true;
}
