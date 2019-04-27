using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidNoiseFilter :INoiseFilter {
	NoiseSettings.RigidNoiseSettings settings;
	OpenSimplexNoise baseNoise;

	public RigidNoiseFilter(NoiseSettings.RigidNoiseSettings settings) {
		this.settings = settings;
		baseNoise = new OpenSimplexNoise(Random.Range(0, 1000));
	}

	// Generates a ~random value based on point
	public float Evaluate(Vector2 point) {
		float noiseVal = 0;
		float frequency = settings.baseRoughness; // 0-1
		float amplitude = 1;
		float weight = 1;

		for ( int i = 0; i < settings.numLayers; i++ ) {
			Vector3 p = point * frequency + settings.center; // point we iterate on in next loop

			float v = 1-Mathf.Abs((float)baseNoise.eval(p.x, p.y)); // invert and clamp from 0-1
			//v *= v; // to make ridges more pronounced and valleys more flat
			v *= weight;
			weight = v * Mathf.Clamp01(settings.weightMultiplier);

			noiseVal += v * amplitude;
			frequency *= settings.roughness;
			amplitude *= settings.persistence;
		}

		noiseVal = Mathf.Max(0, noiseVal);
		return noiseVal *= settings.strength;
	}
}
