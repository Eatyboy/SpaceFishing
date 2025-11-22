using UnityEngine;

public class SpaceObject : MonoBehaviour
{
    public float mass;
    public Rigidbody2D rb;

    private void Awake()
    {
        rb.mass = mass;
    }

    public void InitializeMovement(Vector2 velocity, float angularVelocity)
    {
        if (rb != null)
        {
            rb.linearVelocity = velocity;
            rb.angularVelocity = angularVelocity;
        }
    }

}