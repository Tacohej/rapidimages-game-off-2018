using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaitCameraController : MonoBehaviour {

	[SerializeField]
	private RenderTexture renderTexture;

	void Start () {
		renderTexture = new RenderTexture(Screen.width, Screen.height, 2);
		Shader.SetGlobalTexture("BaitCameraTexture", renderTexture);
		GetComponent<Camera>().targetTexture = renderTexture;
	}
}
