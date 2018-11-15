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
		Chase
	}

	private State currentState;
	private Vector3 originalPosition;
	private Vector3 targetPosition;
	private Rigidbody rbody;
	private GameObject targetBait;
	private float duration;
	
	public LayerMask layerMask;
	public FishStats fishStats;

	void Start ()
	{
		rbody = GetComponent<Rigidbody>();
		originalPosition = this.transform.position;
		duration = GetRandomDuration();
	}
	
	void FixedUpdate ()
	{
		switch(currentState)
		{
			case State.Idle: 
			{
				duration -= Time.deltaTime;
				if (duration < 0)
				{
					targetBait = GetNearbyBait();
					if (targetBait)
					{
						ChangeState(State.Chase);
					} else
					{
						var randomInsideEllipsoid = Vector3.Scale(Random.insideUnitSphere, new Vector3(1, 0.1f, 1));
						targetPosition = (originalPosition + randomInsideEllipsoid) * fishStats.swimAreaRadius;
						ChangeState(State.Swim);
					}
				}
				break;
			}
			case State.Swim:
			{
				// reset when close to target
				if (Vector3.SqrMagnitude(this.transform.position - targetPosition) < 0.1){
					ChangeState(State.Idle);
				} else
				{
					// always turn to target to prevent getting of track
					transform.LookAt(targetPosition);
					rbody.AddRelativeForce(Vector3.forward * fishStats.swimSpeed);
				}
				break;
			}
			case State.Chase:
			{
				Debug.Log("did chase");
				var dist = Vector3.Distance(this.transform.position, targetBait.transform.position);
				print(dist);
				if (dist > fishStats.stopChasingAtDistance) 
				{
					targetBait = null;
					ChangeState(State.Idle);
				} else
				{
					transform.LookAt(targetBait.transform.position);
					rbody.AddRelativeForce(Vector3.forward * fishStats.chaseSpeed);
				}
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

	void ChangeState (State state) {

		if (state == State.Idle) {
			duration = GetRandomDuration();
		}

		currentState = state;
	}

	GameObject GetNearbyBait()
	{
		Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, fishStats.sightRadius, layerMask);

		foreach(Collider col in hitColliders)
		{
			var bait = col.gameObject.GetComponent<Bait>();
			if (bait && fishStats.attactToList.Contains(bait.type) )
			{
				print("test");
				return bait.gameObject;
			}
		}

		return null;
	}

	float GetRandomDuration ()
	{
		return Random.Range(fishStats.minIdleDuration, fishStats.maxIdleDuration);
	}

}
