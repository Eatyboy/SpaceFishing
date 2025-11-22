using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Parallax Settings")]
    public Transform player;
    public float parallaxFactor = 0.5f; // 0 = no movement, 1 = moves with player
    
    private Vector3 lastPlayerPosition;

    void Start()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        if(player != null)
        {
            lastPlayerPosition = player.position;
        }
    }

    void LateUpdate()
    {
        if(player == null) return;
        
        Vector3 deltaMovement = player.position - lastPlayerPosition;
        transform.position += deltaMovement * parallaxFactor;
        
        lastPlayerPosition = player.position;
    }
}