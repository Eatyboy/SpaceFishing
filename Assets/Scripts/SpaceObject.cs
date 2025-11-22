using UnityEngine;

public class SpaceObject : MonoBehaviour
{
    [Header("Properties")]
    public Rigidbody2D rb;
    public float mass;
    public float scale;
    //from luna
    public SpriteRenderer spriteRenderer;

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
}
