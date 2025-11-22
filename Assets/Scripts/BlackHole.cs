using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public SpaceObject spaceObject;

    private void Awake()
    {
        spaceObject = GetComponent<SpaceObject>();
    }

    private void OnEnable()
    {
        Player.instance.activeBlackHoles.Add(this);
    }

    private void OnDisable()
    {
        Player.instance.activeBlackHoles.Remove(this);
    }
}
