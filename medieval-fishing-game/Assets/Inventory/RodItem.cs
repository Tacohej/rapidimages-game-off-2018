using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "MedievalFishing/Inventory/Rod")]
public class RodItem : InventoryItem {

	[Header("Battle")]
	public int stamina = 10000;
	public int staminaRegen = 5;
	public int damage = 20;
}