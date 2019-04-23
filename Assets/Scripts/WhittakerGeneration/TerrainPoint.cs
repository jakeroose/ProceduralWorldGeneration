using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPoint {

	public Color color;
	public Vector3 coord;

	public TerrainPoint(Vector3 coord, Color color) {
		this.color = color;
		this.coord = coord;
	}

	public TerrainPoint() {

	}
}