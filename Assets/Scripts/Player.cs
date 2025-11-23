using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player instance;

    private InputSystem_Actions ctrl;

    public Rigidbody2D rb;
    public Hook hook;
    public DeathScreen deathScreen;

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
    public int startingMoney = 100;
    public float maxHp = 10.0f;
    public float hp = 10.0f;

    public int hookCost = 5;
    public int hookCostIncrease = 1;
    public int currentHookCost;

    public bool gameStarted = false;
    private bool isDying = false;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        ctrl = new();
        rb.mass = mass;
        hp = maxHp;
        money = startingMoney;
        currentHookCost = hookCost;
        deathScreen.gameObject.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(StartGame());
    }

    public IEnumerator StartGame()
    {
        gameStarted = false;

        if (Fader.instance != null)
        {
            yield return Fader.instance.FadeIn();
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(AudioManager.Instance.gameplayMusic);
        }

        gameStarted = true;
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
        if (!gameStarted || isDying) return;

        if (ctrl.Player.Button1.IsPressed() && !ctrl.Player.Button2.IsPressed())
        {
            rb.SetRotation(rb.rotation + rotationPower * Time.deltaTime);
        } else if (ctrl.Player.Button2.IsPressed() && !ctrl.Player.Button1.IsPressed())
        {
            rb.SetRotation(rb.rotation - rotationPower * Time.deltaTime);
        }

        if (hp <= 0 || (money < currentHookCost && hook.state == Hook.HookState.Idle))
        {
            StartCoroutine(Die());
        }
    }

    private void FixedUpdate()
    {
        if (!gameStarted) return;

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
        if (!gameStarted) return;
        if (hook.state != Hook.HookState.Idle) return;

        if (money >= currentHookCost)
        {
            money -= currentHookCost;
            UIManager.instance.CostPopup(currentHookCost);
            currentHookCost += hookCostIncrease;
            StartCoroutine(hook.SendHook());
        }
    }

    [ContextMenu("Kill Player")]
    public void KillPlayer()
    {
        StartCoroutine(Die());
    }

    public IEnumerator Die()
    {
        isDying = true;
        gameStarted = false;

        yield return hook.Retract();

        AudioManager.Instance.StopMusic();

        yield return new WaitForSeconds(0.5f);

        deathScreen.gameObject.SetActive(true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!gameStarted) return;

        if (collision.gameObject.CompareTag("Object"))
        {
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.shipHit);

            HazardousItem hazardousItem = collision.gameObject.GetComponent<HazardousItem>();
            if (hazardousItem != null)
            {
                hazardousItem.HitPlayer();
                return;
            }

            SpaceObject spaceObject = collision.gameObject.GetComponent<SpaceObject>();
            switch (spaceObject.soundType)
            {
                case SpaceObject.SoundType.None: break;
                case SpaceObject.SoundType.Metal:
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.metal);
                    break;
                case SpaceObject.SoundType.Rock: 
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.rock);
                    break;
                case SpaceObject.SoundType.Cow: 
                    AudioManager.Instance.PlayOneShot(AudioManager.Instance.moo);
                    break;
                default: break;
            }
        }

        if (hook.state == Hook.HookState.Pulling && collision.gameObject.CompareTag("Object") && collision.gameObject == hook.hookedObject.gameObject)
        {
            hook.StopAllCoroutines();
            StartCoroutine(hook.Retract());

            SpaceObject spaceObject = collision.gameObject.GetComponent<SpaceObject>();
            if (spaceObject.isCollectable)
            {
                Item item = collision.gameObject.GetComponent<Item>();
                if (item != null)
                {
                    item.Collect();
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!gameStarted) return;

        if (collision.gameObject.CompareTag("BlackHole"))
        {
            BlackHole blackHole = collision.gameObject.GetComponent<BlackHole>();
            hp -= blackHoleDamageFactor * Time.fixedDeltaTime * math.log2(blackHole.spaceObject.mass);
        }
    }
}
