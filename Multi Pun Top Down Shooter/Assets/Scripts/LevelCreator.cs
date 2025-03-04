using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCreator : MonoBehaviour
{
    [Header("Level Settings")]
    public int width = 50;
    public int length = 50;
    public int maxHeight = 3;
    public float roomSize = 1f;

    [Header("Prefabs")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject ceilingPrefab;
    public GameObject[] obstaclePrefabs;

    [Header("Generation Settings")]
    [Range(0, 100)]
    public int obstaclePercentage = 20;
    public bool generateCeiling = true;

    private void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        // Crear contenedor para el nivel
        GameObject levelContainer = new GameObject("GeneratedLevel");
        levelContainer.transform.parent = transform;

        // Generar suelo base
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                Vector3 position = new Vector3(x * roomSize, 0, z * roomSize);
                GameObject floor = Instantiate(floorPrefab, position, Quaternion.identity);
                floor.transform.parent = levelContainer.transform;

                // Generar obstáculos aleatoriamente
                if (Random.Range(0, 100) < obstaclePercentage)
                {
                    float height = Random.Range(1, maxHeight);
                    for (int y = 1; y <= height; y++)
                    {
                        Vector3 obstaclePos = new Vector3(x * roomSize, y * roomSize, z * roomSize);
                        GameObject obstacle = Instantiate(
                            obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)],
                            obstaclePos,
                            Quaternion.identity
                        );
                        obstacle.transform.parent = levelContainer.transform;
                    }
                }
            }
        }

        // Generar paredes exteriores
        GenerateWalls(levelContainer);

        // Generar techo si está activado
        if (generateCeiling)
        {
            GenerateCeiling(levelContainer);
        }
    }

    void GenerateWalls(GameObject container)
    {
        // Paredes en X
        for (int x = -1; x <= width; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                Vector3 pos1 = new Vector3(x * roomSize, y * roomSize, -1 * roomSize);
                Vector3 pos2 = new Vector3(x * roomSize, y * roomSize, length * roomSize);
                
                Instantiate(wallPrefab, pos1, Quaternion.identity).transform.parent = container.transform;
                Instantiate(wallPrefab, pos2, Quaternion.identity).transform.parent = container.transform;
            }
        }

        // Paredes en Z
        for (int z = -1; z <= length; z++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                Vector3 pos1 = new Vector3(-1 * roomSize, y * roomSize, z * roomSize);
                Vector3 pos2 = new Vector3(width * roomSize, y * roomSize, z * roomSize);
                
                Instantiate(wallPrefab, pos1, Quaternion.identity).transform.parent = container.transform;
                Instantiate(wallPrefab, pos2, Quaternion.identity).transform.parent = container.transform;
            }
        }
    }

    void GenerateCeiling(GameObject container)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                Vector3 position = new Vector3(x * roomSize, maxHeight * roomSize, z * roomSize);
                GameObject ceiling = Instantiate(ceilingPrefab, position, Quaternion.identity);
                ceiling.transform.parent = container.transform;
            }
        }
    }
}
