using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Keeps track of settings for a noise filter
 * TODO: Brief description for each of the variables
 */
[System.Serializable]
public class NoiseSettings {
	//public int noiseSeed;

	public enum FilterType { Simple, Rigid };
	public FilterType filterType;

	[ConditionalHide("filterType", 0)]
	public SimpleNoiseSettings simpleNoiseSettings;
	[ConditionalHide("filterType", 1)]
	public RigidNoiseSettings rigidNoiseSettings;

	[System.Serializable]
	public class SimpleNoiseSettings {
		//public int width = 100, height = 100, depth = 10, mountainHeight = 2, detailHeight = 2;
		//public float scale = 12f, mountainScale = 5f;

		public Vector2 center;

		[Range(1,8)]
		public int numLayers = 1;
		[Range(0f,1f)]
		public float baseRoughness = 1f;
		//[Range(0f,0.2f)]
		public float roughness = 0.05f;
		public float strength = 2f;
		public float persistence = 0.5f; // amplitude halved with each layer
	}
	
	[System.Serializable]
	public class RigidNoiseSettings : SimpleNoiseSettings {
		public float weightMultiplier = 0.5f;

	}
}
