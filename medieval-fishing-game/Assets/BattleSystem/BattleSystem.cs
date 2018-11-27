using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : ScriptableObject {

    public enum FishState
    {
        Pulling,
        Relaxing,
        Exhausted
    }

    public enum PlayerAction
    {
        Reel,
        Lift,
        Relax
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

    public int playerStamina;
    public int fishStamina;
    public int fishState;
    private float totalBattleTime;


	public void StartBattle (FishStats fishStats, BaitStats baitStats)
    {
        this.fishStats = fishStats;
        this.baitStats = baitStats;
        totalBattleTime = 0;
	}

    private bool FishWasLost ()
    {
        // todo: calc 
        return false;
    }

    public BattleState UpdateBattle (float deltaTime)
    {
        // fish might change state

        totalBattleTime += deltaTime;

        if (FishWasLost())
        {
            return BattleState.LostFish;
        }
        

        return BattleState.StillInBattle;
    }

    public void DoAction (PlayerAction action)
    {

        switch (action)
        {
            case PlayerAction.Reel:
                {
                    
                    break;
                }
            case PlayerAction.Lift:
                {

                    break;
                }
            case PlayerAction.Relax:
                {
                    
                    break;
                }
        }
    }
}
