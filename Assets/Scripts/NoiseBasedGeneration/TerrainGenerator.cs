using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Used to calculate the height for a point on the terrain.
 */
public class TerrainGenerator {

	TerrainSettings settings;
	INoiseFilter[] noiseFilters;

	public TerrainGenerator(TerrainSettings settings){
		this.settings = settings;
		noiseFilters = new INoiseFilter[settings.noiseLayers.Length];

		for(int i = 0; i < noiseFilters.Length; i++){
			noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings);
		}
	}

	// Takes in a point and returns the elevation for that point
	public float CalculatePointOnPlanet(Vector2 point){
		float elevation = 0;
		for(int i = 0; i < noiseFilters.Length; i++){
			if(settings.noiseLayers[i].enabled){
				if(settings.noiseLayers[i].useFirstLayerAsMask && i > 0){
					// TODO: Optimize, don't need to calc noiseFilters[0] every loop.
					elevation += noiseFilters[i].Evaluate(point) * noiseFilters[0].Evaluate(point);
				}
				else {
					elevation += noiseFilters[i].Evaluate(point);
				}
			}
		}
		elevation = Mathf.Max(elevation, settings.minHeight);
		return elevation;
	}
}
