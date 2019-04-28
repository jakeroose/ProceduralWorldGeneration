using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Main class for holding all of the information for the terrain,
 * used in editor to change features of terrain.
 * Only handles terrain. Other things like trees, animals, etc. should
 * be handled elsewhere.
 * 
 * TODO:
 * - add minHeight for water values, use that for coloring as well.
 * BUGS:
 * - colors should be set based on local y, not on global y
 */

public class Terrain : MonoBehaviour {

	[Range(1,50)]
	public int pointDensity = 10;
	[Range(1, 20)]
	public int terrainSize = 3;
	public bool autoUpdate = true;
	public MinMax elevationMinMax;

	public ColorSettings colorSettings;
	public TerrainSettings terrainSettings;

	// Used for editor scripting
	[HideInInspector]
	public bool terrainSettingsFoldout;
	[HideInInspector]
	public bool colorSettingsFoldout;

	public ColorGenerator colorGenerator = new ColorGenerator();
	TerrainGenerator terrainGenerator;

	[SerializeField, HideInInspector]
	MeshFilter meshFilter;
	Mesh mesh;

	public void GenerateTerrain(){
		Initialize();
		ConstructMesh();
	}

	void Initialize() {
		elevationMinMax = new MinMax();
		colorGenerator.UpdateSettings(colorSettings);
		if(terrainGenerator == null){
			terrainGenerator = new TerrainGenerator(terrainSettings);
		}

		if ( meshFilter == null ) {
			// initialize new mesh for the terrain
			meshFilter = new MeshFilter();
			GameObject meshObj = new GameObject("mesh");
			meshObj.transform.parent = transform;
			meshObj.AddComponent<MeshRenderer>();
			meshFilter = meshObj.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = new Mesh();
		}
		meshFilter.GetComponent<MeshRenderer>().sharedMaterial = colorSettings.terrainMaterial;
	}

	public void ConstructMesh(){
		int resolution = pointDensity * terrainSize;
		Vector3[] verticies = new Vector3[resolution*resolution];
		Debug.Log("Num Vertices = " + (resolution * resolution));
		int[] triangles = new int[(resolution-1) * (resolution-1) * 6];
		Debug.Log("Num Triangles = " + ((resolution - 1) * (resolution - 1) * 6));
		int triIndex = 0;

		mesh = meshFilter.sharedMesh;
		// set indexFormat from 16bit to 32bit so that we can have a more detailed mesh
		meshFilter.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

		for ( int y = 0; y < resolution; y++ ) {
			for ( int x = 0; x < resolution; x++ ) {
				// (x,y) / pointDensity = game coordinate 
				Vector2 point = new Vector2(x, y) / (float)pointDensity;

				// noiseVal is the height value at our coordinate
				float noiseVal = terrainGenerator.CalculatePointOnPlanet(point);
				elevationMinMax.AddValue(noiseVal); // track min/max values for our terrain

				// index of the current point
				int i = y + x * resolution;
				// set the vertex for our mesh
				verticies[i] = new Vector3(point.x, noiseVal, point.y);

				// create triangles for mesh
				// triangles are created in clockwise pattern
				if ( x != resolution - 1 && y != resolution - 1 ) {
					triangles[triIndex] = i; // starting point
					triangles[triIndex + 1] = i + resolution + 1; // point on the next row + 1
					triangles[triIndex + 2] = i + resolution; 

					triangles[triIndex + 3] = i;
					triangles[triIndex + 4] = i + 1;
					triangles[triIndex + 5] = i + 1 + resolution;

					triIndex += 6;
				}
			}
		}

		// update color generator with new minmax values
		colorGenerator.UpdateElevation(elevationMinMax);
		//colorGenerator.UpdateColors();

		// update mesh with new triangles
		mesh.Clear();
		mesh.vertices = verticies;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}

	// To be called when only color settings are updated
	public void OnColorSettingsUpdated() {
		if(autoUpdate){
			Initialize();
			colorGenerator.UpdateSettings(colorSettings);
			colorGenerator.UpdateColors();
		}
	}

	// To be called when only terrain gen settings are updated
	public void OnTerrainSettingsUpdated() {
		if(autoUpdate){
			Initialize();
			ConstructMesh();
		}
	}
}
