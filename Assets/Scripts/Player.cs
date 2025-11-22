using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player instance;

    private InputSystem_Actions ctrl;

    public Rigidbody2D rb;
    public Hook hook;

    public float mass = 1.0f;
    public float rotationPower = 1.0f;

    public float hookRange = 1.0f;
    public float hookSpeed = 1.0f;
    public float hookPullForce = 1.0f;

    public int money = 0;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        ctrl = new();
        rb.mass = mass;
    }

    private void OnEnable()
    {
        ctrl.Enable();
        ctrl.Player.Button1.performed += Fire;
    }

    private void OnDisable()
    {
        ctrl.Player.Button1.performed -= Fire;
        ctrl.Disable();
    }

    public void Update()
    {
        if (ctrl.Player.Button2.IsPressed())
        {
            rb.SetRotation(rb.rotation + rotationPower * Time.deltaTime);
        }
    }

    private void Fire(InputAction.CallbackContext ctx)
    {
        StartCoroutine(hook.SendHook());
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
}
