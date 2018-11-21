using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public enum State
	{
		Setup,
		Aim,
		Release,
		Splash,
		Reel,
		Loot,
	}

	public CastStats castStats;
	public Bait equppedBait;
	public HingeJoint fishLinePrefab;

	public GameObject castPreview;

	[SerializeField]
	private State currentState;
	private int currentPositionIndex;
	private Vector3 prevPosition;

	private List<HingeJoint> fishLineJoints = new List<HingeJoint>();
	private LineRenderer fishLine;


	void Start () {
		currentState = State.Setup;
		fishLine = GetComponent<LineRenderer>();
		fishLine.positionCount = 32;
	}

	Vector3 JointsToPositions (HingeJoint joint) {
		return joint.transform.position;
	}
	
	void Update ()
	{

		switch(currentState) {

			case State.Setup:
			{
				// todo: handle equipping bait & handle move character (in an arc)
				if (Input.GetKeyDown(KeyCode.Space))
				{
					currentState = State.Aim;
				}
				break;
			}

			case State.Aim:
			{
				// todo: calculations need some love
				var speed = 1f;
				var ntime = Time.time * speed % Mathf.PI;
				var velocity = 120 - Mathf.Abs(Mathf.Sin(ntime) + Mathf.PI / 2) * 35;
				
				castStats.currentVelocity = velocity;
				castStats.currentGravity = -15 + Mathf.Abs(Mathf.Sin(ntime) + Mathf.PI / 2) * 5;
				castStats.currentAccuracy = Mathf.Sin(Time.time * 3);

				if (Input.GetKeyDown(KeyCode.Space))
				{
					currentState = State.Release;
					currentPositionIndex = 0;
					prevPosition = this.transform.position;
				}
				break;
			}
			case State.Release:
			{
				castPreview.GetComponent<LineRenderer>().enabled = false;
				var currentPosition = castStats.positions[currentPositionIndex] + this.transform.position;
				if (Vector3.Distance(equppedBait.transform.position, currentPosition) < 1f) {
					if (currentPositionIndex >= castStats.positions.Length -1) {
						currentState = State.Splash;

						// todo:move to another script
						var hingeJoint = equppedBait.gameObject.AddComponent<HingeJoint>();
						hingeJoint.connectedBody = fishLineJoints[fishLineJoints.Count -1].connectedBody;
						fishLineJoints.Add(hingeJoint);

					} else {

						// todo:move to another script
						var instance = Instantiate(fishLinePrefab, currentPosition, this.transform.rotation);
						if (fishLineJoints.Count == 0) {
							instance.connectedAnchor = this.transform.position;
							fishLineJoints.Add(instance);
						} else {
							instance.connectedBody = fishLineJoints[fishLineJoints.Count -1].GetComponent<Rigidbody>();
							fishLineJoints.Add(instance);
						}

						prevPosition = currentPosition;
						currentPositionIndex++;
					}
				}

				equppedBait.transform.position = Vector3.MoveTowards(equppedBait.transform.position, currentPosition, Vector3.Distance(prevPosition, currentPosition) * 0.3f);
				break;
			}
			case State.Splash:
			{
				// todo: do some damage or whatever
				currentState = State.Reel;
				break;
			}
			case State.Reel:
			{
				// todo: fix this
				// var positions = fishLineJoints.ConvertAll<Vector3>(JointsToPositions).ToArray();
				// fishLine.SetPositions(positions);

				break;
			}
		}

			var positions = fishLineJoints.ConvertAll<Vector3>(JointsToPositions).ToArray();
			fishLine.positionCount = positions.Length;
			fishLine.SetPositions(positions);
	}
}
