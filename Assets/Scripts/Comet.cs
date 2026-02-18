using UnityEngine;

public class Comet : MonoBehaviour
{
    public float speed = 5.0f;
    public Vector2 speedRange = new(3f, 10f);

    private Rigidbody2D rb;
    private HazardousItem hazardousItem;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hazardousItem = GetComponent<HazardousItem>();
    }

    public void Initialize(float newDamage, float newSpeed, Vector2 direction)
    {
        speed = newSpeed;

        if (hazardousItem != null)
        {
            hazardousItem.Initialize(newDamage);
        }

        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}