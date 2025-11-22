using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemSpawnData
{
    public GameObject prefab;
    [Tooltip("Higher weight = more likely to spawn")]
    public float spawnWeight = 1f;
}

public class ItemSpawner : MonoBehaviour
{
    [Header("References")]
    public ItemSpawnData[] itemSpawnData;
    public Transform player;

    [Header("Spawn Settings")]
    public float spawnRadius = 20f;
    public float despawnRadius = 30f;
    public int itemsPerChunk = 5;
    public float chunkSize = 15f;

    [Header("Item ranges")]
    public Vector2 scaleRange = new Vector2(0.5f, 2f);
    public Vector2 weightRange = new Vector2(0.1f, 5f);
    public Vector2 valueRange = new Vector2(0f, 100f);

    [Header("Initial Movement Settings")]
    public Vector2 initialVelocityRange = new Vector2(0.1f, 0.5f);
    public Vector2 initialAngularVelocityRange = new Vector2(-10f, 10f);

    // Track Spawned Items and Chunks
    private Dictionary<Vector2Int, List<GameObject>> spawnedChunks = new Dictionary<Vector2Int, List<GameObject>>();
    private HashSet<Vector2Int> activeChunkCoords = new HashSet<Vector2Int>();
    private float totalSpawnWeight;

    void Start()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            if(player == null)
                Debug.LogError("Player not found in the scene");
        }

        CalculateTotalSpawnWeight();

        if(player != null)
            UpdateSpawnedChunks();
    }

    void Update()
    {
        if (player == null) return;
        UpdateSpawnedChunks();
    }

    void CalculateTotalSpawnWeight()
    {
        totalSpawnWeight = 0f;
        if (itemSpawnData != null)
        {
            foreach (var data in itemSpawnData)
            {
                totalSpawnWeight += data.spawnWeight;
            }
        }
    }

    GameObject GetWeightedRandomPrefab()
    {
        if (itemSpawnData == null || itemSpawnData.Length == 0)
        {
            Debug.LogError("No item spawn data assigned!");
            return null;
        }

        float randomValue = Random.Range(0f, totalSpawnWeight);
        float cumulativeWeight = 0f;

        foreach (var data in itemSpawnData)
        {
            cumulativeWeight += data.spawnWeight;
            if (randomValue <= cumulativeWeight)
            {
                return data.prefab;
            }
        }

        // Fallback lol
        return itemSpawnData[0].prefab;
    }

    void UpdateSpawnedChunks()
    {
        Vector2Int currChunk = GetChunkCoord(player.position);

        HashSet<Vector2Int> chunksToLoad = new HashSet<Vector2Int>();
        int chunkRadius = Mathf.CeilToInt(spawnRadius / chunkSize);

        for(int x = -chunkRadius; x <= chunkRadius; x++)
        {
            for(int y = -chunkRadius; y <= chunkRadius; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(currChunk.x + x, currChunk.y + y);
                chunksToLoad.Add(chunkCoord);
            }
        }

        foreach(var chunk in chunksToLoad)
        {
            if(!activeChunkCoords.Contains(chunk))
            {
                SpawnChunk(chunk);
                activeChunkCoords.Add(chunk);
            }
        }

        List<Vector2Int> chunksToUnload = new List<Vector2Int>();
        foreach(var chunk in activeChunkCoords)
        {
            if(!chunksToLoad.Contains(chunk))
            {
                DespawnChunk(chunk);
                chunksToUnload.Add(chunk);
            }
        }

        foreach(var chunk in chunksToUnload)
        {
            activeChunkCoords.Remove(chunk);
        }
    }

    private void SpawnChunk(Vector2Int chunkCoord)
    {
        List<GameObject> chunkItems = new List<GameObject>();
        Vector2 chunkOrigin = new Vector2(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize);

        int seed = GetChunkSeed(chunkCoord);
        Random.InitState(seed);

        for(int i = 0; i < itemsPerChunk; i++)
        {
            Vector2 randomOff = new Vector2(
              Random.Range(-chunkSize / 2, chunkSize / 2),
              Random.Range(-chunkSize / 2, chunkSize / 2)  
            );
            Vector2 spawnPos = chunkOrigin + randomOff;

            float scale = Random.Range(scaleRange.x, scaleRange.y);
            float weight = Random.Range(weightRange.x, weightRange.y);
            int value = (int) Random.Range(valueRange.x, valueRange.y + 1);

            GameObject prefabToSpawn = GetWeightedRandomPrefab();
            if (prefabToSpawn == null)
            {
                Debug.LogError("Failed to get prefab to spawn!");
                continue;
            }

            GameObject newItem = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

            Item itemComp = newItem.GetComponent<Item>();
            if(itemComp != null)
            {
                itemComp.Initialize(scale, weight, value);
                
                Vector2 randomVelocity = Random.insideUnitCircle.normalized * Random.Range(initialVelocityRange.x, initialVelocityRange.y);
                float randomAngularVelocity = Random.Range(initialAngularVelocityRange.x, initialAngularVelocityRange.y);
                itemComp.InitializeMovement(randomVelocity, randomAngularVelocity);
            }
            else
            {
                SpaceObject spaceObj = newItem.GetComponent<SpaceObject>();
                if (spaceObj != null)
                {
                    Vector2 randomVelocity = Random.insideUnitCircle.normalized * Random.Range(initialVelocityRange.x, initialVelocityRange.y);
                    float randomAngularVelocity = Random.Range(initialAngularVelocityRange.x, initialAngularVelocityRange.y);
                    spaceObj.InitializeMovement(randomVelocity, randomAngularVelocity);
                }
                else
                {
                    Debug.LogWarning("Spawned object has neither Item nor SpaceObject component");
                }
            }

            chunkItems.Add(newItem);
        }

        spawnedChunks[chunkCoord] = chunkItems;
        
        Random.InitState(System.Environment.TickCount);
    }

    private void DespawnChunk(Vector2Int chunkCoord)
    {
        if(spawnedChunks.ContainsKey(chunkCoord))
        {
            foreach(var item in spawnedChunks[chunkCoord])
            {
                if(item != null)
                    Destroy(item);
            }
            spawnedChunks.Remove(chunkCoord);
        }
    }

    private Vector2Int GetChunkCoord(Vector2 position)
    {
        int x = Mathf.FloorToInt(position.x / chunkSize);
        int y = Mathf.FloorToInt(position.y / chunkSize);
        return new Vector2Int(x, y);
    }

    private int GetChunkSeed(Vector2Int chunkCoord)
    {
        return chunkCoord.x * 73856093 ^ chunkCoord.y * 19349663;
    }

    private void OnDrawGizmosSelected()
    {
        if (player == null) return;

        // Spawn radius
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.position, spawnRadius);

        // Despawn radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, despawnRadius);

        // Draw chunk grid
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Vector2Int currentChunk = GetChunkCoord(player.position);
        int chunkRadius = Mathf.CeilToInt(spawnRadius / chunkSize);

        for (int x = -chunkRadius; x <= chunkRadius; x++)
        {
            for (int y = -chunkRadius; y <= chunkRadius; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(currentChunk.x + x, currentChunk.y + y);
                Vector2 chunkCenter = new Vector2(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize);
                
                Gizmos.DrawWireCube(new Vector3(chunkCenter.x, chunkCenter.y, 0), new Vector3(chunkSize, chunkSize, 0));
            }
        }
    }
}