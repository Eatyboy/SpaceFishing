using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player instance;

    private InputSystem_Actions ctrl;

    public Rigidbody2D rb;
    public Hook hook;

    public HashSet<BlackHole> activeBlackHoles = new();

    public float mass = 1.0f;
    public float rotationPower = 1.0f;

    public float hookRange = 1.0f;
    public float hookSpeed = 1.0f;
    public float hookPullForce = 1.0f;

    public float blackHoleGravity = 0.1f;
    public float blackHoleDamageFactor = 1.0f;
    public float blackHoleSoftness = 0.5f;

    public int money = 0;
    public float maxHp = 10.0f;
    public float hp = 10.0f;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        ctrl = new();
        rb.mass = mass;
        hp = maxHp;
    }

    private void Start()
    {
        AudioManager.Instance.PlayMusic(AudioManager.Instance.gameplayMusic);
    }

    private void OnEnable()
    {
        ctrl.Enable();
        ctrl.Player.Combo.performed += Fire;
    }

    private void OnDisable()
    {
        ctrl.Player.Combo.performed -= Fire;
        ctrl.Disable();
    }

    public void Update()
    {
        if (ctrl.Player.Button1.IsPressed())
        {
            rb.SetRotation(rb.rotation + rotationPower * Time.deltaTime);
        } else if (ctrl.Player.Button2.IsPressed())
        {
            rb.SetRotation(rb.rotation - rotationPower * Time.deltaTime);
        }

        if (hp <= 0) Die();
    }

    private void FixedUpdate()
    {
        foreach (BlackHole hole in activeBlackHoles)
        {
            Vector2 separation = (Vector2)hole.transform.position - (Vector2)transform.position;
            float distSq = separation.sqrMagnitude + blackHoleSoftness * blackHoleSoftness;
            float forceMag = blackHoleGravity * hole.spaceObject.mass / distSq;
            rb.AddForce(forceMag * separation.normalized);

            if (separation.magnitude < 0.3f) rb.linearVelocity *= 0.03f;
        }
    }

    private void Fire(InputAction.CallbackContext ctx)
    {
        StartCoroutine(hook.SendHook());
    }

    public void Die()
    {
        Debug.Log("You died lmao");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hook.state == Hook.HookState.Pulling && collision.gameObject.CompareTag("Object"))
        {
            hook.StopAllCoroutines();
            StartCoroutine(hook.Retract());

            SpaceObject spaceObject = collision.gameObject.GetComponent<SpaceObject>();
            if (spaceObject.isCollectable)
            {
                Item item = collision.gameObject.GetComponent<Item>();
                item.Collect();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BlackHole"))
        {
            BlackHole blackHole = collision.gameObject.GetComponent<BlackHole>();
            hp -= blackHoleDamageFactor * Time.fixedDeltaTime * math.log2(blackHole.spaceObject.mass);
        }
    }
}
