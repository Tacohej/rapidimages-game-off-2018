using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName="MedievalFishing/Player/RewardSystem")]
public class RewardSystem : ScriptableObject
{
    public enum RodUnlockLevel {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

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
    RodUnlockLevel unlockLevel = RodUnlockLevel.Common;

    [Header("Designers may modify these values")]
    public int baseLevelUp = 500;
    public int baseLevelScaleFactor = 2;

    public void Reset() {
        currentLevel = 1;
        currentXP = 0;
        fishesCaught = 0;
        nextLevelScaleFactor = baseLevelScaleFactor;
        nextLevelUp = baseLevelUp;
        unlockLevel = RodUnlockLevel.Common;
    }

    void LevelUp () {
        Debug.Log("Level up");
        currentLevel ++;
        nextLevelUp *= nextLevelScaleFactor;
        if (unlockLevel != RodUnlockLevel.Legendary) {
            unlockLevel++;
        }
    }

    public void FishCaught(int xpGain) // also add probability to gain item
    {
        fishesCaught += 1;
        int nrLevelsGained = (currentXP + xpGain) / nextLevelUp;
        currentXP = (currentXP + xpGain) % nextLevelUp;

        if (nrLevelsGained > 0) {
            LevelUp();
            if (nrLevelsGained > 1) { Debug.LogWarning("OPS! Should not be able to level more than once"); }
        }

    }
}
