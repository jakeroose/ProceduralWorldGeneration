/*
 * OpenSimplex (Simplectic) Noise Test for Unity (C#)
 * This file is in the Public Domain.
 *
 * This file is intended to test the functionality of OpenSimplexNoise.cs
 * Attach this script to a GameObject with mesh (eg a Quad prefab).
 * Texture is updated every frame to assist profiling for performance.
 * Using a RenderTexture should perform better, however using a Texture2D
 * as an example makes this compatible with the free version of Unity.
 * 
 */

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class NoiseTexture : MonoBehaviour {

    public int width = 512;
    public int height = 512;
    public float feature_size = 24.0f;
    Texture2D _texture;
    Color[] _colorArray;

	void Start () {
        _texture = new Texture2D(width, height);
        // Filling an array and using Texture2D.SetPixels is slightly faster 
        // than calling SetPixel many times
        _colorArray = new Color[width * height];
		GetComponent<Renderer>().material.mainTexture = _texture;
	}

    void UpdateTexture()
    {
        OpenSimplexNoise noise = new OpenSimplexNoise((int) (Time.timeSinceLevelLoad * Time.deltaTime * 1000));
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float value = (float)noise.eval((double) x / feature_size, (double) y / feature_size, 0.0);
                value = (value * 0.5f) + 0.5f;
                Color color = new Color(value, value, value);
                _colorArray[x + (y * width)] = color;
            }
        }
        _texture.SetPixels(_colorArray);
        _texture.Apply();
		GetComponent<Renderer>().material.mainTexture = _texture;
    }

    void Update () {
        UpdateTexture();
    }
}