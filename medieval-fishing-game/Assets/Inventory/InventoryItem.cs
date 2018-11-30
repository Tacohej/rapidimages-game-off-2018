using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class InventoryItem : ScriptableObject {
	public string title;
	public string boon;
	public string flavorText;
	public Sprite diplayImage;
	[NonSerialized] public bool found;
}

[CreateAssetMenu (menuName = "MedievalFishing/Inventory/Rod")]
public class RodItem : InventoryItem {

	[Header("Battle")]
	public int stamina = 10000;
	public int staminaRegen = 5;
	public int damage = 20;
}

[CreateAssetMenu (menuName = "MedievalFishing/Inventory/Bait")]
public class BaitItem : InventoryItem {

}