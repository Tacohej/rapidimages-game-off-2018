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

    private TextScroller textScroller;
    private bool tutorialMode;

    public bool IsPlayerActionOnCooldown () {
        return coolDownTime > 0;
    }

	public void StartBattle (FishStats fishStats, BaitStats baitStats, TextScroller textScroller, bool tutorialMode)
    {
        this.textScroller = textScroller;
        this.tutorialMode = tutorialMode;
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
                    textScroller.AddScrollText("It got away!", true, true);
                    if (tutorialMode) {
                        textScroller.AddScrollText("Remember to PULL next time.");
                    }
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
                    playerStamina = Mathf.Min(playerStamina + baitStats.staminaRegen, baitStats.stamina);
                } else {
                    playerStamina -= fishStats.staminaRegen;
                }
                break;
            }
        }

        if (playerStamina <= 0) {
            if (tutorialMode) {
                textScroller.AddScrollText("You will get the next one!", true, true);
            }
            return BattleState.LostFish;
        }

        if (fishStamina <= 0) {
            if (tutorialMode) {
                textScroller.AddScrollText("You caught it!", true, true);
            }
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
        if (tutorialMode) {
            textScroller.AddScrollText("Uh oh! The fish is trying to escape.", true, true);
            textScroller.AddScrollText("Use PULL");
        }
    }

    public void DoAction (PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.Slack:
            {
                playerState = PlayerState.Slacking;
                if (tutorialMode) {
                    if (fishState == FishState.Struggling) {
                        textScroller.AddScrollText("Nice! Regain some stamina.", true, true);
                    } else if (fishState == FishState.Exhausted) {
                        textScroller.AddScrollText("Hmmm, fish is now regaining stamina.", true, true);
                    } else if (fishState == FishState.Escapes) {
                        textScroller.AddScrollText("No time! PULL to interupt!", true, true);
                    }
                }
                break;
            }
            case PlayerAction.Pull:
            {
                playerState = PlayerState.Pulling;
                if (tutorialMode) {
                    if (fishState == FishState.Struggling) {
                        textScroller.AddScrollText("It's not very effective...", true, true);
                    } else if (fishState == FishState.Escapes) {
                        textScroller.AddScrollText("Good! Interupted!", true, true);
                    } else if (fishState == FishState.Exhausted) {
                        textScroller.AddScrollText("It's not very effective...", true, true);
                    }
                }
                break;
            }
            case PlayerAction.Reel:
            {
                playerState = PlayerState.Reeling;
                if (tutorialMode) {
                    if (fishState == FishState.Struggling) {
                        textScroller.AddScrollText("Risky move!", true, true);
                    } else if (fishState == FishState.Escapes) {
                        textScroller.AddScrollText("No time! PULL to interupt!", true, true);
                    } else if (fishState == FishState.Exhausted) {
                        textScroller.AddScrollText("Excellent choice!", true, true);
                    }
                }
                break;
            }
  
        }
        if (tutorialMode) {
            textScroller.AddScrollText("Pick a new ACTION.");
        }
        coolDownTime = MAX_COOLDOWN;

    }
}
