using UnityEngine;

public class ItemBounce : MonoBehaviour
{
    public float squashAmount = 0.5f;
    public float bounceSpeed = 20f;
    public float cooldown = 3f;
    
    private Vector3 originalScale;
    private float bounceTime = 0f;
    private float cooldownTimer = 0f;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if(cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
        
        if(bounceTime > 0f)
        {
            bounceTime -= Time.deltaTime * bounceSpeed;
            float t = bounceTime;
            
            // Squash it YEEAHHHHHHH
            float scaleX = 1f + Mathf.Sin(t * Mathf.PI * 2) * squashAmount;
            float scaleY = 1f - Mathf.Sin(t * Mathf.PI * 2) * squashAmount;
            
            transform.localScale = new Vector3(
                originalScale.x * scaleX,
                originalScale.y * scaleY,
                originalScale.z
            );
        }
        else
        {
            transform.localScale = originalScale;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if((collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Hook")) && cooldownTimer <= 0f)
        {
            bounceTime = 1f;
            cooldownTimer = cooldown;
        }
    }
}