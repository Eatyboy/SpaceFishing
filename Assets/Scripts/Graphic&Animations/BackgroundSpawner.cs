using UnityEngine;
using System.Collections.Generic;

public class BackgroundSpawner : MonoBehaviour
{
    [Header("References")]
    public Sprite[] backgroundSprites;
    public float[] spawnProbabilities;
    public Transform player;

    [Header("Spawn Settings")]
    public float spawnRadius = 25f;
    public float despawnRadius = 30f;
    public int objectsPerChunk = 10;
    public float chunkSize = 15f;
    public Vector2 scaleRange = new Vector2(0.8f, 1.5f);
    public float minDistance = 1f;

    private Dictionary<Vector2Int, List<GameObject>> spawnedChunks = new Dictionary<Vector2Int, List<GameObject>>();
    private HashSet<Vector2Int> activeChunks = new HashSet<Vector2Int>();
    private Vector2Int lastPlayerChunk;
    
    private float totalProbability;
    private List<Vector2Int> chunksToRemove = new List<Vector2Int>();
    private List<Vector2> spawnedPositions = new List<Vector2>();

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            if (player == null)
                Debug.LogError("Player not found in the scene");
        }

        if (spawnProbabilities == null || spawnProbabilities.Length != backgroundSprites.Length)
        {
            spawnProbabilities = new float[backgroundSprites.Length];
            for (int i = 0; i < spawnProbabilities.Length; i++)
                spawnProbabilities[i] = 1f;
        }

        CalculateTotalProbability();

        if (player != null)
        {
            lastPlayerChunk = GetChunkCoord(player.position);
            UpdateChunks();
        }
    }

    void Update()
    {
        if (player == null) return;

        Vector2Int currentChunk = GetChunkCoord(player.position);
        
        if (currentChunk != lastPlayerChunk)
        {
            lastPlayerChunk = currentChunk;
            UpdateChunks();
        }
    }

    void CalculateTotalProbability()
    {
        totalProbability = 0f;
        foreach (float prob in spawnProbabilities)
            totalProbability += prob;
    }

    void UpdateChunks()
    {
        int spawnRadiusChunks = Mathf.CeilToInt(spawnRadius / chunkSize);
        float despawnRadiusSqr = despawnRadius * despawnRadius;

        HashSet<Vector2Int> chunksToLoad = new HashSet<Vector2Int>();

        // Load nearby chunks
        for (int x = -spawnRadiusChunks; x <= spawnRadiusChunks; x++)
        {
            for (int y = -spawnRadiusChunks; y <= spawnRadiusChunks; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(lastPlayerChunk.x + x, lastPlayerChunk.y + y);
                chunksToLoad.Add(chunkCoord);

                if (!activeChunks.Contains(chunkCoord))
                {
                    SpawnChunk(chunkCoord);
                    activeChunks.Add(chunkCoord);
                }
            }
        }

        // Despawn distant chunks
        chunksToRemove.Clear();
        Vector2 playerPos = player.position;

        foreach (var chunk in activeChunks)
        {
            if (!chunksToLoad.Contains(chunk))
            {
                Vector2 chunkCenter = new Vector2(chunk.x * chunkSize, chunk.y * chunkSize);
                float distanceSqr = (chunkCenter - playerPos).sqrMagnitude;

                if (distanceSqr > despawnRadiusSqr)
                {
                    DespawnChunk(chunk);
                    chunksToRemove.Add(chunk);
                }
            }
        }

        for (int i = 0; i < chunksToRemove.Count; i++)
            activeChunks.Remove(chunksToRemove[i]);
    }

    void SpawnChunk(Vector2Int chunkCoord)
    {
        int seed = GetChunkSeed(chunkCoord);
        Random.InitState(seed);

        List<GameObject> objects = new List<GameObject>(objectsPerChunk);
        spawnedPositions.Clear();
        Vector2 chunkOrigin = new Vector2(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize);

        int attempts = 0;
        int maxAttempts = objectsPerChunk * 10;
        float minDistanceSqr = minDistance * minDistance;

        while (objects.Count < objectsPerChunk && attempts < maxAttempts)
        {
            attempts++;

            Vector2 randomOffset = new Vector2(
                Random.Range(-chunkSize / 2, chunkSize / 2),
                Random.Range(-chunkSize / 2, chunkSize / 2)
            );
            Vector2 pos = chunkOrigin + randomOffset;

            // Check for overlap
            bool overlaps = false;
            for (int i = 0; i < spawnedPositions.Count; i++)
            {
                if ((pos - spawnedPositions[i]).sqrMagnitude < minDistanceSqr)
                {
                    overlaps = true;
                    break;
                }
            }

            if (overlaps) continue;

            GameObject obj = new GameObject("BG");
            obj.transform.SetParent(transform, false);
            obj.transform.position = pos;
            obj.transform.localScale = Vector3.one * Random.Range(scaleRange.x, scaleRange.y);

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = SelectSprite();
            sr.sortingOrder = -10;

            objects.Add(obj);
            spawnedPositions.Add(pos);
        }

        spawnedChunks[chunkCoord] = objects;
        
        Random.InitState(System.Environment.TickCount);
    }

    Sprite SelectSprite()
    {
        if (backgroundSprites == null || backgroundSprites.Length == 0)
        {
            Debug.LogError("No background sprites assigned!");
            return null;
        }

        float rand = Random.Range(0f, totalProbability);
        float cumulative = 0f;

        for (int i = 0; i < backgroundSprites.Length; i++)
        {
            cumulative += spawnProbabilities[i];
            if (rand <= cumulative)
                return backgroundSprites[i];
        }

        return backgroundSprites[0];
    }

    void DespawnChunk(Vector2Int chunkCoord)
    {
        if (spawnedChunks.TryGetValue(chunkCoord, out List<GameObject> objects))
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] != null)
                    Destroy(objects[i]);
            }
            spawnedChunks.Remove(chunkCoord);
        }
    }

    Vector2Int GetChunkCoord(Vector2 position)
    {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / chunkSize),
            Mathf.FloorToInt(position.y / chunkSize)
        );
    }

    int GetChunkSeed(Vector2Int chunkCoord)
    {
        return chunkCoord.x * 73856093 ^ chunkCoord.y * 19349663;
    }

    void OnDrawGizmosSelected()
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