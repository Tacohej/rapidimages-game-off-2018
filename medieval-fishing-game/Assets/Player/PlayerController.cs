using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public enum State
	{
		Setup,
		Cast_1,
		Cast_2,
		Reel,
		Loot
	}

	public CastStats castStats;

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
					currentState = State.Cast_1;
					castStats.Reset();
				}
				break;
			}

			case State.Cast_1:
			{
				var speed = 1f;

				var ntime = Time.time * speed % Mathf.PI;
				var a = 100 - Mathf.Abs(Mathf.Sin(ntime) + Mathf.PI / 2) * 35;

				castStats.currentVelocity = a;
				// castStats.currentGravity = a;
				// var a = Mathf.Abs(Mathf.Sin(Time.time) * 3);
				// print(a);
				// castStats.currentVelocity = 10 + a * a * a * a;
				castStats.currentGravity = -15 + Mathf.Abs(Mathf.Sin(ntime) + Mathf.PI / 2) * 5;
				// castStats.currentGravity = -10 - Mathf.Sin(Time.time * 5) * 5;
				// print(castStats.currentGravity);
				if (Input.GetKeyDown(KeyCode.Space))
				{
					currentState = State.Cast_2;
					// if (!castStats.velocityIsSet)
					// {
					// 	castStats.velocityIsSet = true;
					// } else if (!castStats.accuracyIsSet)
					// {
					// 	castStats.accuracyIsSet = true;
					// 	currentState = State.Reel;
					// 	break;
				}
				break;
			}
			case State.Cast_2:
			{

				break;
			}
		}
	}
}
