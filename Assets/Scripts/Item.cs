using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Properties")]
    public float scale = 1f;
    public float weight = 1f;
    public int value = 10;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        transform.localScale = Vector3.one * scale;
    }

    public void Initialize(float newScale, float newWeight, int newValue)
    {
        scale = newScale;
        weight = newWeight;
        value = newValue;

        transform.localScale = Vector3.one * scale;

        if (rb != null)
            rb.mass = weight;
    }

    public void InitializeMovement(Vector2 velocity, float angularVelocity)
    {
        if (rb != null)
        {
            rb.linearVelocity = velocity;
            rb.angularVelocity = angularVelocity;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Collect();
        }
    }

    // penis
    public void Collect()
    {
        if (Player.instance != null)
        {
            Player.instance.money += value;
        }

        Destroy(gameObject);
    }
}