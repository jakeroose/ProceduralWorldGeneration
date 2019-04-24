using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseFilter {

	NoiseSettings settings;
	OpenSimplexNoise baseNoise = new OpenSimplexNoise(1);

	public NoiseFilter(NoiseSettings settings){
		this.settings = settings;
	}

	public float Evaluate(Vector3 point){
			float noiseVal = (float)baseNoise.eval(
																point.x / settings.strength + settings.center.x, 
																point.y / settings.strength + settings.center.y
															) * settings.depth;
			return noiseVal;
	}

}
