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

	public float minHeight = 0; // Water level
	public NoiseLayer[] noiseLayers;

	[System.Serializable]
	public class NoiseLayer {
		public bool enabled = true;
		public bool useFirstLayerAsMask = true;
		public NoiseSettings noiseSettings;
	}
}
