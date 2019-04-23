using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* TODO:
 * - move terrain settings into TerrainSettings
 * - move noise filters into their own class(es)
 * - make UI more user friendly
 * - update mesh whenever settings are changed - https://www.youtube.com/watch?v=LyV7cEQyZMk
 * - 0 index the altitude
*/

public class PlaneTerrainGenerator : MonoBehaviour {
	public int width = 100, height = 100, depth = 10, mountainHeight = 2, detailHeight = 2;
	public float S = 12f, mountainScale = 5f, detailScale = 5f;
	public float mountainZone = 0.7f, grassZone = 0.2f, sandZone = 0f;

	[Range(32,256)]
	public int resolution = 100;
	public MinMax elevationMinMax;


	public ColorSettings colorSettings;
	public TerrainSettings terrainSettings;
	public ColorGenerator colorGenerator = new ColorGenerator();

	private TerrainPoint[,] m_terrainHeights;

	Mesh mesh;

	// Use this for initialization
	void Start() {
		//GenerateWorld();
		ConstructMesh();
	}

	// Update is called once per frame
	void Update() {
		//if ( Input.anyKeyDown ) {
		//}
	}

	[SerializeField, HideInInspector]
	MeshFilter meshFilter;

	private void OnValidate() {
		Initialize();
		ConstructMesh();
	}

	void Initialize(){
		elevationMinMax = new MinMax();
		colorGenerator.UpdateSettings(colorSettings);

		// initialize new mesh for the terrain
		if ( meshFilter == null ) {
			meshFilter = new MeshFilter();
			GameObject meshObj = new GameObject("mesh");
			meshObj.transform.parent = transform;
			meshObj.AddComponent<MeshRenderer>();
			meshFilter = meshObj.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = new Mesh();
		}
		meshFilter.GetComponent<MeshRenderer>().sharedMaterial = colorSettings.terrainMaterial;
	}

	public void ConstructMesh() {
		Vector3[] verticies = new Vector3[resolution*resolution];
		int[] triangles = new int[(resolution-1) * (resolution-1) * 6];
		int triIndex = 0;

		mesh = meshFilter.sharedMesh;

		// noise for initial landmass
		OpenSimplexNoise baseNoise = new OpenSimplexNoise(1);
		// noise for adding more detail to landscape
		OpenSimplexNoise detailNoise = new OpenSimplexNoise(2);
		// noise for generating mountains
		OpenSimplexNoise mountainNoise = new OpenSimplexNoise(3);

		for ( int y = 0; y < resolution; y++ ) {
			for ( int x = 0; x < resolution; x++ ) {
				float noiseVal = (float)baseNoise.eval(x / S, y/ S)*depth;
				float detVal = (float)detailNoise.eval(x/detailScale,y/detailScale)*detailHeight;

				float h = noiseVal + detVal;

				// we want to scale moutains based on height of terrain (for the time being)
				// higher terrain is more affected by the mountain noise
				// want it to always be additive though, don't want to subtract from mountains
				float normalizedH = h/(depth + detailHeight); // between 0 and 1
				float mountVal = (float)mountainNoise.eval(x/mountainScale,y/mountainScale) + 1; // between 0 and 2

				// height at which terrain is mountain
				if ( normalizedH > mountainZone ) {
					float t = (normalizedH - mountainZone) / (1 - mountainZone); // how much to apply the mountain detail, from 1 to 2
					//Debug.Log("Before height: " + h);
					//Debug.Log("t: " + t + ", mountVal: " + mountVal + ", normalizedH: " + normalizedH);
					// current height factor * mountain noise factor * max mountain height
					h += t * mountVal * mountainHeight;
					//Debug.Log("After height: " + h);
				}

				int i = y + x * resolution;
				Vector2 percent = new Vector2(x, y) / (resolution - 1);
				float tHeight = h/resolution;
				tHeight = (tHeight < sandZone ? 0 : tHeight);
				elevationMinMax.AddValue(tHeight);
				verticies[i] = new Vector3((percent.x - 0.5f) * 2, tHeight, (percent.y - 0.5f) * 2);

				// create triangles for mesh
				if ( x != resolution - 1 && y != resolution - 1 ) {
					//Debug.Log(triIndex);
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
		colorGenerator.UpdateColors();

		// update mesh with new triangles
		mesh.Clear();
		mesh.vertices = verticies;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}

	public void OnColorSettingsUpdated(){
		Initialize();
		colorGenerator.UpdateColors();
	}

	public void OnTerrainSettingsUpdated(){
		Initialize();
		ConstructMesh();
	}

	Color GetHeightColor(float h) {
		float normalizedH = h/depth;
		//float mountain = 0.7f, hill = 0.2f, sand = 0f;
		if ( normalizedH >= mountainZone ) {
			return new Color(0.5f, 0.5f, 0.5f);
		} else if ( normalizedH >= grassZone ) {
			return new Color(0.05f, 0.9f, 0.15f);
		} else if ( normalizedH >= sandZone ) {
			return new Color(1f, 0.95f, 0.5f);
		} else {
			return new Color(0f, 0.1f, 0.9f);
		}
	}
}
