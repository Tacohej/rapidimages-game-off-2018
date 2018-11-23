﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostProcessScript : MonoBehaviour {

	public Material material;

	void OnRenderImage(RenderTexture source, RenderTexture destination) {
		if (material != null) {
			Graphics.Blit(source, destination, material);
		}
	}
}
