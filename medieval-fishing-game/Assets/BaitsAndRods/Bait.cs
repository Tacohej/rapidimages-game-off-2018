using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bait : MonoBehaviour {
	
	private GameObject hookedFish;
	private bool isTaken = false;

	public BaitItem baitItem;

	public enum Type
	{
		common,
		uncommon,
		rare,
		legendary
	}

	public Type type;

	public GameObject GetHookedFish () {
		return hookedFish;
	}

	public void ReleaseHookedFish () {
		hookedFish = null;
	}

	public bool IsTaken () {
		return isTaken;
	}

	public void Reset() {
		this.isTaken = false;
	}

	public void HookFish(GameObject fish) {
		this.hookedFish = fish;
		this.isTaken = true;
		hookedFish.transform.parent = this.gameObject.transform;
	}
}
