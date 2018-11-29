using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class InventoryItem : ScriptableObject {
	public string title;
	public string flavorText;
	public Sprite diplayImage;
	[NonSerialized] public bool found;
}

[CreateAssetMenu (menuName = "MedievalFishing/Inventory/Rod")]
public class RodItem : InventoryItem {

}

[CreateAssetMenu (menuName = "MedievalFishing/Inventory/Bait")]
public class BaitItem : InventoryItem {

}