using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleUseInventory : MonoBehaviour {

	public Inventory inventory;
	public RodItem itemToFind;

	void OnEnable () {
		inventory.onSelectedRodChanged += OnSelectedRodChanged;
		inventory.onSelectedBaitChanged += OnSelectedBaitChanged;

		Invoke ("FindItem", 2);
	}

	void OnDisabled () {
		inventory.onSelectedRodChanged -= OnSelectedRodChanged;
		inventory.onSelectedBaitChanged -= OnSelectedBaitChanged;
	}

	void OnSelectedRodChanged (RodItem item) {
		Debug.Log ("selected rod changed to: " + item.title);
	}

	void OnSelectedBaitChanged (BaitItem item) {
		Debug.Log ("selected bait changed to: " + item.title);
	}

	void FindItem () {
		itemToFind.found = true;
	}
}