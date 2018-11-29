using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaitItemSlot : MonoBehaviour {

	public Inventory inventory;
	public BaitItem item;

	private Image image;
	private Image selectionIndicator;

	void Awake () {
		image = transform.Find ("Image").GetComponent<Image> ();
		selectionIndicator = transform.Find ("SelectionIndicator").GetComponent<Image> ();
		selectionIndicator.enabled = false;
	}

	void Update () {
		if (item.found) {
			image.sprite = item.diplayImage;
			var tempColor = image.color;
			tempColor.a = 1f;
			image.color = tempColor;
		} else {
			image.sprite = null;
			var tempColor = image.color;
			tempColor.a = 0.2f;
			image.color = tempColor;
		}

		if (inventory.SelectedBait == item) {
			selectionIndicator.enabled = true;
		} else {
			selectionIndicator.enabled = false;
		}
	}

	public void OnClicked () {
		if (!item.found || inventory.SelectedBait == item) {
			return;
		}

		inventory.SelectedBait = item;
	}
}