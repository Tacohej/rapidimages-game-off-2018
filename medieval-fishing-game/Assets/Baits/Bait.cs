using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bait : MonoBehaviour {
	
	[SerializeField]
	private GameObject hookedFish;
	[SerializeField]
	private bool isTaken = false;

	public enum Type
	{
		common,
		uncommon,
		rare,
		legendary
	}

	public Type type;

	public bool IsTaken () {
		return isTaken;
	}

	public void HookFish(GameObject fish) {
		this.hookedFish = fish;
		this.isTaken = true;
		hookedFish.transform.parent = this.gameObject.transform;
	}
}
