using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class generateTerrain : MonoBehaviour
{
    public Sprite[] trees;
    public Sprite[] mountains;
    public Tilemap tilemap;

    private static int seedOrigin = 0;

    void Start()
    {
        seedOrigin = Random.Range(1, 10000);
        populateTiles();
    }

    void populateTiles()
    {
        for (int x = 0; x < 89; x++)
        {
            for (int y = 0; y < 89; y++)
            {
                float noise = perlinNoise(x, y, 1.0f, 1.0f, 0.2f);
                if (noise < 0.3f)
                {
                    Vector3Int p = new Vector3Int(x, y, 0);
                    Tile tile = ScriptableObject.CreateInstance<Tile>();
                    tile.sprite = trees[Random.Range(0, trees.Length)];
                    tilemap.SetTile(p, tile);
                }
                else if (noise > 0.8f)
                {
                    Vector3Int p = new Vector3Int(x, y, 0);
                    Tile tile = ScriptableObject.CreateInstance<Tile>();
                    tile.sprite = mountains[Random.Range(0, mountains.Length)];
                    tilemap.SetTile(p, tile);
                }
            }
        }
    }

    /// scale : The scale of the "perlin noise" view
    /// heightMultiplier : The maximum height of the terrain
    /// persistance : The higher it is, the rougher the terrain will be (this value should be between 0 and 1 excluded)
    public static float perlinNoise(int x, int y, float scale, float heightMultiplier, float persistance)
    {
        float PerlinValue = 0f;
        float amplitude = 1f;

        // Get the perlin value at that octave and add it to the sum
        PerlinValue += Mathf.PerlinNoise( (x + seedOrigin) * 0.1f, (y + seedOrigin) * 0.1f) * amplitude;

        // Decrease the amplitude and the frequency
        amplitude *= persistance;

        // Return the noise value
        return PerlinValue * heightMultiplier;
    }
}
