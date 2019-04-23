using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* TODO:
 * - Draw on a plane instead of gizmos
 * - Adjustable variables for the different regions
 * - 0 index the altitude
 * 
*/

public class TerrainGenerator : MonoBehaviour {
	public int width = 100, height = 100, depth = 10, mountainHeight = 2, detailHeight = 2;
	public float S = 12f, mountainScale = 5f, detailScale = 5f;
	public float mountainZone = 0.7f, grassZone = 0.2f, sandZone = 0f;

	private TerrainPoint[,] m_terrainHeights;

	Mesh mesh;


	// Use this for initialization
	void Start() {
		//GenerateWorld();
		ConstructMesh();
	}

	// Update is called once per frame
	void Update() {
		if ( Input.anyKeyDown ) {
			//GenerateWorld();
			ConstructMesh();

		}
	}

	MeshFilter meshFilter;
	GameObject meshObj;
	public void ConstructMesh(){
		Vector3[] verticies = new Vector3[width*height];
		int[] triangles = new int[(width-1) * (height-1) * 6];
		int triIndex = 0;

		// initialize new mesh for the terrain
		if(meshFilter == null){
			meshFilter = new MeshFilter();
			meshObj = new GameObject("mesh");
			meshObj.transform.parent = transform;
			meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
			meshFilter = meshObj.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = new Mesh();
		}

		mesh = meshFilter.sharedMesh;

		for(int y = 0; y < height; y++){
			for(int x = 0; x < width; x++){
				int i = y + x * height;
				Vector2 percent = new Vector2((float)x/(width - 1), (float)y/(height - 1));
				verticies[i] = new Vector3((percent.x - 0.5f) * 2, 0, (percent.y - 0.5f) * 2);

				// create triangles for mesh
				if(x != width - 1 && y != height - 1){
					//Debug.Log(triIndex);
					triangles[triIndex] = i;
					triangles[triIndex + 1] = i + height + 1;
					triangles[triIndex + 2] = i + height;

					triangles[triIndex + 3] = i;
					triangles[triIndex + 4] = i + 1;
					triangles[triIndex + 5] = i + 1 + height;
					
					triIndex += 6;
				}
			}
		}
		mesh.Clear();
		mesh.vertices = verticies;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
	}

	public void GenerateWorld() {
		// noise for initial landmass
		OpenSimplexNoise baseNoise = new OpenSimplexNoise((byte)Time.time);
		// noise for adding more detail to landscape
		OpenSimplexNoise detailNoise = new OpenSimplexNoise((byte)Random.value*2);
		// noise for generating mountains
		OpenSimplexNoise mountainNoise = new OpenSimplexNoise((byte)Random.value);

		m_terrainHeights = new TerrainPoint[height, width];

		for ( int i = 0; i < height; i++ ) {
			for ( int j = 0; j < width; j++ ) {
				float noiseVal = (float)baseNoise.eval(i / S, j/ S)*depth;
				float detVal = (float)detailNoise.eval(i/detailScale,j/detailScale)*detailHeight;


				float h = noiseVal + detVal;

				// we want to scale moutains based on height of terrain (for the time being)
				// higher terrain is more affected by the mountain noise
				// want it to always be additive though, don't want to subtract from mountains
				float normalizedH = h/(depth + detailHeight); // between 0 and 1
				float mountVal = (float)mountainNoise.eval(i/mountainScale,j/mountainScale) + 1; // between 0 and 2

				// height at which terrain is mountain
				if ( normalizedH > mountainZone){
					float t = (normalizedH - mountainZone) / (1 - mountainZone); // how much to apply the mountain detail, from 1 to 2
					//Debug.Log("Before height: " + h);
					//Debug.Log("t: " + t + ", mountVal: " + mountVal + ", normalizedH: " + normalizedH);
					// current height factor * mountain noise factor * max mountain height
					h += t * mountVal * mountainHeight;
					//Debug.Log("After height: " + h);
				}

				m_terrainHeights[i,j] = new TerrainPoint(new Vector3(j, h, i), GetHeightColor(h));


			}
		}
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

	void OnDrawGizmos() {
		//for ( int i = 0; i < height; i++ ) {
		//	for ( int j = 0; j < width; j++ ) {
		//		Gizmos.color = m_terrainHeights[i, j].color;
		//		Gizmos.DrawCube(m_terrainHeights[i, j].coord, Vector3.one);
		//	}
		//}
		if ( m_terrainHeights == null ) return;
		foreach (TerrainPoint p in m_terrainHeights){
			Gizmos.color = p.color;
			Gizmos.DrawCube(p.coord, Vector3.one);
		}
	}
}
