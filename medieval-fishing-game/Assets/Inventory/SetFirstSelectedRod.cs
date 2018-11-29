using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetFirstSelectedRod : MonoBehaviour {

	public Inventory inventory;
	public RodItem rod;

	void Start () {
		rod.found = true;
		inventory.SelectedRod = rod;
	}
}