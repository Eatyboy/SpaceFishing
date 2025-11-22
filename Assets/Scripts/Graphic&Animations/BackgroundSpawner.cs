using UnityEngine;
using System.Collections.Generic;

public class BackgroundSpawner : MonoBehaviour
{
    public Sprite[] backgroundSprites;
    public float[] spawnProbabilities;
    public Transform player;

    public float spawnRadius = 25f;
    public int objectsPerChunk = 10;
    public float chunkSize = 15f;
    public Vector2 scaleRange = new Vector2(0.8f, 1.5f);
    public float minDistance = 1f; // Minimum distance between objects

    private Dictionary<Vector2Int, List<GameObject>> spawnedChunks = new Dictionary<Vector2Int, List<GameObject>>();
    private HashSet<Vector2Int> activeChunks = new HashSet<Vector2Int>();

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (spawnProbabilities == null || spawnProbabilities.Length != backgroundSprites.Length)
        {
            spawnProbabilities = new float[backgroundSprites.Length];
            for (int i = 0; i < spawnProbabilities.Length; i++)
                spawnProbabilities[i] = 1f;
        }

        UpdateChunks();
    }

    void Update()
    {
        if (player != null)
            UpdateChunks();
    }

    void UpdateChunks()
    {
        // Use this transform's position (which moves with parallax)
        Vector2Int currentChunk = GetChunkCoord(transform.position);
        int radius = Mathf.CeilToInt(spawnRadius / chunkSize);

        HashSet<Vector2Int> chunksToKeep = new HashSet<Vector2Int>();

        // Load nearby chunks
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector2Int chunk = new Vector2Int(currentChunk.x + x, currentChunk.y + y);
                chunksToKeep.Add(chunk);

                if (!activeChunks.Contains(chunk))
                {
                    SpawnChunk(chunk);
                    activeChunks.Add(chunk);
                }
            }
        }

        // Unload distant chunks
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var chunk in activeChunks)
        {
            if (!chunksToKeep.Contains(chunk))
            {
                DespawnChunk(chunk);
                toRemove.Add(chunk);
            }
        }

        foreach (var chunk in toRemove)
            activeChunks.Remove(chunk);
    }

    void SpawnChunk(Vector2Int chunkCoord)
    {
        Random.InitState(chunkCoord.x * 73856093 ^ chunkCoord.y * 19349663);

        List<GameObject> objects = new List<GameObject>();
        List<Vector2> spawnedPositions = new List<Vector2>();
        Vector2 chunkOrigin = new Vector2(chunkCoord.x * chunkSize, chunkCoord.y * chunkSize);

        int attempts = 0;
        int maxAttempts = objectsPerChunk * 10;

        while (objects.Count < objectsPerChunk && attempts < maxAttempts)
        {
            attempts++;

            Vector2 pos = chunkOrigin + new Vector2(
                Random.Range(-chunkSize / 2, chunkSize / 2),
                Random.Range(-chunkSize / 2, chunkSize / 2)
            );

            // Check for overlap
            bool overlaps = false;
            foreach (var existingPos in spawnedPositions)
            {
                if (Vector2.Distance(pos, existingPos) < minDistance)
                {
                    overlaps = true;
                    break;
                }
            }

            if (overlaps) continue;

            GameObject obj = new GameObject("BG");
            obj.transform.position = pos;
            obj.transform.parent = transform;

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = SelectSprite();
            sr.sortingOrder = -10;

            float scale = Random.Range(scaleRange.x, scaleRange.y);
            obj.transform.localScale = Vector3.one * scale;

            objects.Add(obj);
            spawnedPositions.Add(pos);
        }

        spawnedChunks[chunkCoord] = objects;
    }

    Sprite SelectSprite()
    {
        float total = 0f;
        foreach (float prob in spawnProbabilities)
            total += prob;

        float rand = Random.Range(0f, total);
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
        if (spawnedChunks.ContainsKey(chunkCoord))
        {
            foreach (var obj in spawnedChunks[chunkCoord])
                if (obj != null) Destroy(obj);
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
}