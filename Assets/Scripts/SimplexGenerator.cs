using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplexGenerator : MonoBehaviour {
	public GameObject cube;
	public int width = 100, height = 100, depth = 10;
	public float S = 12f;


	// Use this for initialization
	void Start () {
		GenerateWorld();
		//Generate3DNoise();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GenerateWorld() {
		OpenSimplexNoise noise = new OpenSimplexNoise(123456789);

		for(int i = 0; i < width; i++){
			for(int j = 0; j < height; j++){
				//GameObject go = GameObject.Instantiate(cube);
				double noiseVal = noise.eval(i / S, j/ S);
				//go.transform.position = new Vector3(i, 2f * (float)noiseVal, j);
				//go.GetComponent<MeshRenderer>().materials[0].color = new Color((float) noiseVal, 0, 0);
				CreateCube(i, (float)noiseVal, j);
				//CreateCube(i, (float)noiseVal-1f, j);
			}
		}
	}

	public void Generate3DNoise(){
		OpenSimplexNoise noise = new OpenSimplexNoise(123456789);

		for ( int i = 0; i < width; i++ ) {
			for ( int j = 0; j < height; j++ ) {
				for ( int k = 0; k < height; k++ ) {
					double noiseVal = noise.eval(i / S, j/ S, k/S);
					if(noiseVal > 0.6){
						GameObject go = GameObject.Instantiate(cube);
						go.transform.position = new Vector3(i, j, k);
					}
				}
			}
		}
	}

	GameObject CreateCube(float x, float y, float z){
		GameObject go = GameObject.Instantiate(cube);
		go.transform.position = new Vector3(x, y * depth, z);
		//go.GetComponent<MeshRenderer>().materials[0].color = new Color(y, 0, 0);
		go.GetComponent<MeshRenderer>().materials[0].color = GetHeightColor(y*depth);
		go.transform.localScale = new Vector3(1f, depth, 1f);
		return go;
	}

	Color GetHeightColor(float h) {
		float normalizedH = h/depth;
		float mountain = 0.7f, hill = 0.2f, sand = 0f;
		if ( normalizedH >= mountain ) {
			return new Color(0.5f, 0.5f, 0.5f);
		} else if ( normalizedH >= hill ) {
			return new Color(0.05f, 0.9f, 0.15f);
		} else if ( normalizedH >= sand ) {
			return new Color(1f, 0.95f, 0.5f);
		} else {
			return new Color(0f, 0.1f, 0.9f);
		}
	}
}
