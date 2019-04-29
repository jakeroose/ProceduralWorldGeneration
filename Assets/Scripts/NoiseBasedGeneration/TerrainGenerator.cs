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
		float baseLayer = noiseFilters[0].Evaluate(point); // save base layer in case used as mask
		float elevation = settings.noiseLayers[0].enabled ? baseLayer : 0;

		for (int i = 1; i < noiseFilters.Length; i++){
			if(settings.noiseLayers[i].enabled){
				if(settings.noiseLayers[i].useFirstLayerAsMask){
					elevation += noiseFilters[i].Evaluate(point) * baseLayer;
				}
				else {
					elevation += noiseFilters[i].Evaluate(point);
				}
			}
		}
		// Keep terrain at least minHeight specified in terrainSettings
		elevation = Mathf.Max(elevation, settings.minHeight);
		return elevation;
	}
}
