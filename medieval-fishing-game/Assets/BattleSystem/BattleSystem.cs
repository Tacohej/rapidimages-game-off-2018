using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="MedievalFishing/Battle/BattleSystem")]
public class BattleSystem : ScriptableObject {

    public enum FishState
    {
        Struggling,
        Escapes,
        Exhausted
    }

    public enum PlayerAction
    {
        Slack,
        Pull,
        Reel
    }

    public enum PlayerState 
    {
        Idle,
        Slacking,
        Pulling,
        Reeling
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
    public PlayerState playerState;
    public float MAX_COOLDOWN = 3;
    public float escapeTimer = 0;

    public float coolDownTime = 0;
    public float fleeAfterMilliSec = 0;

    public bool IsPlayerActionOnCooldown () {
        return coolDownTime > 0;
    }

	public void StartBattle (FishStats fishStats, BaitStats baitStats)
    {
        this.fishStats = fishStats;
        this.baitStats = baitStats;
        playerStamina = baitStats.stamina;
        fishStamina = fishStats.stamina;
        maxPlayerStamina = playerStamina;
        maxFishStamina = fishStamina;
        fleeAfterMilliSec = fishStats.escapesAfterSeconds;

        fishState = FishState.Struggling;
        playerState = PlayerState.Idle;
        escapeTimer = 0;
        coolDownTime = 0;
	}

    public BattleState UpdateBattle (float deltaTime)
    {

        if (!IsPlayerActionOnCooldown() && playerState != PlayerState.Idle) {
            playerState = PlayerState.Idle;
            if (fishState != FishState.Escapes) {
                SetFishStateToRandom();
            }
        } else {
            coolDownTime -= deltaTime;
        }

        switch (fishState) {
            case FishState.Escapes:
            {
                escapeTimer -= deltaTime;
                
                if (playerState == PlayerState.Pulling) {
                    fishStamina -= baitStats.damage * 20;
                    escapeTimer = 0;
                    coolDownTime = 0;
                    playerState = PlayerState.Idle;
                    SetFishStateToStruggleOrExhausted();
                    return BattleState.StillInBattle;
                } else if (playerState == PlayerState.Reeling) {
                    playerStamina -= fishStats.damage;
                }

                if (escapeTimer <= 0) {
                    return BattleState.LostFish;
                }

                break;
            }
            case FishState.Exhausted:
            {
                if (playerState == PlayerState.Reeling) {
                    fishStamina -= baitStats.damage;
                } else {
                    fishStamina = Mathf.Min(fishStamina + fishStats.staminaRegen, fishStats.stamina);
                }
                break;
            }
            case FishState.Struggling:
            {
                if (playerState == PlayerState.Reeling)
                {
                    playerStamina -= fishStats.damage;
                    fishStamina -= baitStats.damage;
                } else if (playerState == PlayerState.Slacking)
                {
                    // playerStamina += baitStats.staminaRegen;
                    playerStamina = Mathf.Min(playerStamina + baitStats.staminaRegen, baitStats.stamina);
                } else {
                    playerStamina -= fishStats.staminaRegen;
                }
                break;
            }
        }

        if (playerStamina <= 0) {
            return BattleState.LostFish;
        }

        if (fishStamina <= 0) {
            return BattleState.GotFish;
        }

        return BattleState.StillInBattle;
    }

    public void SetFishStateToStruggleOrExhausted () {
        fishState = Random.Range(0, 2) > 0 ? FishState.Struggling: FishState.Exhausted;
    }

    public void SetFishStateToRandom () {
        var v = Random.Range(0,2);
        if (v > 0) {
            SetFishStateToStruggleOrExhausted();
        } else {
            SetFishStateToEscaping();
        }
    }

    public void SetFishStateToEscaping () {
        escapeTimer = fishStats.escapesAfterSeconds;
        fishState = FishState.Escapes;
    }

    public void DoAction (PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.Slack:
            {
                playerState = PlayerState.Slacking;
                break;
            }
            case PlayerAction.Pull:
            {
                playerState = PlayerState.Pulling;
                break;
            }
            case PlayerAction.Reel:
            {
                playerState = PlayerState.Reeling;
                break;
            }
  
        }

        coolDownTime = MAX_COOLDOWN;

    }
}
