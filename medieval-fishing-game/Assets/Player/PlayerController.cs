using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	public enum State
	{
		Intro,
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

	public Button[] buttons = new Button[3];

	public Animator animator;

	public TextScroller textScroller;

	public CastStats castStats;
	public RewardSystem rewardSystem;
	public BattleSystem battleSystem;

	public Bait equppedBait;
	public GameObject castPreview;
	public GameObject tip;
	public RectTransform battleMenu;
	public RectTransform battleButtons;
	public RectTransform leftInventoryPanel;
	public RectTransform rightInventoryPanel;

	public float waterLevelY = 0;

	[SerializeField]
	private State currentState;
	private int currentPositionIndex;
	private Vector3 prevPosition;

	private List<HingeJoint> fishLineJoints = new List<HingeJoint>();
	private LineRenderer fishLine;
	private float catchDistance = 1;
	private Button lastButton;

	public Material baitCameraMaterial;
	private float fadeInValue;
	private FadeDir fadeDirection;

	private bool tutorialMode = true;

	private SoundManager soundManager;
	
	void Awake() {
		soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
	}

	GameObject waterSplash;

	void Start () {
		waterSplash = Resources.Load<GameObject>("Particles/WaterSplash");
		soundManager.PlayMusicFile(SoundData.FISHING_MUSIC);
		currentState = State.Intro;
		fishLine = GetComponent<LineRenderer>();
		fishLine.positionCount = 2;
		fadeInValue = 0;
		rewardSystem.Reset();
		fadeDirection = FadeDir.Out;

		textScroller.AddScrollText("...");
		textScroller.AddScrollText("!");
		textScroller.AddScrollText("Well hello there!");
		textScroller.AddScrollText("Press SPACE when ready.");
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

	public void DoSlackAction () {
		if (currentState == State.Battle) {
			battleSystem.DoAction(BattleSystem.PlayerAction.Slack);
			soundManager.PlayClipFile("Sound/MenuSounds/MenuSelect");
			lastButton = buttons[0];
		} 
	}
	public void DoPullAction () {
		if (currentState == State.Battle)
		{
			battleSystem.DoAction(BattleSystem.PlayerAction.Pull);
			soundManager.PlayClipFile("Sound/MenuSounds/MenuSelect");
			lastButton = buttons[1];
		} 
	}
	public void DoReelAction () {
		if (currentState == State.Battle) 
		{
			battleSystem.DoAction(BattleSystem.PlayerAction.Reel);
			soundManager.PlayClipFile("Sound/MenuSounds/MenuSelect");
			lastButton = buttons[2];
		}
	}
	
	void Update ()
	{

		switch(currentState) {

			case State.Intro:
			{

				if (Input.GetKeyDown(KeyCode.Space))
				{
					currentState = State.Aim;
					textScroller.AddScrollText("Look around with A and D", true, true);
					textScroller.AddScrollText("... or use left and right arrow.");
					textScroller.AddScrollText("Press SPACE again to cast.");
				}

				UpdateBaitPosition();
				break;
			}
			case State.Setup:
			{			
				fadeDirection = FadeDir.Out;
				if (tutorialMode) {
					textScroller.AddScrollText("Now you may equip your new bait.");
					textScroller.AddScrollText("How go get some more fish!");
					textScroller.AddScrollText("Press SPACE when Ready.");
					tutorialMode = false;
				}

				if (Input.GetKeyDown(KeyCode.Space)) {
					currentState = State.Aim;
				}

				UpdateBaitPosition();
				break;
			}

			case State.Aim:
			{
				if (soundManager.currentMusic != SoundData.FISHING_MUSIC){
					soundManager.PlayMusicFile(SoundData.FISHING_MUSIC);
				}
				fadeDirection = FadeDir.Out;
				castPreview.GetComponent<LineRenderer>().enabled = true;

				// todo: Could use rod stats here
				var speed = 1f;
				var ntime = Time.time * speed % Mathf.PI;
				var velocity = 120 - Mathf.Abs(Mathf.Sin(ntime) + Mathf.PI / 2) * 35;
				
				castStats.currentVelocity = velocity;
				castStats.currentGravity = -15 + Mathf.Abs(Mathf.Sin(ntime) + Mathf.PI / 2) * 5;
				castStats.currentAccuracy = Mathf.Sin(Time.time * 3);

				var horizontalInput = Input.GetAxis("Horizontal");


				if ((this.transform.rotation.y > -0.35f && horizontalInput < 0) || (this.transform.rotation.y < 0.35f && horizontalInput > 0))
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
						soundManager.PlayClipFile(SoundData.WATER_SPLASH);
						// SÄTT UT SKITEN HÄR
						Vector3 particlePos = new Vector3(equppedBait.transform.position.x, 0, equppedBait.transform.position.z);
						GameObject particle = Instantiate(waterSplash, particlePos, Quaternion.AngleAxis(-90, Vector3.right));
						particle.GetComponent<ParticleSystem>().Play();
						currentState = State.Splash;
						Destroy(particle, 0.5f);
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
					textScroller.AddScrollText("Now use SPACE to reel in.");
				}
				currentState = State.Reel;
				break;
			}
			case State.Battle:
			{

				if (soundManager.currentMusic != SoundData.BATTLE_MUSIC){
					soundManager.PlayMusicFile(SoundData.BATTLE_MUSIC);
				}

				if (!battleSystem.IsPlayerActionOnCooldown()) {
					foreach (Button button in buttons) {
						if (button != button.interactable) {
							button.interactable = true;
							if (button == lastButton) {
								button.Select();
							}
						}
					}
				} else {
					foreach (Button button in buttons) {
						button.interactable = false;
					}
				}

				var battleState = battleSystem.UpdateBattle(Time.deltaTime);

				if (battleState == BattleSystem.BattleState.GotFish) {
					var fish = equppedBait.GetComponent<Bait>().GetHookedFish();
					fish.GetComponent<FishAI>().defeted = true;
					currentState = State.Reel;
				}

				if (battleState == BattleSystem.BattleState.LostFish || battleState == BattleSystem.BattleState.LostBait) { // todo: both cases here
					var fish = equppedBait.GetComponent<Bait>().GetHookedFish();
					fish.GetComponent<FishAI>().SwimBack();
					equppedBait.GetComponent<Bait>().ReleaseHookedFish();
					currentState = State.Reel;
				}

				break;
			}
			case State.Reel:
			{
				var fish = equppedBait.GetComponent<Bait>().GetHookedFish();

				if (fish && !fish.GetComponent<FishAI>().defeted) {
					battleSystem.StartBattle(fish.GetComponent<FishAI>().fishStats, equppedBait.baitStats, textScroller, tutorialMode);
					currentState = State.Battle;
					buttons[0].Select();
					if (tutorialMode) {
						textScroller.AddScrollText("You have one hooked!", true, true);
						textScroller.AddScrollText("Use ACTIONS to tire it out.");
						textScroller.AddScrollText("Some work better than others.");
						textScroller.AddScrollText("The fish is struggling!");
						textScroller.AddScrollText("Try Slack");
					}
				}

				if (Input.GetKey(KeyCode.Space) || equppedBait.GetComponent<Bait>().IsTaken()){
					if (equppedBait.transform.position.y > waterLevelY && Mathf.Abs(equppedBait.transform.position.x - tip.transform.position.x) > 5f) {
					//	fish.transform
						var position = new Vector3(tip.transform.position.x, equppedBait.transform.position.y, tip.transform.position.z);
						equppedBait.transform.position = Vector3.MoveTowards(equppedBait.transform.position, position , 50 * Time.deltaTime);
					} else {
						equppedBait.transform.position = Vector3.MoveTowards(equppedBait.transform.position, tip.transform.position , 50 * Time.deltaTime);
					}
				} else {
					equppedBait.transform.position = Vector3.MoveTowards(equppedBait.transform.position, new Vector3(0,-1000,0), 3 * Time.deltaTime);
				}

				// Catch fish
				if (Vector3.Distance(tip.transform.position, equppedBait.transform.position) < catchDistance) {
					var fishObject = equppedBait.GetHookedFish();
					this.currentState = State.Setup;
					if (fishObject) {
						var fishStats = fishObject.GetComponent<FishAI>().fishStats;
						var currentXP = fishStats.XP;
						rewardSystem.FishCaught(currentXP, textScroller);
						Destroy(fishObject);
					} else if (tutorialMode) {
						textScroller.AddScrollText("You will get one next Time");
						this.currentState = State.Aim;
					}
					animator.SetTrigger("Reset");
					equppedBait.Reset();
				}
				break;
			}
		}

			// dual camera effect
			Fade(fadeDirection);

			// Handle UI updates
			if (currentState == State.Battle) {
				battleMenu.localPosition = Vector3.Lerp(battleMenu.localPosition, new Vector3(0,0,0), Time.deltaTime * 5);
				battleButtons.localPosition = Vector3.Lerp(battleButtons.localPosition, new Vector3(0,0,0), Time.deltaTime * 7);
			} else {
				battleButtons.localPosition = Vector3.Lerp(battleButtons.localPosition, new Vector3(0,-100,0), Time.deltaTime * 7);
				battleMenu.localPosition = Vector3.Lerp(battleMenu.localPosition, new Vector3(0,500,0), Time.deltaTime * 5);
			}

			if (currentState == State.Setup) {
				leftInventoryPanel.localPosition = Vector3.Lerp(leftInventoryPanel.localPosition, new Vector3(0,0,0), Time.deltaTime * 5);
				rightInventoryPanel.localPosition = Vector3.Lerp(rightInventoryPanel.localPosition, new Vector3(0,0,0), Time.deltaTime * 5);
			} else {
				leftInventoryPanel.localPosition = Vector3.Lerp(leftInventoryPanel.localPosition, new Vector3(-300,0,0), Time.deltaTime * 5);
				rightInventoryPanel.localPosition = Vector3.Lerp(rightInventoryPanel.localPosition, new Vector3(300,0,0), Time.deltaTime * 5);
			}

			var positions = new Vector3[] {tip.transform.position, equppedBait.transform.position};
			fishLine.SetPositions(positions);
	}

}
