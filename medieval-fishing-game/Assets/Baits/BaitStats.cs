using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="MedievalFishing/Baits/BaitStats")]
public class BaitStats : ScriptableObject {

    public int stamina = 10000;
    public int staminaRegen = 5;
    public int damage = 20;
}
