using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{

    public enum GenerationType
    {
        RANDOM, PERLINNOISE
    }
    public GenerationType generationType;
    public int mapWidth;
    public int mapHeigth;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunary;
    public bool autoUpdate;
    public int seed;
    public Vector2 offset;
    public Tilemap tilemap;

    public TerrainType[] regions;
    public TerrainType[] regionsMinerals;

    public void GenerateMap()
    {
        if (generationType == GenerationType.PERLINNOISE)
        {
            GenerateMapWithNoise();
        }
        else if (generationType == GenerationType.RANDOM)
        {
            GenerateMapWithRandom();
        }
    }

    private void GenerateMapWithRandom()
    {
        TileBase[] customTileMap = new TileBase[mapWidth * mapHeigth];
        for (int x = 0; x < mapHeigth; x++)
        {
            for (int y = 0; y < mapWidth; y++)
            {
                float rnd = UnityEngine.Random.Range(0f, 1f);
                customTileMap[y * mapWidth + x] = findTileFromRegion(rnd);
            }
        }
        SetTileMap(customTileMap);
        }

    private void SetTileMap(TileBase[] customTileMap)
    {
        for (int y = 0; y < mapHeigth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), customTileMap[y * mapWidth + x]);
            }
        }   
    }

    private TileBase findTileFromRegion(float rnd)
    {
        for (int i = 0; i < regions.Length; i++)
        {
            if (rnd <= regions[i].height)
            {
                return regions[i].tile;
            }
        }
        return regions[0].tile;
    }
    private void GenerateMapWithNoise()
    {
        float[,] noiseMapGround = Noise.GenerateNoiseMap(mapWidth, mapHeigth, seed, noiseScale, octaves, persistance, lacunary, offset);
        float[,] noiseMapGisement = Noise.GenerateNoiseMap(mapWidth, mapHeigth, seed+1, noiseScale, octaves, persistance, lacunary, offset);
        float[,] noiseMapContenuGisement = Noise.GenerateNoiseMap(mapWidth, mapHeigth, seed + 2, noiseScale, octaves, persistance, lacunary, offset);
        TileBase[] customTileMap = new TileBase[mapWidth * mapHeigth];
        for (int x = 0; x < mapHeigth; x++)
        {
            for (int y = 0; y < mapWidth; y++)
            {
                if (noiseMapGisement[x,y] >= 0.8f)
                {
                    float rnd2 = noiseMapContenuGisement[x, y];
                    customTileMap[y * mapWidth + x] = FindTileForGisement(rnd2);
                }
                else
                {
                float rnd = noiseMapGround[x, y];  
                customTileMap[y * mapWidth + x] = findTileFromRegion(rnd);
                }               
            }
        }
        SetTileMap(customTileMap);
    }

    private TileBase FindTileForGisement(float rnd)
    {
        for (int i = 0; i < regionsMinerals.Length; i++)
        {
            if (rnd <= regionsMinerals[i].height)
            {
                return regionsMinerals[i].tile;
            }
        }
        return regionsMinerals[0].tile;
    }

    public void OnValidate()
    {
        if (mapHeigth < 1)
        {
            mapHeigth = 1;
        }
        if (mapWidth < 1)
        {
            mapWidth = 1;
        }
        if (lacunary < 1)
        {
            lacunary = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
    }
}


[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public TileBase tile;
}
