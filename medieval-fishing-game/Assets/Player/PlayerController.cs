using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public enum State
	{
		Setup,
		Cast,
		Reel,
		Loot
	}

	public CastMeter castMeter;

	[SerializeField] // will make it visible in the inspector without exposing it
	private State currentState;

	void Start () {
		currentState = State.Setup;
	}
	
	void Update ()
	{
		switch(currentState) {

			case State.Setup:
			{
				// todo: handle equipping bait
				// todo: handle move character
				if (Input.GetKeyDown(KeyCode.Space))
				{
					currentState = State.Cast;
					castMeter.StartMeter();
				}
				break;
			}

			case State.Cast:
			{
				if (Input.GetKeyDown(KeyCode.Space))
				{
					if (!castMeter.velocityIsSet)
					{
						castMeter.velocityIsSet = true;
					} else if (!castMeter.accuracyIsSet)
					{
						castMeter.accuracyIsSet = true;
						currentState = State.Reel;
						break;
					}
				}

				castMeter.UpdateMeter(Time.deltaTime);

				break;
			}
		}
	}

}
