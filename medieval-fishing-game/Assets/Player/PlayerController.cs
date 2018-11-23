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
	public RewardSystem rewardSystem;

	public Bait equppedBait;
	public GameObject castPreview;

	[SerializeField]
	private State currentState;
	private int currentPositionIndex;
	private Vector3 prevPosition;

	private List<HingeJoint> fishLineJoints = new List<HingeJoint>();
	private LineRenderer fishLine;
	private float catchDistance = 1;

	private Shader baitCameraShader;


	void Start () {
		currentState = State.Setup;
		fishLine = GetComponent<LineRenderer>();
		fishLine.positionCount = 2;
		baitCameraShader = Shader.Find("BaitCameraShader");
		rewardSystem.Reset();
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
				castPreview.GetComponent<LineRenderer>().enabled = true;
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
					if (currentPositionIndex >= castStats.positions.Length -1){
						currentState = State.Splash;
					} else {
						prevPosition = currentPosition;
						currentPositionIndex++;
					}
				}

				equppedBait.transform.position = Vector3.MoveTowards(equppedBait.transform.position, currentPosition, Vector3.Distance(prevPosition, currentPosition) * 100 * Time.deltaTime);
				break;
			}
			case State.Splash:
			{
				equppedBait.GetComponent<SphereCollider>().enabled = true;
				// todo: do some damage or whatever
				currentState = State.Reel;
				break;
			}
			case State.Reel:
			{
				// Reel in
				if (Input.GetKey(KeyCode.Space)){
					equppedBait.transform.position = Vector3.MoveTowards(equppedBait.transform.position, this.transform.position, 50 * Time.deltaTime);
				} else {
					// Quick and dirty sink mechanic
					// todo: handle when over water surface
					equppedBait.transform.position = Vector3.MoveTowards(equppedBait.transform.position, new Vector3(0,-1000,0), 3 * Time.deltaTime);
				}

				// Catch fish
				if (Vector3.Distance(this.transform.position, equppedBait.transform.position) < catchDistance) {
					var fishObject = equppedBait.GetHookedFish();
					if (fishObject) {
						var fishStats = fishObject.GetComponent<FishAI>().fishStats;
						var currentXP = fishStats.XP;
						rewardSystem.FishCaught(currentXP);
						Destroy(fishObject);
					}
					equppedBait.Reset();
					this.currentState = State.Setup;
				}
				break;
			}
		}

			var positions = new Vector3[] {this.transform.position, equppedBait.transform.position};
			fishLine.SetPositions(positions);
	}
}
