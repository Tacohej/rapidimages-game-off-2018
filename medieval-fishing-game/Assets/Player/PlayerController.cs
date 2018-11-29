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
		Casting,
		Battle
	}

	public enum FadeDir {
		In,
		Out
	}

	public Animator animator;

	public TextScroller textScroller;

	public CastStats castStats;
	public RewardSystem rewardSystem;
	public BattleSystem battleSystem;

	public Bait equppedBait;
	public GameObject castPreview;
	public GameObject tip;
	public RectTransform battleMenu;

	public float waterLevelY = 0;

	[SerializeField]
	private State currentState;
	private int currentPositionIndex;
	private Vector3 prevPosition;

	private List<HingeJoint> fishLineJoints = new List<HingeJoint>();
	private LineRenderer fishLine;
	private float catchDistance = 1;

	public Material baitCameraMaterial;
	private float fadeInValue;
	private FadeDir fadeDirection;

	private bool tutorialMode = true;

	void Start () {
		currentState = State.Setup;
		fishLine = GetComponent<LineRenderer>();
		fishLine.positionCount = 2;
		fadeInValue = 0;
		rewardSystem.Reset();



		textScroller.AddScrollText("...");
		textScroller.AddScrollText("!");
		textScroller.AddScrollText("Well hello there!");
		textScroller.AddScrollText("Press SPACE when ready.");

	}

	public float EaseOutBack(float start, float end, float value)
	{
			float s = 1.70158f;
			end -= start;
			value = (value) - 1;
			return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
	}

	void Fade (FadeDir dir, float speed = 3) {
		var dirValue = dir == FadeDir.In ? Time.deltaTime : -Time.deltaTime;
		fadeInValue = Mathf.Clamp01(fadeInValue + dirValue * speed);
		baitCameraMaterial.SetFloat("_FadeValue", fadeInValue);
	}

	Vector3 JointsToPositions (HingeJoint joint) {
		return joint.transform.position;
	}

	public void FinishCasting () {
		currentState = State.Release;
	}

	public void UpdateBaitPosition(){
		equppedBait.transform.position = tip.transform.position;
	}
	
	void Update ()
	{

		switch(currentState) {

			case State.Setup:
			{
				fadeDirection = FadeDir.Out;

				if (Input.GetKeyDown(KeyCode.Space))
				{
					currentState = State.Aim;
					textScroller.AddScrollText("Look around with A and D...", true, true);
					textScroller.AddScrollText("or use left and right arrow.");
					textScroller.AddScrollText("Press Space again to cast.");
				}

				UpdateBaitPosition();
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

				var horizontalInput = Input.GetAxis("Horizontal");

				if (this.transform.rotation.y > -25 && this.transform.rotation.y < 25) // todo: fix
				{
						this.transform.Rotate(this.transform.up, horizontalInput * Time.deltaTime * 40);
				}

				if (Input.GetKeyDown(KeyCode.Space))
				{
					if (tutorialMode) {
						textScroller.AddScrollText("Let's do this!", true, true);
						textScroller.AddScrollText("Wieeeeee!");
						textScroller.AddScrollText("");
					}

					currentState = State.Casting;
					currentPositionIndex = 0;
					prevPosition = this.transform.position;
					animator.SetTrigger("BeginCast");
				}
				UpdateBaitPosition();
				break;
			}
			case State.Casting: 
			{
				UpdateBaitPosition();
				break;
			}
			case State.Release:
			{
				fadeDirection = FadeDir.In;
				castPreview.GetComponent<LineRenderer>().enabled = false;
				var currentPosition = castStats.positions[currentPositionIndex] + this.transform.position;

				currentPosition = Quaternion.Euler(0, this.transform.rotation.y * Mathf.Rad2Deg, 0) * currentPosition;
				if (Vector3.Distance(equppedBait.transform.position, currentPosition) < 0.1f) {
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
				if (tutorialMode) {
					textScroller.AddScrollText("Wow...", true, true);
					textScroller.AddScrollText("Now use space to reel in.");
				}
				currentState = State.Reel;
				break;
			}
			case State.Battle:
			{

				if (!battleSystem.IsPlayerActionOnCooldown()) {
					if (Input.GetKeyDown(KeyCode.A)) {
						battleSystem.DoAction(BattleSystem.PlayerAction.Slack);
					}

					if (Input.GetKeyDown(KeyCode.S)){
						battleSystem.DoAction(BattleSystem.PlayerAction.Pull);
					}

					if (Input.GetKey(KeyCode.D)) {
						battleSystem.DoAction(BattleSystem.PlayerAction.Reel);
					}
				}

				var battleState = battleSystem.UpdateBattle(Time.time);

				if (battleState == BattleSystem.BattleState.GotFish) {
					var fish = equppedBait.GetComponent<Bait>().GetHookedFish();
					fish.GetComponent<FishAI>().defeted = true;
					currentState = State.Reel;
				}

				if (battleState == BattleSystem.BattleState.LostFish) {
					var fish = equppedBait.GetComponent<Bait>().GetHookedFish();
					fish.GetComponent<FishAI>().SwimBack();
					currentState = State.Reel;
				}

				if (battleState == BattleSystem.BattleState.LostBait) {
					// todo: implement
				}

				break;
			}
			case State.Reel:
			{
				// Reel in
				if (equppedBait.transform.position.y > waterLevelY)
				{
						equppedBait.transform.position = Vector3.MoveTowards(equppedBait.transform.position, new Vector3(0, -1000), 10 * Time.deltaTime);
				}

				var fish = equppedBait.GetComponent<Bait>().GetHookedFish();

				if (fish && !fish.GetComponent<FishAI>().defeted) {
					// todo enable ui
					battleSystem.StartBattle(fish.GetComponent<FishAI>().fishStats, equppedBait.baitStats);
					currentState = State.Battle;
				}

				if (Input.GetKey(KeyCode.Space)){
					equppedBait.transform.position = Vector3.MoveTowards(equppedBait.transform.position, tip.transform.position , 50 * Time.deltaTime);
				} else {
					// Quick and dirty sink mechanic
					// todo: handle when over water surface
					equppedBait.transform.position = Vector3.MoveTowards(equppedBait.transform.position, new Vector3(0,-1000,0), 3 * Time.deltaTime);
				}

				// Catch fish
				if (Vector3.Distance(tip.transform.position, equppedBait.transform.position) < catchDistance) {
					var fishObject = equppedBait.GetHookedFish();
					if (fishObject) {
						var fishStats = fishObject.GetComponent<FishAI>().fishStats;
						var currentXP = fishStats.XP;
						rewardSystem.FishCaught(currentXP);
						Destroy(fishObject);
					}
					animator.SetTrigger("Reset");
					equppedBait.Reset();
					this.currentState = State.Aim;
				}
				break;
			}
		}

			Fade(fadeDirection);

			if (currentState == State.Battle) {
				battleMenu.localPosition = Vector3.Lerp(battleMenu.localPosition, new Vector3(0,0,0), Time.deltaTime * 5);
			} else {
				battleMenu.localPosition = Vector3.Lerp(battleMenu.localPosition, new Vector3(0,500,0), Time.deltaTime * 5);
			}

			var positions = new Vector3[] {tip.transform.position, equppedBait.transform.position};
			fishLine.SetPositions(positions);
	}

}
