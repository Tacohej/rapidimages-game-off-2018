using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FishAI : MonoBehaviour
{
	enum State
	{
		Idle,
		Swim,
		Flee,
		Chase
	}

	private State currentState;
	private Rigidbody rbody;
	private Vector3 originalPosition;
	private Vector3 targetPosition;
	private float duration;

	public FishStats fishStats;

	void Start ()
	{
		rbody = GetComponent<Rigidbody>();
		originalPosition = this.transform.position;
		duration = GetRandomDuration();
	}
	
	void Update ()
	{
		switch(currentState)
		{
			case State.Idle: 
			{
				duration -= Time.deltaTime;
				if (duration < 0)
				{
					var randomInsideEllipsoid = Vector3.Scale(Random.insideUnitSphere, new Vector3(1, 0.1f, 1));
					targetPosition = (originalPosition + randomInsideEllipsoid) * fishStats.swimAreaRadius;
					currentState = State.Swim;
				}
				break;
			}
			case State.Swim:
			{
				// reset when close to target
				if (Vector3.SqrMagnitude(this.transform.position - targetPosition) < 0.1){
					currentState = State.Idle;
					duration = GetRandomDuration();
				}
				else
				{
					// todo: fixed update and deltatime
					transform.LookAt(targetPosition);
					rbody.AddRelativeForce(Vector3.forward * fishStats.swimSpeed);
				}
				break;
			}
			case State.Chase:
			{
				
				break;
			}
		}
	}

	void OnDrawGizmos()
	{
		if (fishStats.showSwimArea)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(originalPosition, fishStats.swimAreaRadius);
		}

		if (fishStats.showSightArea)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(this.transform.position, fishStats.sightRadius);
		}
	}

	void GetNearColliders()
	{
		// todo: fix
		Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, fishStats.sightRadius);
	}

	float GetRandomDuration ()
	{
		return Random.Range(fishStats.minIdleDuration, fishStats.maxIdleDuration);
	}

}
