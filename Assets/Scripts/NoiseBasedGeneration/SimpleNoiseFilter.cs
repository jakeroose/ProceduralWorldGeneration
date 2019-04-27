using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Creates random noise based on it's settings.
 */
public class SimpleNoiseFilter : INoiseFilter {

	NoiseSettings.SimpleNoiseSettings settings;
	OpenSimplexNoise baseNoise;

	public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings){
		this.settings = settings;
		baseNoise = new OpenSimplexNoise(Random.Range(0, 1000));
	}

	// Generates a ~random value based on point
	public float Evaluate(Vector2 point){
		float noiseVal = 0;
		float frequency = settings.baseRoughness; // 0-1
		float amplitude = 1;

		for(int i = 0; i < settings.numLayers; i++){
			Vector3 p = point * frequency + settings.center; // point we iterate on in next loop

			float v = (float)baseNoise.eval(p.x, p.y);
			noiseVal += v * 0.5f * amplitude;
			frequency *= settings.roughness;
			amplitude *= settings.persistence;
		}


		return noiseVal *= settings.strength;
	}

}
