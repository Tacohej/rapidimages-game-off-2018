using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPreview : MonoBehaviour
{
	public CastMeter castMeter;	
	private LineRenderer lineRenderer;

	void Start ()
	{
		lineRenderer = GetComponent<LineRenderer>();
	}

	void Update ()
	{
		lineRenderer.SetPosition(1, new Vector3(0, castMeter.accuracy * 10, castMeter.velocity * 10));
	}
}
