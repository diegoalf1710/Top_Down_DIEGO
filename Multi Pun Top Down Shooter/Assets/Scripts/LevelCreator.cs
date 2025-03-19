using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation; // Añadir este using para NavMeshSurface

public class LevelCreator : MonoBehaviour
{
    [Header("Level Settings")]
    public int width = 50; // Ancho del nivel
    public int length = 50; // Largo del nivel
    public int maxHeight = 3; // Altura máxima de los obstáculos
    public float roomSize = 1f; // Tamaño de cada celda del nivel

    [Header("Prefabs")]
    public GameObject floorPrefab; // Prefab del suelo
    public GameObject wallPrefab; // Prefab de las paredes
    public GameObject ceilingPrefab; // Prefab del techo
    public GameObject[] obstaclePrefabs; // Prefabs de los obstáculos

    [Header("Generation Settings")]
    [Range(0, 100)]
    public int obstaclePercentage = 20; // Porcentaje de generación de obstáculos
    public bool generateCeiling = true; // Indica si se debe generar el techo

    [Header("Items Settings")]
    [Range(0, 100)]
    public int itemSpawnPercentage = 10; // Porcentaje de generación de ítems
    public GameObject[] itemPrefabs; // Prefabs de los ítems
    public float minItemSpacing = 3f; // Espacio mínimo entre ítems

    [Header("Enemy Settings")]
    [Range(0, 100)]
    public int enemySpawnPercentage = 15; // Porcentaje de generación de enemigos
    public GameObject[] enemyPrefabs; // Prefabs de los enemigos
    public float minEnemySpacing = 5f; // Espacio mínimo entre enemigos

    [Header("NavMesh Settings")]
    public NavMeshSurface navMeshSurface;
    public float navMeshBakeDelay = 0.5f;

    [Header("Terrain Settings")]
    [Range(0, 100)]
    public int stonePercentage = 30;
    // Altura base del suelo (siempre habrá esta cantidad mínima de bloques)
    public int baseGroundHeight = 1;
    // Profundidad de la capa de tierra desde la superficie
    public int dirtLayerDepth = 3;
    // Altura máxima del terreno desde el nivel base
    public int maxTerrainHeight = 8;
    public GameObject stonePrefab;
    public GameObject dirtPrefab;
    public float terrainHeightScale = 1f;
    public float noiseScale = 0.1f;

    private List<Vector2> occupiedPositions = new List<Vector2>(); // Lista de posiciones ocupadas

    // Sistema de grid para rastrear objetos
    private Dictionary<Vector2Int, List<GameObject>> gridObjects = new Dictionary<Vector2Int, List<GameObject>>();
    private Dictionary<Vector2Int, BlockType> gridBlockTypes = new Dictionary<Vector2Int, BlockType>();

    public enum BlockType
    {
        Empty, // Bloque vacío
        Floor, // Bloque de suelo
        Obstacle, // Bloque de obstáculo
        Wall, // Bloque de pared
        Item, // Bloque de ítem
        Enemy, // Bloque de enemigo
        Stone,
        Dirt
    }

    private void Start()
    {
        GenerateLevel(); // Generar el nivel al iniciar
    }

    void GenerateLevel()
    {
        occupiedPositions.Clear();
        gridObjects.Clear();
        gridBlockTypes.Clear();
        GameObject levelContainer = new GameObject("GeneratedLevel");
        levelContainer.transform.parent = transform;

        // Generar terreno con ruido Perlin
        float[,] heightMap = GenerateHeightMap();

        // Generar capas del terreno
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                int terrainHeight = Mathf.RoundToInt(heightMap[x,z] * terrainHeightScale);
                
                // Generar capas
                for (int y = 0; y <= terrainHeight; y++)
                {
                    GenerateBlock(x, y, z, terrainHeight, levelContainer);
                }

                // Generar recursos y elementos adicionales solo en la superficie
                if (Random.Range(0, 100) < itemSpawnPercentage && IsPositionValid(x, z, minItemSpacing))
                {
                    Vector3 itemPos = new Vector3(x * roomSize, (terrainHeight + 1) * roomSize, z * roomSize);
                    SpawnItem(itemPos, levelContainer);
                }
                
                if (Random.Range(0, 100) < enemySpawnPercentage && IsPositionValid(x, z, minEnemySpacing))
                {
                    Vector3 enemyPos = new Vector3(x * roomSize, (terrainHeight + 1) * roomSize, z * roomSize);
                    SpawnEnemy(enemyPos, levelContainer);
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

        // Añadir NavMeshSurface si no existe
        if (navMeshSurface == null)
        {
            navMeshSurface = levelContainer.AddComponent<NavMeshSurface>();
            navMeshSurface.collectObjects = CollectObjects.All;
            navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
        }

        // Iniciar la generación del NavMesh después de un pequeño delay
        StartCoroutine(BuildNavMeshDelayed());
    }

    IEnumerator BuildNavMeshDelayed()
    {
        // Esperar a que todos los objetos estén en su lugar
        yield return new WaitForSeconds(navMeshBakeDelay);
        
        // Construir el NavMesh
        navMeshSurface.BuildNavMesh();
        
        // Añadir NavMeshAgent a los enemigos
        SetupEnemyNavigation();
    }

    void SetupEnemyNavigation()
    {
        foreach (var gridPos in gridObjects)
        {
            foreach (var obj in gridPos.Value)
            {
                if (GetBlockTypeAt(gridPos.Key) == BlockType.Enemy)
                {
                    // Añadir AIController como componente
                    AIController aiController = obj.AddComponent<AIController>();
                }
            }
        }
    }

    private float[,] GenerateHeightMap()
    {
        float[,] heightMap = new float[width, length];
        float offsetX = Random.Range(0f, 1000f);
        float offsetZ = Random.Range(0f, 1000f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                float xCoord = (float)x * noiseScale + offsetX;
                float zCoord = (float)z * noiseScale + offsetZ;
                // Ajusta el ruido para que esté entre baseGroundHeight y maxTerrainHeight
                float noise = Mathf.PerlinNoise(xCoord, zCoord);
                heightMap[x,z] = Mathf.Lerp(baseGroundHeight, maxTerrainHeight, noise);
            }
        }

        return heightMap;
    }

    private void GenerateBlock(int x, int y, int z, int surfaceHeight, GameObject container)
    {
        Vector3 position = new Vector3(x * roomSize, y * roomSize, z * roomSize);
        GameObject block;

        // Calcula la profundidad desde la superficie
        int depthFromSurface = surfaceHeight - y;

        if (y == 0)
        {
            // Capa base (bedrock)
            block = Instantiate(floorPrefab, position, Quaternion.identity);
            AddToGrid(new Vector2Int(x, z), block, BlockType.Floor);
        }
        else if (depthFromSurface <= dirtLayerDepth && y < surfaceHeight)
        {
            // Capa de tierra (desde la superficie hacia abajo)
            block = Instantiate(dirtPrefab, position, Quaternion.identity);
            AddToGrid(new Vector2Int(x, z), block, BlockType.Dirt);
        }
        else
        {
            // Piedra (todo lo demás)
            block = Instantiate(stonePrefab, position, Quaternion.identity);
            AddToGrid(new Vector2Int(x, z), block, BlockType.Stone);
        }
        
        block.transform.parent = container.transform;
    }

    private void SpawnItem(Vector3 position, GameObject container)
    {
        GameObject item = Instantiate(
            itemPrefabs[Random.Range(0, itemPrefabs.Length)],
            position,
            Quaternion.identity
        );
        item.transform.parent = container.transform;
        AddToGrid(new Vector2Int((int)(position.x/roomSize), (int)(position.z/roomSize)), item, BlockType.Item);
    }

    private void SpawnEnemy(Vector3 position, GameObject container)
    {
        GameObject enemy = Instantiate(
            enemyPrefabs[Random.Range(0, enemyPrefabs.Length)],
            position,
            Quaternion.identity
        );
        enemy.transform.parent = container.transform;
        AddToGrid(new Vector2Int((int)(position.x/roomSize), (int)(position.z/roomSize)), enemy, BlockType.Enemy);
    }

    private void GenerateObstacle(int x, int z, GameObject container)
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
            obstacle.transform.parent = container.transform;
            AddToGrid(new Vector2Int(x, z), obstacle, BlockType.Obstacle);
        }
    }

    private bool TrySpawnItem(int x, int z, GameObject container)
    {
        // Verificar si hay espacio suficiente
        if (IsPositionValid(x, z, minItemSpacing))
        {
            Vector3 itemPosition = new Vector3(x * roomSize, roomSize * 1.8f, z * roomSize);
            GameObject item = Instantiate(
                itemPrefabs[Random.Range(0, itemPrefabs.Length)],
                itemPosition,
                Quaternion.identity
            );
            item.transform.parent = container.transform;
            occupiedPositions.Add(new Vector2(x, z));
            AddToGrid(new Vector2Int(x, z), item, BlockType.Item);
            return true;
        }
        return false;
    }

    private bool TrySpawnEnemy(int x, int z, GameObject container)
    {
        if (IsPositionValid(x, z, minEnemySpacing))
        {
            // Ajustamos la altura a 0.1f para que esté justo sobre el suelo
            Vector3 enemyPosition = new Vector3(x * roomSize, 1.5f, z * roomSize);
            GameObject enemy = Instantiate(
                enemyPrefabs[Random.Range(0, enemyPrefabs.Length)],
                enemyPosition,
                Quaternion.identity
            );
            enemy.transform.parent = container.transform;
            occupiedPositions.Add(new Vector2(x, z));
            AddToGrid(new Vector2Int(x, z), enemy, BlockType.Enemy);
            return true;
        }
        return false;
    }

    private void AddToGrid(Vector2Int gridPosition, GameObject obj, BlockType type)
    {
        if (!gridObjects.ContainsKey(gridPosition))
        {
            gridObjects[gridPosition] = new List<GameObject>();
        }
        gridObjects[gridPosition].Add(obj);
        gridBlockTypes[gridPosition] = type;
    }

    // Método para obtener objetos en una posición específica
    public List<GameObject> GetObjectsAt(Vector2Int position)
    {
        return gridObjects.ContainsKey(position) ? gridObjects[position] : new List<GameObject>();
    }

    // Método para obtener el tipo de bloque en una posición
    public BlockType GetBlockTypeAt(Vector2Int position)
    {
        return gridBlockTypes.ContainsKey(position) ? gridBlockTypes[position] : BlockType.Empty;
    }

    // Método para obtener objetos en un radio específico
    public Dictionary<Vector2Int, List<GameObject>> GetObjectsInRadius(Vector2Int center, int radius)
    {
        Dictionary<Vector2Int, List<GameObject>> result = new Dictionary<Vector2Int, List<GameObject>>();
        
        for (int x = -radius; x <= radius; x++)
        {
            for (int z = -radius; z <= radius; z++)
            {
                Vector2Int checkPos = new Vector2Int(center.x + x, center.y + z);
                if (gridObjects.ContainsKey(checkPos))
                {
                    result[checkPos] = gridObjects[checkPos];
                }
            }
        }
        
        return result;
    }

    private bool IsPositionValid(int x, int z, float minSpacing)
    {
        foreach (Vector2 pos in occupiedPositions)
        {
            float distance = Vector2.Distance(new Vector2(x, z), pos);
            if (distance < minSpacing)
            {
                return false;
            }
        }
        return true;
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
                AddToGrid(new Vector2Int(x, -1), wallPrefab, BlockType.Wall);
                AddToGrid(new Vector2Int(x, length), wallPrefab, BlockType.Wall);
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
                AddToGrid(new Vector2Int(-1, z), wallPrefab, BlockType.Wall);
                AddToGrid(new Vector2Int(width, z), wallPrefab, BlockType.Wall);
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
                AddToGrid(new Vector2Int(x, z), ceiling, BlockType.Empty);
            }
        }
    }
}
