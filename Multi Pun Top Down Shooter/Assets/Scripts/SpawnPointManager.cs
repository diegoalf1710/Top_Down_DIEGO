using UnityEngine;

public class SpawnPointManager : MonoBehaviour
{
    private static SpawnPointManager instance;
    public static SpawnPointManager Instance { get { return instance; } }

    public Transform[] spawnPoints;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Vector3 GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No hay spawn points configurados!");
            return Vector3.zero;
        }
        
        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex].position;
    }
}
