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
    [SerializeField]
    int currentXP = 0;
    [SerializeField]
    int fishesCaught = 0;
    [SerializeField]
    int nextLevelUp = 500;
    [SerializeField]
    int nextLevelScaleFactor = 2;
    [SerializeField]

    [Header("Designers may modify these values")]
    public int baseLevelUp = 500;
    public int baseLevelScaleFactor = 2;

    void OnEnable () {
        inventory.onSelectedRodChanged += OnSelectedRodChanged;
        inventory.onSelectedBaitChanged += OnSelectedBaitChanged;
    }

    void OnSelectedRodChanged (RodItem item) {
       Debug.Log ("selected rod changed to: " + item.title);
    }

    void OnSelectedBaitChanged (BaitItem item) {
       Debug.Log ("selected bait changed to: " + item.title);
    }

    public void Reset() {
        currentLevel = 1;
        currentXP = 0;
        fishesCaught = 0;
        nextLevelScaleFactor = baseLevelScaleFactor;
        nextLevelUp = baseLevelUp;
    }

    void LevelUp (TextScroller log) {
        currentLevel ++;
        log.AddScrollText("You Leveled up!");
        log.AddScrollText("Current Level: " + currentLevel);
        nextLevelUp *= nextLevelScaleFactor;

        if (currentLevel == 2) {
            rewardLevel2.found = true;
            log.AddScrollText("You unlocked " + rewardLevel2.title);
            log.AddScrollText("It says: " + rewardLevel2.flavorText);
        }
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

    }
}
