using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Holds information about the terrain.
 * May include cutoffs for different zones, dimentions of terrain,
 * and references to different noise settings.
 * 
 */
[CreateAssetMenu()]
public class TerrainSettings : ScriptableObject {

	public NoiseSettings noiseSettings;

	//public int width = 100, height = 100, depth = 10, mountainHeight = 2, detailHeight = 2;
	//public float S = 12f, mountainScale = 5f, detailScale = 5f;
	public float mountainZone = 0.7f, grassZone = 0.2f, sandZone = 0f;
}
