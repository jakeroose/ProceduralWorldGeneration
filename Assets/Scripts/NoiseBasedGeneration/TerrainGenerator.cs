using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator {

	TerrainSettings settings;

	public TerrainGenerator(TerrainSettings settings){
		this.settings = settings;
	}

	public Vector3 CalculatePointOnPlanet(Vector3 point){
		return point;
	}
}
