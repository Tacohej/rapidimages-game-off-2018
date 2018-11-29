using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFirstSelectedBait : MonoBehaviour {

	public Inventory inventory;
	public BaitItem bait;

	void Start () {
		bait.found = true;
		inventory.SelectedBait = bait;
	}
}