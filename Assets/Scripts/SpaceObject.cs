using UnityEngine;

public class SpaceObject : MonoBehaviour
{
    public float mass;
    public Rigidbody2D rb;

    private void Awake()
    {
        rb.mass = mass;
    }
}
