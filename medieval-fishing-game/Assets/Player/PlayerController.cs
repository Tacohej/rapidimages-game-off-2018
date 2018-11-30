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

	public List<Bait> _baits = new List<Bait>();
	public List<Rod> _rods = new List<Rod>();

	public Button[] buttons = new Button[3];
	public GameObject grip;
	public GameObject baitHolder;
	
	public Animator animator;
	public GameObject castPreview;

	public CastStats castStats;
	public Inventory inventory;
	public RewardSystem rewardSystem;
	public BattleSystem battleSystem;
	public Material baitCameraMaterial;
	public Camera baitCamera;

	public Bait equippedBait;
	public Rod equippedRod;
	
	public TextScroller textScroller;
	public RectTransform battleMenu;
	public RectTransform battleButtons;
	public RectTransform leftInventoryPanel;
	public RectTransform rightInventoryPanel;

	[SerializeField]
	private State currentState;
	private LineRenderer fishLine;
	private Vector3 prevPosition;
	private Button lastButton;
	private FadeDir fadeDirection;
	private float waterLevelY = 0;
	private float catchDistance = 1;
	private float fadeInValue;
	private int currentPositionIndex;
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

		equippedRod.rodItem.found = true;
		equippedBait.baitItem.found = true;

		inventory.SelectedRod = equippedRod.rodItem;
		inventory.SelectedBait = equippedBait.baitItem;

		textScroller.AddScrollText("...", true, true);
		textScroller.AddScrollText("!");
		textScroller.AddScrollText("Well hello there!");
		textScroller.AddScrollText("Press SPACE when ready.");
	}

	void OnEnable () {
		inventory.onSelectedRodChanged += OnSelectedRodChanged;
		inventory.onSelectedBaitChanged += OnSelectedBaitChanged;
	}

	void EquipRod (RodItem rodItem) {
		// var rods = grip.GetComponentsInChildren<Rod>();
		foreach (Rod rod in _rods) {
			var active = rodItem == rod.rodItem;
			rod.gameObject.SetActive(active);
			if (active) {
				equippedRod = rod;
			}
		}
	}

	void EquipBait (BaitItem baitItem) {
		// var baits = baitHolder.GetComponentsInChildren<Bait>();
		foreach (Bait bait in _baits) {
			var active = baitItem == bait.baitItem;
			bait.gameObject.SetActive(active);
			if (active) {
				equippedBait = bait;
			}
		}
	}

	void OnSelectedRodChanged (RodItem item) {
		textScroller.AddScrollText("TODO: Add info here" + item.title, true, true);
		EquipRod(item);
	}

	void OnSelectedBaitChanged (BaitItem item) {
		textScroller.AddScrollText("TODO: Add info here", true, true);
		EquipBait(item);
	}

	public void FinishCasting() {
		currentState = State.Release;
	}

	public void UpdateBaitPosition(){
		equippedBait.transform.position = equippedRod.tip.transform.position;
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

	void Fade (FadeDir dir, float speed = 3) {
		var dirValue = dir == FadeDir.In ? Time.deltaTime : -Time.deltaTime;
		fadeInValue = Mathf.Clamp01(fadeInValue + dirValue * speed);
		baitCameraMaterial.SetFloat("_FadeValue", fadeInValue);
	}

	Vector3 JointsToPositions (HingeJoint joint) {
		return joint.transform.position;
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
				if (Vector3.Distance(equippedBait.transform.position, currentPosition) < 0.1f) {
					if (currentPositionIndex >= castStats.positions.Length -1){
						soundManager.PlayClipFile(SoundData.WATER_SPLASH);
						Vector3 particlePos = new Vector3(equippedBait.transform.position.x, 0, equippedBait.transform.position.z);
						GameObject particle = Instantiate(waterSplash, particlePos, Quaternion.AngleAxis(-90, Vector3.right));
						particle.GetComponent<ParticleSystem>().Play();
						currentState = State.Splash;
						Destroy(particle, 0.5f);
					} else {
						prevPosition = currentPosition;
						currentPositionIndex++;
					}
				}
				equippedBait.transform.position = Vector3.MoveTowards(equippedBait.transform.position, currentPosition, Vector3.Distance(prevPosition, currentPosition) * 100 * Time.deltaTime);
				break;
			}
			case State.Splash:
			{
				equippedBait.GetComponent<SphereCollider>().enabled = true;
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
					var fish = equippedBait.GetComponent<Bait>().GetHookedFish();
					fish.GetComponent<FishAI>().defeted = true;
					currentState = State.Reel;
				}

				if (battleState == BattleSystem.BattleState.LostFish || battleState == BattleSystem.BattleState.LostBait) { // todo: both cases here
					var fish = equippedBait.GetComponent<Bait>().GetHookedFish();
					fish.GetComponent<FishAI>().SwimBack();
					equippedBait.GetComponent<Bait>().ReleaseHookedFish();
					currentState = State.Reel;
				}

				break;
			}
			case State.Reel:
			{
				var fish = equippedBait.GetComponent<Bait>().GetHookedFish();

				if (fish && !fish.GetComponent<FishAI>().defeted) {
					battleSystem.StartBattle(fish.GetComponent<FishAI>().fishStats, equippedRod.rodItem, textScroller, tutorialMode);
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

				if (Input.GetKey(KeyCode.Space) || equippedBait.GetComponent<Bait>().IsTaken()){
					if (equippedBait.transform.position.y > waterLevelY && Mathf.Abs(equippedBait.transform.position.x - equippedRod.tip.transform.position.x) > 5f) {
						var position = new Vector3(equippedRod.tip.transform.position.x, equippedBait.transform.position.y, equippedRod.tip.transform.position.z);
						equippedBait.transform.position = Vector3.MoveTowards(equippedBait.transform.position, position , 50 * Time.deltaTime);
					} else {
						equippedBait.transform.position = Vector3.MoveTowards(equippedBait.transform.position, equippedRod.tip.transform.position , 50 * Time.deltaTime);
					}
				} else {
					equippedBait.transform.position = Vector3.MoveTowards(equippedBait.transform.position, new Vector3(0,-1000,0), 3 * Time.deltaTime);
				}

				// Catch fish
				if (Vector3.Distance(equippedRod.tip.transform.position, equippedBait.transform.position) < catchDistance) {
					var fishObject = equippedBait.GetHookedFish();
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
					equippedBait.Reset();
				}
				break;
			}
		}
			// TODO
			// Update Camera
			// var baitCameraPos = baitCamera.transform.position;
			// baitCameraPos.x = equippedBait.transform.position.x;
			// baitCameraPos.z = equippedBait.transform.position.z;
			// baitCamera.transform.position = baitCameraPos;
			baitCamera.transform.parent = equippedBait.transform;

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

			var positions = new Vector3[] {equippedRod.tip.transform.position, equippedBait.transform.position};
			fishLine.SetPositions(positions);
	}

}
