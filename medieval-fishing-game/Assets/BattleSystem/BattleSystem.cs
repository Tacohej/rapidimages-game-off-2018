using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="MedievalFishing/Battle/BattleSystem")]
public class BattleSystem : ScriptableObject {

    public enum FishState
    {
        Attacking,
        Defending,
        Exhausted
    }

    public enum PlayerAction
    {
        Attack,
        Defend,
        Reel
    }

    public enum BattleState
    {
        StillInBattle,
        GotFish,
        LostFish,
        LostBait
    }

    [SerializeField]
    private FishStats fishStats;
    [SerializeField]
    private BaitStats baitStats;

    public int maxPlayerStamina = 1;
    public int maxFishStamina = 1;

    public int playerStamina;
    public int fishStamina;
    public FishState fishState;

    [SerializeField]
    private float noActionTime;
    [SerializeField]
    private float exhaustedTime;

	public void StartBattle (FishStats fishStats, BaitStats baitStats)
    {
        this.fishStats = fishStats;
        this.baitStats = baitStats;
        playerStamina = baitStats.stamina;
        fishStamina = fishStats.stamina;
        maxPlayerStamina = playerStamina;
        maxFishStamina = fishStamina;
        SetRandomFishState();
        noActionTime = 0;
	}

    public BattleState UpdateBattle (float deltaTime)
    {
        if (fishState == FishState.Exhausted) {
            exhaustedTime -= deltaTime;
            if (exhaustedTime < 0) {
                SetRandomFishState();
            }
        } else {
            noActionTime += deltaTime;
        }

        if (noActionTime > fishStats.fleeAfterMilliSec)
        {
            return BattleState.LostFish;
        }

        if (fishStamina < 0) {
            return BattleState.GotFish;
        }

        if (playerStamina < 0) {
            return BattleState.LostBait;
        }

        return BattleState.StillInBattle;
    }

    public void SetRandomFishState () {
        fishState = Random.Range(0, 2) > 0 ? FishState.Attacking: FishState.Defending;
    }

    public void DoAction (PlayerAction action)
    {
        noActionTime = 0;
        switch (action)
        {
            case PlayerAction.Attack:
            {
                if (fishState == FishState.Attacking) {
                    playerStamina -= fishStats.damage;
                } else if (fishState == FishState.Defending) {
                    fishStamina -= baitStats.damage;
                    fishState = FishState.Exhausted;
                    exhaustedTime = Random.Range(fishStats.minMilliSecExhausted, fishStats.maxMilliSecExhausted);
                } else if (fishState == FishState.Exhausted) {
                    SetRandomFishState();
                }
                break;
            }
            case PlayerAction.Defend:
            {
                if (fishState == FishState.Attacking) {
                    fishStamina -= baitStats.damage;
                    fishState = FishState.Exhausted;
                    exhaustedTime = Random.Range(fishStats.minMilliSecExhausted, fishStats.maxMilliSecExhausted);
                } else if (fishState == FishState.Defending) {
                    playerStamina -= fishStats.damage;
                } else if (fishState == FishState.Exhausted) {
                    SetRandomFishState();
                }
                break;
            }
            case PlayerAction.Reel:
            {
                if (fishState == FishState.Exhausted) {
                    playerStamina = Mathf.Min(playerStamina + baitStats.staminaRegen, baitStats.stamina);
                } else {
                    fishStamina = Mathf.Min(fishStamina + fishStats.staminaRegen, fishStats.stamina);
                }
                break;
            }
  
        }
    }
}
