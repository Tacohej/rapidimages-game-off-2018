using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPreview : MonoBehaviour
{
	public CastMeter castMeter;	
	private LineRenderer lineRenderer;
	[SerializeField]
	private Vector3[] positions;


	void Start ()
	{
		lineRenderer = GetComponent<LineRenderer>();
		positions = new Vector3[10];
	}

	void Update ()
	{

		float t = 0.0f;
		for(int i =0; i < positions.Length; i++) {
			t = i * 1f;
			positions[i].x = 5.0f * t;
			var tt = t + 10.0f;
			positions[i].y = 1.0f - tt * tt;
			positions[i].z = 0.0f;
		}
		// print(lineRenderer.positionCount);

		lineRenderer.SetPositions(positions);
		//lineRenderer.SetPosition(1, new Vector3(0, castMeter.accuracy * 10, castMeter.velocity * 10));
	}
}
