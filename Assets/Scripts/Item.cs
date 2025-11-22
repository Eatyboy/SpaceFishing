using UnityEngine;

public class Item : MonoBehaviour
{
    public int value = 10;
    public Vector2 valueRange = new(0f, 100f);

    public void Initialize(int newValue)
    {
        value = newValue;
    }

    public void Collect()
    {
        if (Player.instance != null)
        {
            Player.instance.money += value;
        }

        Destroy(gameObject);
    }
}
