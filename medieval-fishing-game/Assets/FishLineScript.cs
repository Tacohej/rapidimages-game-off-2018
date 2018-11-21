using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishLineScript : MonoBehaviour {

	public float waterLevelYPosition = 0;
	private Rigidbody rbody;

	void Start () {
		rbody = GetComponent<Rigidbody>();
	}
	
	void Update () {

		this.rbody.useGravity = (this.transform.position.y > waterLevelYPosition);

	}
}
