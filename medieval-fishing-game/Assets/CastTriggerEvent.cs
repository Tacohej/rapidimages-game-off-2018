using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastTriggerEvent : MonoBehaviour {

	public PlayerController playerController;

	public void FinishCast( ) {
		playerController.FinishCasting();
	}
}
