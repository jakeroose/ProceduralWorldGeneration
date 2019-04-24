using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings {

	public int width = 100, height = 100, depth = 10, mountainHeight = 2, detailHeight = 2;
	public float strength = 12f, mountainScale = 5f, roughness = 5f;
	public float mountainZone = 0.7f, grassZone = 0.2f, sandZone = 0f;

	public Vector3 center;
}
