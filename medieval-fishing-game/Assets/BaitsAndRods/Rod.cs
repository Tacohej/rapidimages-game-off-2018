using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rod : MonoBehaviour {

	public RodItem rodItem;
	[HideInInspector]
	public GameObject tip;

	void Awake () {
		tip = this.transform.Find("Tip").gameObject;
	}
}
