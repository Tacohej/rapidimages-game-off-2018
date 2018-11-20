using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Class maybe should be static instead of MonoBehaviour?
public class PlayerLevel : MonoBehaviour
{
    int currentLevel = 1;
    int nextLevelUp = 5;
    int nextLevelScaleFactor = 2;
    int fishesCaught = 0;

    float ratioConstant = 0.7f;

    List<string> unlockedRods = new List<string>();
    List<string> unlockableRods =  new List<string>(){"Common", "Uncommon", "Rare", "Epic", "Legendary"};

    public void FishCaught()
    {
        fishesCaught += 1;
        
        //print("Fishes caught: " + fishesCaught);
       // print("Amount needed for next level: " + nextLevelUp);
        if (fishesCaught >= nextLevelUp)
        {
            //print("Enough fishes caught, level up!");
            currentLevel += 1;
            nextLevelUp *= nextLevelScaleFactor;

            if (unlockableRods.Count > 0){
                unlockedRods.Add(unlockableRods[0]);
                unlockableRods.RemoveAt(0);

                //print("UnlockedRods now contains: " + String.Join(", ", unlockedRods.ToArray()));
                //print("UnlockableRods now contains: " + String.Join(", ", unlockableRods.ToArray()));            }
            
        }   
    }
}
