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
		Chase,
		Hooked
	}
	
	public LayerMask layerMask;
	public FishStats fishStats;
	public float waterLevelY = 0;

	[Header("Read only variables")]
	[SerializeField]
	private State currentState;
	[SerializeField]
	private GameObject targetBait;
	[SerializeField]

	private float duration;
	private Vector3 originalPosition;
	private Vector3 targetPosition;
	private Rigidbody rbody;

	public bool defeted = false;
	

	void Start ()
	{
		rbody = GetComponent<Rigidbody>();
		originalPosition = this.transform.position;
		duration = GetRandomDuration();
	}
	
	public void SwimBack () {
		this.transform.parent = null;
		currentState = State.Swim;
		targetPosition = originalPosition + new Vector3(10, 0, 0);
		rbody.isKinematic = false;
	}

	void FixedUpdate ()
	{

		if (transform.position.y > waterLevelY && targetBait == null)
		{
				if (rbody.useGravity == false)
				{
						rbody.AddForce((Vector3.up + transform.rotation.eulerAngles).normalized * 2000); // todo: fix does not work
						rbody.useGravity = true;
						currentState = State.Idle;
						rbody.drag = 0;
				}
				transform.rotation = Quaternion.LookRotation(rbody.velocity);
				return;
		} else
		{
				rbody.drag = 1;
				rbody.useGravity = false;
		}

		switch(currentState)
		{
			case State.Idle: 
			{
				duration -= Time.fixedDeltaTime;
				if (duration < 0)
				{
					targetBait = GetNearbyBait();
					if (targetBait)
					{
						ChangeState(State.Chase);
					} else
					{
						var randomInsideEllipsoid = Vector3.Scale(Random.insideUnitSphere, new Vector3(1, 0.1f, 1));
						targetPosition = originalPosition + randomInsideEllipsoid * fishStats.swimAreaRadius;
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
				// todo: stop chasing if the bait is no longer in water
				var dist = Vector3.Distance(this.transform.position, targetBait.transform.position);
				if (dist > fishStats.sightRadius * 2 || targetBait.GetComponent<Bait>().IsTaken())
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
			case State.Hooked: 
			{
				// todo: struggle?
				break;
			}
		}
	}

	void OnCollisionEnter(Collision collision) {

		if (!collision.gameObject.GetComponent<Bait>()){
			return;
		}

		if (targetBait){
			targetBait.GetComponent<Bait>().HookFish(gameObject);
			rbody.isKinematic = true;
			ChangeState(State.Hooked);
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
			if (bait && fishStats.attactToList.Contains(bait.type) && !bait.IsTaken())
			{
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
