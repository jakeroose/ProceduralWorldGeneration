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

	public float mountainZone = 0.7f, grassZone = 0.2f, sandZone = 0f;

	[Range(32,256)]
	public int resolution = 100;
	public bool autoUpdate = true;
	public MinMax elevationMinMax;

	public ColorSettings colorSettings;
	public TerrainSettings terrainSettings;

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
		Vector3[] verticies = new Vector3[resolution*resolution];
		int[] triangles = new int[(resolution-1) * (resolution-1) * 6];
		int triIndex = 0;

		mesh = meshFilter.sharedMesh;

		// TODO: Resolution should actually change resolution and not "zoom in"!!!!
		for ( int y = 0; y < resolution; y++ ) {
			for ( int x = 0; x < resolution; x++ ) {

				// TODO: Go through this section and refactor to work with new variables.
				// things like detailHeight and depth shouldn't be used any more. 
				// Also, we shouldn't have to do any more calculations on noiseVal here,
				// that should all be done by the time we get it from TerrainGenerator
				float noiseVal = terrainGenerator.CalculatePointOnPlanet(new Vector3(x, y));
				float h = noiseVal;

				// we want to scale moutains based on height of terrain (for the time being)
				// higher terrain is more affected by the mountain noise
				// want it to always be additive though, don't want to subtract from mountains
				float normalizedH = h/(terrainSettings.noiseSettings.simpleNoiseSettings.depth + terrainSettings.noiseSettings.simpleNoiseSettings.detailHeight); // between 0 and 1

				int i = y + x * resolution;
				Vector2 percent = new Vector2(x, y) / (resolution - 1);
				float tHeight = h/resolution;
				tHeight = (tHeight < sandZone ? 0 : tHeight); // move to terrainGenerator
				elevationMinMax.AddValue(tHeight);
				verticies[i] = new Vector3((percent.x - 0.5f) * 2, tHeight, (percent.y - 0.5f) * 2);

				// create triangles for mesh
				if ( x != resolution - 1 && y != resolution - 1 ) {
					triangles[triIndex] = i;
					triangles[triIndex + 1] = i + resolution + 1;
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

	//public void ConstructMeshOLD() {
	//	Vector3[] verticies = new Vector3[resolution*resolution];
	//	int[] triangles = new int[(resolution-1) * (resolution-1) * 6];
	//	int triIndex = 0;

	//	mesh = meshFilter.sharedMesh;

	//	// noise for initial landmass
	//	OpenSimplexNoise baseNoise = new OpenSimplexNoise(1);
	//	// noise for adding more detail to landscape
	//	OpenSimplexNoise detailNoise = new OpenSimplexNoise(2);
	//	// noise for generating mountains
	//	OpenSimplexNoise mountainNoise = new OpenSimplexNoise(3);

	//	for ( int y = 0; y < resolution; y++ ) {
	//		for ( int x = 0; x < resolution; x++ ) {
	//			float noiseVal = (float)baseNoise.eval(x / S, y/ S)*depth;
	//			float detVal = (float)detailNoise.eval(x/detailScale,y/detailScale)*detailHeight;

	//			float h = noiseVal + detVal;

	//			// we want to scale moutains based on height of terrain (for the time being)
	//			// higher terrain is more affected by the mountain noise
	//			// want it to always be additive though, don't want to subtract from mountains
	//			float normalizedH = h/(depth + detailHeight); // between 0 and 1
	//			float mountVal = (float)mountainNoise.eval(x/mountainScale,y/mountainScale) + 1; // between 0 and 2

	//			// height at which terrain is mountain
	//			if ( normalizedH > mountainZone ) {
	//				float t = (normalizedH - mountainZone) / (1 - mountainZone); // how much to apply the mountain detail, from 1 to 2
	//				h += t * mountVal * mountainHeight;
	//			}

	//			int i = y + x * resolution;
	//			Vector2 percent = new Vector2(x, y) / (resolution - 1);
	//			float tHeight = h/resolution;
	//			tHeight = (tHeight < sandZone ? 0 : tHeight);
	//			elevationMinMax.AddValue(tHeight);
	//			verticies[i] = new Vector3((percent.x - 0.5f) * 2, tHeight, (percent.y - 0.5f) * 2);

	//			// create triangles for mesh
	//			if ( x != resolution - 1 && y != resolution - 1 ) {
	//				//Debug.Log(triIndex);
	//				triangles[triIndex] = i;
	//				triangles[triIndex + 1] = i + resolution + 1;
	//				triangles[triIndex + 2] = i + resolution;

	//				triangles[triIndex + 3] = i;
	//				triangles[triIndex + 4] = i + 1;
	//				triangles[triIndex + 5] = i + 1 + resolution;

	//				triIndex += 6;
	//			}
	//		}
	//	}

	//	// update color generator with new minmax values
	//	colorGenerator.UpdateElevation(elevationMinMax);
	//	colorGenerator.UpdateColors();

	//	// update mesh with new triangles
	//	mesh.Clear();
	//	mesh.vertices = verticies;
	//	mesh.triangles = triangles;
	//	mesh.RecalculateNormals();
	//}

	public void OnColorSettingsUpdated() {
		if(autoUpdate){
			Initialize();
			colorGenerator.UpdateSettings(colorSettings);
			colorGenerator.UpdateColors();
		}
	}

	public void OnTerrainSettingsUpdated() {
		if(autoUpdate){
			Initialize();
			ConstructMesh();
		}
	}
}
