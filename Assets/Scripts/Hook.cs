using System.Collections;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public LineRenderer hookLine;
    public Transform hookHead;
    public CircleCollider2D hookHeadCollider;

    public enum HookState
    {
        Idle,
        Sending,
        Pulling,
        Retracting,
    }

    public HookState state = HookState.Idle;
    public float currentHookLength = 0.0f;
    public Vector2 hookOrigin;
    public Vector2 hookDirection;
    public SpaceObject hookedObject = null;

    private void Awake()
    {
        hookHeadCollider = hookHead.GetComponent<CircleCollider2D>();
        HideHook();
    }

    private void FixedUpdate()
    {
    }

    public IEnumerator SendHook()
    {
        if (state != HookState.Idle) yield break;

        state = HookState.Sending;
        currentHookLength = 0.0f;
        hookOrigin = transform.position;
        float playerRotation = Player.instance.rb.rotation * Mathf.Deg2Rad;
        hookDirection = new(Mathf.Cos(playerRotation), Mathf.Sin(playerRotation));

        hookHead.gameObject.SetActive(true);
        hookLine.gameObject.SetActive(true);
        hookHead.position = hookOrigin;
        hookLine.SetPosition(0, hookOrigin);

        while (currentHookLength < Player.instance.hookRange)
        {
            hookOrigin = transform.position;
            currentHookLength += Player.instance.hookSpeed * Time.fixedDeltaTime;
            Vector2 headPos = hookOrigin + currentHookLength * hookDirection;
            hookLine.SetPosition(0, hookOrigin);
            hookLine.SetPosition(1, headPos);
            hookHead.SetPositionAndRotation(headPos, Quaternion.FromToRotation(Vector2.up, hookDirection));

            Collider2D[] collisions = Physics2D.OverlapCircleAll(hookHead.transform.position, hookHeadCollider.radius);
            foreach (var collision in collisions)
            {
                if (collision.gameObject.CompareTag("Object"))
                {
                    hookedObject = collision.GetComponent<SpaceObject>();
                    yield return StartCoroutine(Pull(hookHead.position - collision.transform.position));
                    yield break;
                }
            }

            yield return new WaitForFixedUpdate();
        }

        yield return Retract();
    }

    public IEnumerator Pull(Vector2 objHeadOffset)
    {
        state = HookState.Pulling;

        while (currentHookLength > 0)
        {
            hookOrigin = transform.position;
            Vector2 headPos = (Vector2)hookedObject.transform.position + objHeadOffset;
            currentHookLength = (headPos - hookOrigin).magnitude;
            hookLine.SetPosition(0, hookOrigin);
            hookLine.SetPosition(1, headPos);
            hookHead.SetPositionAndRotation(headPos, Quaternion.FromToRotation(Vector2.up, hookDirection));

            Player.instance.rb.AddForce(Player.instance.hookPullForce * hookDirection);
            hookedObject.rb.AddForce(Player.instance.hookPullForce * (Player.instance.transform.position - hookedObject.transform.position).normalized);

            yield return new WaitForFixedUpdate();
        }

        HideHook();
    }

    public IEnumerator Retract()
    {
        state = HookState.Retracting;

        while (currentHookLength > 0)
        {
            hookOrigin = transform.position;
            currentHookLength -= Player.instance.hookSpeed * Time.fixedDeltaTime;
            Vector2 headPos = hookOrigin + currentHookLength * hookDirection;
            hookLine.SetPosition(0, hookOrigin);
            hookLine.SetPosition(1, headPos);
            hookHead.SetPositionAndRotation(headPos, Quaternion.FromToRotation(Vector2.up, hookDirection));

            yield return new WaitForFixedUpdate();
        }

        HideHook();
    }

    public void HideHook()
    {
        state = HookState.Idle;
        hookedObject = null;
        hookHead.gameObject.SetActive(false);
        hookLine.gameObject.SetActive(false);
    }
}
