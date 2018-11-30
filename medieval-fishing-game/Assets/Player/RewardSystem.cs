using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName="MedievalFishing/Player/RewardSystem")]
public class RewardSystem : ScriptableObject
{
    public Inventory inventory;

    [Header("Baits")]
    public BaitItem rewardLevel2;
    public BaitItem rewardLevel4;
    public BaitItem rewardLevel6;
    
    [Header("Baits")]
    public RodItem rewardLevel3;
    public RodItem rewardLevel5;
    public RodItem rewardLevel7;

    [SerializeField]
    int currentLevel = 1;
    
    int currentXP = 0;
    int fishesCaught = 0;
    int nextLevelUp;
    int levelIncreaseAmount;

    [Header("Designers may modify these values")]
    public int baseLevelUp = 100;
    public int baseLevelScaleFactor = 300;

    public void Reset() {
        currentLevel = 1;
        currentXP = 0;
        fishesCaught = 0;
        levelIncreaseAmount = baseLevelScaleFactor;
        nextLevelUp = baseLevelUp;
    }

    public void LevelUp (TextScroller log) {
        currentLevel ++;
        log.AddScrollText("You Leveled up!");
        log.AddScrollText("Current Level: " + currentLevel);
        nextLevelUp += levelIncreaseAmount;

        switch(currentLevel) {
            case 2: { UnlockBait(rewardLevel2, log); break; }
            case 3: { UnlockRod (rewardLevel3, log); break; }
            case 4: { UnlockBait(rewardLevel4, log); break; }
            case 5: { UnlockRod (rewardLevel5, log); break; }
            case 6: { UnlockBait(rewardLevel6, log); break; }
            case 7: { UnlockRod (rewardLevel7, log); break; }
        }

    }

    public void UnlockRod (RodItem rodItem, TextScroller log) {
        log.AddScrollText("You unlocked " + rodItem.title);
        log.AddScrollText("- " + rodItem.boon);
        rodItem.found = true;
    }

    public void UnlockBait (BaitItem baitItem, TextScroller log) {
        log.AddScrollText("You unlocked " + baitItem.title);
        log.AddScrollText("- " + baitItem.boon);
        baitItem.found = true;
    }


    public void FishCaught(int xpGain, TextScroller log) // also add probability to gain item
    {
        fishesCaught += 1;
        int nrLevelsGained = (currentXP + xpGain) / nextLevelUp;
        currentXP = (currentXP + xpGain) % nextLevelUp;

        if (nrLevelsGained > 0) {
            LevelUp(log);
            if (nrLevelsGained > 1) { Debug.LogWarning("OPS! Should not be able to level more than once"); }
        }

        if (currentLevel > 9) {
            log.AddScrollText("You won the game. Keep playing if you want.");
        }

    }
}
