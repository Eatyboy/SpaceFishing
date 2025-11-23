using System.Collections.Generic;
using UnityEngine;

public class SpaceObject : MonoBehaviour
{
    public enum SoundType
    {
        None,
        Metal,
        Rock,
        Cow
    }

    [Header("Properties")]
    public Rigidbody2D rb;
    public float mass;
    public float scale;
    public SoundType soundType = SoundType.None;
    //from luna
    public SpriteRenderer spriteRenderer;
    public ParticleSystem particlePrefab;

    [Header("Settings")]

    public Sprite[] spriteVariations;


    [Header("Item ranges")]
    public Vector2 massRange = new(0.5f, 2f);
    public Vector2 scaleRange = new(0.1f, 5f);
    public bool isCollectable = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void InitializeMovement(Vector2 velocity, float angularVelocity)
    {
        if (rb != null)
        {
            rb.linearVelocity = velocity;
            rb.angularVelocity = angularVelocity;
        }
    }

    public void Initialize(float newScale, float newMass)
    {
        scale = newScale;
        mass = newMass;

        transform.localScale = Vector3.one * scale;

        if (rb != null) rb.mass = mass;

        //random sprites varitions
        if (spriteVariations != null && spriteVariations.Length > 0 && spriteRenderer != null)
        {
        spriteRenderer.sprite = spriteVariations[Random.Range(0, spriteVariations.Length)];
        }
    }

    HashSet<ParticleSystem> particleSpawners = new();
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Hook"))
        {
            particleSpawners.Add(Instantiate(particlePrefab, transform.position, Quaternion.FromToRotation(Vector3.up, collision.contacts[0].normal)));
        }
    }

    private void OnDestroy()
    {
        foreach(var spawner in particleSpawners)
        {
            Destroy(spawner.gameObject);
        }
        particleSpawners.Clear();
    }
}
