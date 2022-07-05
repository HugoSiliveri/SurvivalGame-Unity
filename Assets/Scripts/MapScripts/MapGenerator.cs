using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour {

	public enum DrawMode {NoiseMap, ColourMap, TileMap};
	public DrawMode drawMode;

	public const int mapChunkSize = 241;
	public float noiseScale;

	public int octaves;
    [Range(0,1)]
	public float persistance;
	public float lacunarity;

	public int seed;
	public Vector2 offset;

	public bool autoUpdate;

	public TerrainType[] regions;
	TileMapGenerator tileMapGenerator;

	float[,] falloffMap;

    private void Awake()
    {
		falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }

    private void Start()
    {
		GenerateMap();
    }

    public float[,] GenerateMap() {

		float[,] noiseMap = Noise.GenerateNoiseMap (mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

		Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
		for (int y = 0; y < mapChunkSize; y++)
        {
			for (int x = 0; x < mapChunkSize; x++)
            {
				noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);

				float currentHeight = noiseMap[x, y];
				for (int i = 0; i < regions.Length; i++)
                {
					if (currentHeight <= regions[i].height)
                    {
						colourMap[y * mapChunkSize + x] = regions[i].colour;
						break;
                    }
                }
            }
        }

		MapDisplay display = FindObjectOfType<MapDisplay>();

		if (drawMode == DrawMode.NoiseMap)
        {
			display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
		}
		else if (drawMode == DrawMode.ColourMap)
        {
			display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        }
		else if (drawMode == DrawMode.TileMap)
        {
			tileMapGenerator = FindObjectOfType<TileMapGenerator>();
			tileMapGenerator.GenerateTileMap(noiseMap, regions);
        }

		return noiseMap;
		
	}

    private void OnValidate()
    {
		if (lacunarity < 1)
        {
			lacunarity = 1;
        }
		if (octaves < 0)
        {
			octaves = 0;
        }

		falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);
    }
}

[System.Serializable]
public struct TerrainType
{
	public string name;
	public float height;
	public Color colour;
}
