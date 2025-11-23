using TMPro;
using UnityEngine;

public class MoneyPopup : MonoBehaviour
{
    public TextMeshProUGUI text;

    public float duration = 1.0f;
    public AnimationCurve positionCurve;
    public AnimationCurve scaleCurve;
    public AnimationCurve fadeCurve;
    public Vector2 radiusRange = new(0.5f, 1.0f);
    public float offsetScale = 10.0f;

    public Vector3 startPosition;
    public Color startColor;
    public float time = 0.0f;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void Initialize(int money, Vector3 origin)
    {
        if (money >= 0)
        {
            text.text = $"+${money}";
        }
        else
        {
            text.text = $"-${Mathf.Abs(money)}";
        }

        Vector3 randomOffset = radiusRange * Random.insideUnitCircle;
        startPosition = origin + randomOffset;
        transform.localScale = Vector3.one;
        startColor = text.color;
        time = 0.0f;
    }

    private void Update()
    {
        time += Time.deltaTime;
        float t = time / duration;

        float yOffset = positionCurve.Evaluate(t);
        float scale = scaleCurve.Evaluate(t);
        text.color = startColor * new Color(1.0f, 1.0f, 1.0f, fadeCurve.Evaluate(t));
        transform.position = startPosition + yOffset * Vector3.up;

        transform.localScale = Vector3.one * scale;

        if (t >= 1f) Destroy(gameObject);
    }
}
